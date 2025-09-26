using Ali.Helper;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KahootGameManager : LocalSingleton<KahootGameManager>
{
    [SerializeField] private KahootShark _shark;
    [Space]
    [SerializeField] private Transform _playerParent;
    [SerializeField] private Transform _tileParent;
    [Space]
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _createRoomPanel;
    [Space]
    [SerializeField] private TMP_Text _roomCodeText;
    [SerializeField] private TMP_Text _nickNameText;
    [SerializeField] private TMP_Text _waitingForPlayersText;
    [SerializeField] private TMP_Text _gameStartingPlayersText;
    [Space]
    [SerializeField] private Transform _playerListParent;

    private string _nickname = "";
    private string _suiAddress = "";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus.OnLoginSuccess.AddListener(OnLoginSuccess);
        EventBus.OnCreatedRoomSuccess.AddListener(OnCreatedRoom);
        EventBus.OnNewPlayerJoined.AddListener(OnNewPlayerJoined);
        EventBus.OnGameStarted.AddListener(OnGameStarted);
        EventBus.OnQuestionReceived.AddListener(OnQuestionReceived);
        EventBus.OnQuestionEnded.AddListener(OnQuestionEnded);
        EventBus.OnLeaderboardReceived.AddListener(OnLeaderboardReceived);
    }

    private void OnDestroy()
    {
        EventBus.OnLoginSuccess.RemoveListener(OnLoginSuccess);
        EventBus.OnCreatedRoomSuccess.RemoveListener(OnCreatedRoom);
        EventBus.OnNewPlayerJoined.RemoveListener(OnNewPlayerJoined);
        EventBus.OnGameStarted.RemoveListener(OnGameStarted);
        EventBus.OnQuestionReceived.RemoveListener(OnQuestionReceived);
        EventBus.OnQuestionEnded.RemoveListener(OnQuestionEnded);
        EventBus.OnLeaderboardReceived.RemoveListener(OnLeaderboardReceived);
    }

    private void Update()
    {
        HuntDeadPlayerInSameXWithShark();
    }

    public KahootTile GetTile(int index)
    {
        return _tileParent.GetChild(index).GetComponent<KahootTile>();
    }

    void OnLoginSuccess(string nickname, string address)
    {
        LoadingManager.Instance.SetLoadingActive(false);
        _roomPanel.SetActive(true);
        _suiAddress = address;
        _nickname = nickname;
        _nickNameText.text = nickname;
        _nickNameText.gameObject.SetActive(true);
    }

    void OnCreatedRoom(string code)
    {
        _roomCodeText.text = code;
        LoadingManager.Instance.SetLoadingActive(false);
        _createRoomPanel.SetActive(true);
    }

    void OnNewPlayerJoined(int skinIndex, string name, string address)
    {
        KahootPlayer nextPlayer = ActivateNextPlayer(name, address);
        bool isLast = ActivateNextPlayerIcon();
        if(isLast)
        {
            _waitingForPlayersText.gameObject.SetActive(false);
            _gameStartingPlayersText.gameObject.SetActive(true);
        }
    }

    void OnGameStarted()
    {
        _createRoomPanel.SetActive(false);
        _gameStartingPlayersText.gameObject.SetActive(false);
        InitPlayersForGameplay();
    }

    void OnQuestionReceived(string question, string answersText)
    {
        
        string[] answers = answersText.Split('|');
        QuestionManager.Instance.SetQuestion(question);
        QuestionManager.Instance.SetAnswers(answers);
        QuestionManager.Instance.SetPanelActive(true);
        CounterManager.Instance.StartCounter(20);
    }

    void OnQuestionEnded(int player1Damage, int player2Damage, int player3Damage, int player4Damage)
    {
        QuestionManager.Instance.SetPanelActive(false);
        CounterManager.Instance.StopCounter();
        _playerParent.GetChild(0).GetComponent<KahootPlayer>().Damage(player1Damage);
        _playerParent.GetChild(1).GetComponent<KahootPlayer>().Damage(player2Damage);
        _playerParent.GetChild(2).GetComponent<KahootPlayer>().Damage(player3Damage);
        _playerParent.GetChild(3).GetComponent<KahootPlayer>().Damage(player4Damage);
    }

    void OnLeaderboardReceived(string rankAddressesText)
    {
        string[] rankAddresses = rankAddressesText.Split('|');
        string[] rankNames = GetNamesFromAddresses(rankAddresses);
        int myRank = -1;

        for (int i = 0; i < rankAddresses.Length; i++)
        {
            if(rankAddresses[i].Equals(_suiAddress))
            {
                myRank = i + 1;
                break;
            }
        }
        GameOverManager.Instance.SetRankNames(rankNames);
        GameOverManager.Instance.SetRank(myRank);
        GameOverManager.Instance.SetPanelActive(true);
    }

    string[] GetNamesFromAddresses(string[] addresses)
    {
        string[] result = new string[addresses.Length];
        for (int i = 0; i < addresses.Length; i++)
        {
            for (int j = 0; j < _playerParent.childCount; j++)
            {
                KahootPlayer kp = _playerParent.GetChild(i).GetComponent<KahootPlayer>();
                if(kp.GetAddress().Equals(addresses[i]))
                {
                    result[i] = kp.GetName();
                    break;
                }
            }
        }
        return result;
    }

    public void OnCreateRoomButtonClick()
    {
        StartCoroutine(CreateRoomProcess());
    }

    public string GetNickname()
    {
        return _nickname;
    }

    public string GetAddress()
    {
        return _suiAddress;
    }

    IEnumerator CreateRoomProcess()
    {
        _roomPanel.gameObject.SetActive(false);
        LoadingManager.Instance.SetLoadingActive(true);
        yield return null;
    }


    void InitPlayersForGameplay()
    {
        for (int i = 0; i < _playerParent.childCount; i++)
        {
            _playerParent.GetChild(i).GetComponent<KahootPlayer>().InitForGameplay();
        }
    }

    bool ActivateNextPlayerIcon()
    {
        for (int i = 0; i < _playerListParent.childCount; i++)
        {
            Image playerIcon = _playerListParent.GetChild(i).GetComponent<Image>();
            if(playerIcon.color.a < 0.8)
            {
                GameUtility.ChangeAlphaImage(playerIcon, 1f);
                return i == _playerListParent.childCount -1 ;
            }
        }
        return false;
    }

    KahootPlayer ActivateNextPlayer(string name, string address)
    {
        for (int i = 0; i < _playerParent.childCount; i++)
        {
            if(!_playerParent.GetChild(i).gameObject.activeInHierarchy)
            {
                KahootPlayer kp = _playerParent.GetChild(i).GetComponent<KahootPlayer>();
                kp.SetData(name, address);
                kp.gameObject.SetActive(true);
                return kp;
            }
        }
        return null ;
    }

    void HuntDeadPlayerInSameXWithShark()
    {
        for (int i = 0; i < _playerParent.childCount; i++)
        {
            KahootPlayer kp = _playerParent.GetChild(i).GetComponent<KahootPlayer>();
            if(!kp.IsDead() || kp.IsHunted())
            {
                continue;
            }
            if(Mathf.Abs(kp.transform.position.x - _shark.transform.position.x) < 0.1f)
            {
                _shark.Hunt(kp);
                return;
            }
        }
    }

}
