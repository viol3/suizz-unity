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
    [SerializeField] private GameObject _startGameButton;
    [Space]
    [SerializeField] private TMP_InputField _roomCodeInput;
    [SerializeField] private TMP_Text _roomCodeText;
    [SerializeField] private TMP_Text _nickNameText;
    [SerializeField] private TMP_Text _waitingForPlayersText;
    [SerializeField] private TMP_Text _gameStartingPlayersText;
    [Space]
    [SerializeField] private Transform _playerListParent;

    private string _nickname = "";
    private string _suiAddress = "";
    private bool _isHost = false;
    private bool _gameStarted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus.OnLoginSuccess.AddListener(OnLoginSuccess);
        EventBus.OnCreatedRoomSuccess.AddListener(OnCreatedRoom);
        EventBus.OnJoinRoomSuccess.AddListener(OnJoinRoomSuccess);
        EventBus.OnRoomUpdate.AddListener(OnRoomUpdate);
        EventBus.OnQuestionReceived.AddListener(OnQuestionReceived);
        EventBus.OnQuestionEnded.AddListener(OnQuestionEnded);
        EventBus.OnLeaderboardReceived.AddListener(OnLeaderboardReceived);
    }

    private void OnDestroy()
    {
        EventBus.OnLoginSuccess.RemoveListener(OnLoginSuccess);
        EventBus.OnCreatedRoomSuccess.RemoveListener(OnCreatedRoom);
        EventBus.OnJoinRoomSuccess.RemoveListener(OnJoinRoomSuccess);
        EventBus.OnRoomUpdate.RemoveListener(OnRoomUpdate);
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
        WebSocketManager.Instance.ConnectServer();
    }

    void OnCreatedRoom(string code)
    {
        _roomCodeText.text = code;
        LoadingManager.Instance.SetLoadingActive(false);
        _createRoomPanel.SetActive(true);
        _isHost = true;
    }

    void OnJoinRoomSuccess()
    {
        LoadingManager.Instance.SetLoadingActive(false);
        _roomCodeText.text = _roomCodeInput.text;
        _createRoomPanel.SetActive(true);
        _isHost = false;
    }

    void OnRoomUpdate(UserData[] users)
    {
        for (int i = 0; i < _playerParent.childCount; i++)
        {
            KahootPlayer player = _playerParent.GetChild(i).GetComponent<KahootPlayer>();
            player.gameObject.SetActive(false);
        }

        for (int i = 0; i < _playerListParent.childCount; i++)
        {
            Image playerIcon = _playerListParent.GetChild(i).GetComponent<Image>();
            GameUtility.ChangeAlphaImage(playerIcon, 0.5f);
        }

        for (int i = 0; i < users.Length; i++)
        {
            KahootPlayer player = _playerParent.GetChild(i).GetComponent<KahootPlayer>();
            player.SetData(users[i].name, users[i].id);
            player.gameObject.SetActive(true);
            Image playerIcon = _playerListParent.GetChild(i).GetComponent<Image>();
            GameUtility.ChangeAlphaImage(playerIcon, 1f);
        }
        _waitingForPlayersText.gameObject.SetActive(users.Length < 4);
        if (_isHost)
        {
            _gameStartingPlayersText.gameObject.SetActive(false);
            _startGameButton.SetActive(users.Length == 4);
        }
        else
        {
            _gameStartingPlayersText.gameObject.SetActive(users.Length == 4);
            _startGameButton.SetActive(false);
        }
    }


    void OnQuestionReceived(string question, string answersText)
    {
        if(!_gameStarted)
        {
            _createRoomPanel.SetActive(false);
            _gameStartingPlayersText.gameObject.SetActive(false);
            InitPlayersForGameplay();
            LoadingManager.Instance.SetLoadingActive(false);
            _gameStarted = true;
        }
        QuestionManager.Instance.SetPanelActive(true);
        string[] answers = answersText.Split('|');
        QuestionManager.Instance.SetQuestion(question);
        QuestionManager.Instance.SetAnswers(answers);
        CounterManager.Instance.StartCounter(20);
    }

    void OnQuestionEnded(UserData[] users)
    {
        QuestionManager.Instance.SetPanelActive(false);
        CounterManager.Instance.StopCounter();
        for (int i = 0; i < users.Length; i++)
        {
            KahootPlayer player = GetPlayerFromAddress(users[i].id);
            if(player)
            {
                player.ApplyScore(users[i].score);
            }
        }
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

    public void OnJoinRoomButtonClick()
    {
        _roomPanel.gameObject.SetActive(false);
        LoadingManager.Instance.SetLoadingActive(true);
        string roomCode = _roomCodeInput.text;
        WebSocketManager.Instance.SendJoinRoom(roomCode);
    }

    public void OnCreateRoomButtonClick()
    {
        StartCoroutine(CreateRoomProcess());
    }

    public void OnStartGameButtonClick()
    {
        if(!_isHost)
        {
            return;
        }
        WebSocketManager.Instance.SendStartGame(_roomCodeText.text);
        _createRoomPanel.SetActive(false);
        LoadingManager.Instance.SetLoadingActive(true);
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
        WebSocketManager.Instance.SendCreateRoom();
        yield return null;
    }


    void InitPlayersForGameplay()
    {
        for (int i = 0; i < _playerParent.childCount; i++)
        {
            _playerParent.GetChild(i).GetComponent<KahootPlayer>().InitForGameplay();
        }
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

    KahootPlayer GetPlayerFromAddress(string address)
    {
        for (int i = 0; i < _playerParent.childCount; i++)
        {
            KahootPlayer kp = _playerParent.GetChild(i).GetComponent<KahootPlayer>();
            if(kp.GetAddress().Equals(address))
            {
                return kp;
            }
        }
        return null;
    }

}
