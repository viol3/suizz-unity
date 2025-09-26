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
    [SerializeField] private TMP_Text _waitingForPlayersText;
    [SerializeField] private TMP_Text _gameStartingPlayersText;
    [Space]
    [SerializeField] private Transform _playerListParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus.OnLoginSuccess.AddListener(OnLoginSuccess);
        EventBus.OnCreatedRoomSuccess.AddListener(OnCreatedRoom);
        EventBus.OnNewPlayerJoined.AddListener(OnNewPlayerJoined);
        EventBus.OnGameStarted.AddListener(OnGameStarted);
        EventBus.OnQuestionReceived.AddListener(OnQuestionReceived);
        EventBus.OnQuestionEnded.AddListener(OnQuestionEnded);
    }

    private void Update()
    {
        HuntDeadPlayerInSameXWithShark();
    }

    public KahootTile GetTile(int index)
    {
        return _tileParent.GetChild(index).GetComponent<KahootTile>();
    }

    void OnLoginSuccess()
    {
        LoadingManager.Instance.SetLoadingActive(false);
        _roomPanel.SetActive(true);
    }

    void OnCreatedRoom(string code)
    {
        _roomCodeText.text = code;
        LoadingManager.Instance.SetLoadingActive(false);
        _createRoomPanel.SetActive(true);
    }

    void OnNewPlayerJoined(int skinIndex, string name)
    {
        KahootPlayer nextPlayer = ActivateNextPlayer(name);
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

    public void OnCreateRoomButtonClick()
    {
        StartCoroutine(CreateRoomProcess());
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

    KahootPlayer ActivateNextPlayer(string name)
    {
        for (int i = 0; i < _playerParent.childCount; i++)
        {
            if(!_playerParent.GetChild(i).gameObject.activeInHierarchy)
            {
                KahootPlayer kp = _playerParent.GetChild(i).GetComponent<KahootPlayer>();
                kp.SetName(name);
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
