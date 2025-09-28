using Ali.Helper;
using DG.Tweening;
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
    [SerializeField] private GameObject _confetti;
    [Space]
    [SerializeField] private RectTransform _logoRect;
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
        EventBus.OnTimerReached.AddListener(OnTimerReached);
        EventBus.OnHostAnswerReceived.AddListener(OnHostAnswerReceived);
        ShowLogo();
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
        EventBus.OnTimerReached.RemoveListener(OnTimerReached);
        EventBus.OnHostAnswerReceived.RemoveListener(OnHostAnswerReceived);
    }

    void ShowLogo()
    {
        _logoRect.DOKill(true);
        _logoRect.DOAnchorPosY(-300f, 1f).SetEase(Ease.OutBounce);
    }

    void HideLogo()
    {
        _logoRect.DOKill(true);
        _logoRect.DOAnchorPosY(500f, 1f).SetEase(Ease.Linear);
    }

    public bool IsHost()
    {
        return _isHost;
    }

    void OnTimerReached()
    {
        if(!_isHost)
        {
            return;
        }
        WebSocketManager.Instance.SendManualQuestionEnd(_roomCodeText.text);
    }

    public KahootTile GetTile(int index)
    {
        return _tileParent.GetChild(index).GetComponent<KahootTile>();
    }

    void OnHostAnswerReceived(string owner, int chosenIndex)
    {
        KahootPlayer player = GetPlayerFromAddress(owner);
        QuestionManager.Instance.ChosenFromAPlayer(player.GetColor(), chosenIndex);
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
        HideLogo();
        _roomCodeText.text = code;
        LoadingManager.Instance.SetLoadingActive(false);
        _createRoomPanel.SetActive(true);
        _isHost = true;
    }

    void OnJoinRoomSuccess()
    {
        HideLogo();
        LoadingManager.Instance.SetLoadingActive(false);
        _roomCodeText.text = _roomCodeInput.text;
        _createRoomPanel.SetActive(true);
        _isHost = false;
    }

    void OnRoomUpdate(UserData[] users)
    {
        Debug.Log("users length => " + users.Length);
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
        QuestionManager.Instance.Animation();
        CounterManager.Instance.StartCounter(10);
    }

    void OnQuestionEnded(UserData[] users, int correctIndex)
    {
        StartCoroutine(OnQuestionEndedProcess(users, correctIndex));
    }

    IEnumerator OnQuestionEndedProcess(UserData[] users, int correctIndex)
    {
        CounterManager.Instance.StopCounter();
        QuestionManager.Instance.LightOption(correctIndex);
        yield return new WaitForSeconds(2f);
        QuestionManager.Instance.SetPanelActive(false);
        CounterManager.Instance.StopCounter();
        for (int i = 0; i < users.Length; i++)
        {
            KahootPlayer player = GetPlayerFromAddress(users[i].id);
            if (player)
            {
                player.ApplyScore(users[i].score);
            }
        }
    }

    void OnLeaderboardReceived(UserData[] users)
    {
        int myRank = -1;
        for (int i = 0; i < users.Length; i++)
        {
            if(users[i].id.Equals(_suiAddress))
            {
                myRank = i + 1;
                break;
            }
        }
        GameOverManager.Instance.SetRankNames(users);
        GameOverManager.Instance.SetRank(myRank);
        GameOverManager.Instance.SetPanelActive(true);
        _confetti.SetActive(true);
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

    public void OnCreateBotRoomButtonClick()
    {
        _roomPanel.gameObject.SetActive(false);
        LoadingManager.Instance.SetLoadingActive(true);
        WebSocketManager.Instance.SendCreateBotRoom();
        _isHost = true;
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
