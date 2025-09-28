using UnityEngine;
using NativeWebSocket;
using Ali.Helper;
using Newtonsoft.Json;

public class WebSocketManager : LocalSingleton<WebSocketManager>
{
    private WebSocket _websocket;
    private bool _connected = false;
    public void ConnectServer()
    {
        Connect();
    }

    async void Connect()
    {
        _websocket = new WebSocket("ws://147.93.90.167:1380");

        _websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            _connected = true;
        };

        _websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            _connected = false;
        };

        _websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            _connected = false;
        };

        _websocket.OnMessage += OnMessageReceived; 

        await _websocket.Connect();
    }

    void OnMessageReceived(byte[] bytes)
    {
        var message = System.Text.Encoding.UTF8.GetString(bytes);
        Debug.Log("WS Message => " + message);
        WebSocketMessage wsm = JsonConvert.DeserializeObject<WebSocketMessage>(message);
        switch(wsm.type)
        {
            case "createRoomResponse":
                var crrm = JsonConvert.DeserializeObject<CreateRoomResponseMessage>(message);
                HandleCreateRoomResponse(crrm);
                break;
            case "joinRoomResponse":
                var jrrm = JsonConvert.DeserializeObject<JoinRoomResponseMessage>(message);
                HandleJoinRoomResponse(jrrm);
                break;
            case "roomUpdateResponse":
                var rurm = JsonConvert.DeserializeObject<RoomUpdateResponseMessage>(message);
                HandleRoomUpdateResponse(rurm);
                break;
            case "nextQuestionResponse":
                var nqrm = JsonConvert.DeserializeObject<NextQuestionResponseMessage>(message);
                HandleNextQuestionResponse(nqrm);
                break;
            case "answerResultsResponse":
                var arrm = JsonConvert.DeserializeObject<AnswerResultsResponseMessage>(message);
                HandleAnswerResultsResponse(arrm);
                break;
            case "hostAnswerResponse":
                var harm = JsonConvert.DeserializeObject<HostAnswerResponseMessage>(message);
                HandleHostAnswerResponse(harm);
                break;
            case "gameOverResponse":
                var gorm = JsonConvert.DeserializeObject<GameOverResponseMessage>(message);
                HandleGameOverResponse(gorm);
                break;
            case "sponsoredTxResponse":
                var strm = JsonConvert.DeserializeObject<SponsoredTxResponseMessage>(message);
                HandleSponsoredTxResponse(strm);
                break;
            default:
                Debug.Log("nothing");
                break;
        }
    }

    public void SendCreateRoom()
    {
        CreateRoomRequestMessage crrm = new CreateRoomRequestMessage();
        crrm.owner = KahootGameManager.Instance.GetAddress();
        crrm.type = "createRoomRequest";
        Debug.Log(JsonConvert.SerializeObject(crrm));
        _websocket.SendText(JsonConvert.SerializeObject(crrm));
    }

    public void SendCreateBotRoom()
    {
        CreateBotRoomRequestMessage cbrrm = new CreateBotRoomRequestMessage();
        cbrrm.owner = KahootGameManager.Instance.GetAddress();
        cbrrm.type = "createBotRoomRequest";
        Debug.Log(JsonConvert.SerializeObject(cbrrm));
        _websocket.SendText(JsonConvert.SerializeObject(cbrrm));
    }

    public void SendJoinRoom(string roomCode)
    {
        JoinRoomRequestMessage jrrm = new JoinRoomRequestMessage();
        jrrm.owner = KahootGameManager.Instance.GetAddress();
        jrrm.type = "joinRoomRequest";
        jrrm.code = roomCode;
        jrrm.ownerName = KahootGameManager.Instance.GetNickname();
        _websocket.SendText(JsonConvert.SerializeObject(jrrm));
    }

    public void SendStartGame(string roomCode)
    {
        StartGameRequestMessage sgrm = new StartGameRequestMessage();
        sgrm.owner = KahootGameManager.Instance.GetAddress();
        sgrm.type = "startGameRequest";
        sgrm.code = roomCode;
        _websocket.SendText(JsonConvert.SerializeObject(sgrm));
    }

    public void SendSubmitAnswer(string roomCode, int optionIndex)
    {
        SubmitAnswerRequestMessage sarm = new SubmitAnswerRequestMessage();
        sarm.owner = KahootGameManager.Instance.GetAddress();
        sarm.type = "submitAnswerRequest";
        sarm.code = roomCode;
        sarm.optionIndex = optionIndex;
        _websocket.SendText(JsonConvert.SerializeObject(sarm));
    }

    public void SendManualQuestionEnd(string roomCode)
    {
        ManualQuestionEndRequestMessage mqem = new ManualQuestionEndRequestMessage();
        mqem.owner = KahootGameManager.Instance.GetAddress();
        mqem.type = "manualQuestionEndRequest";
        mqem.code = roomCode;
        _websocket.SendText(JsonConvert.SerializeObject(mqem));
    }

    public void SendSponsoredSignature(string signature, string digest)
    {
        SponsoredSignatureRequest ssr = new SponsoredSignatureRequest();
        ssr.type = "sponsoredSignatureRequest";
        ssr.signature = signature;
        ssr.digest = digest;
        _websocket.SendText(JsonConvert.SerializeObject(ssr));
    }

    public void SendEnokiTest()
    {
        EnokiSponsorRequestMessage esrm = new EnokiSponsorRequestMessage();
        esrm.type = "enokiSponsorRequest";
        _websocket.SendText(JsonConvert.SerializeObject(esrm));
    }

    void HandleCreateRoomResponse(CreateRoomResponseMessage crrm)
    {
        EventBus.OnCreatedRoomSuccess?.Invoke(crrm.message);
    }

    void HandleJoinRoomResponse(JoinRoomResponseMessage jrrm)
    {
        if(jrrm.message.Equals("success"))
        {
            EventBus.OnJoinRoomSuccess.Invoke();
            EventBus.OnRoomUpdate.Invoke(jrrm.users);
        }
        
    }

    void HandleRoomUpdateResponse(RoomUpdateResponseMessage rurm)
    {
        EventBus.OnRoomUpdate?.Invoke(rurm.users);
    }

    void HandleNextQuestionResponse(NextQuestionResponseMessage nqrm)
    {
        string options = string.Join('|', nqrm.options);
        EventBus.OnQuestionReceived?.Invoke(nqrm.question, options);
    }

    void HandleAnswerResultsResponse(AnswerResultsResponseMessage arrm)
    {
        EventBus.OnQuestionEnded?.Invoke(arrm.users, arrm.correctIndex);
    }

    void HandleGameOverResponse(GameOverResponseMessage gorm)
    {
        EventBus.OnLeaderboardReceived?.Invoke(gorm.ranking);
    }

    void HandleSponsoredTxResponse(SponsoredTxResponseMessage strm)
    {
        Debug.Log("Sponsored Byte Length => " + strm.bytes.Length);
        Debug.Log("Sponsored Digest => " + strm.digest);
        EventBus.OnSponsoredTxArrived?.Invoke(strm.bytes, strm.digest);
    }

    void HandleHostAnswerResponse(HostAnswerResponseMessage harm)
    {
        EventBus.OnHostAnswerReceived?.Invoke(harm.owner, harm.chosenIndex);
    }


    void Update()
    {
        if(_connected)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _websocket.DispatchMessageQueue();
#endif
        }
    }
}

public class WebSocketMessage
{
    public string type;
    public string message;
}

public class CreateRoomRequestMessage : WebSocketMessage
{
    public string owner;
}

public class CreateBotRoomRequestMessage : WebSocketMessage
{
    public string owner;
}

public class CreateRoomResponseMessage : WebSocketMessage
{
    
}

public class JoinRoomRequestMessage : WebSocketMessage
{
    public string owner;
    public string ownerName;
    public string code;
}

public class StartGameRequestMessage : WebSocketMessage
{
    public string owner;
    public string code;
}

public class JoinRoomResponseMessage : WebSocketMessage
{
    public UserData[] users;
}

public class RoomUpdateResponseMessage : WebSocketMessage
{
    public UserData[] users;
}


public class NextQuestionResponseMessage : WebSocketMessage
{
    public int questionIndex;
    public string question;
    public string[] options;
}

public class GameOverResponseMessage : WebSocketMessage
{
    public UserData[] ranking;
}

public class SubmitAnswerRequestMessage : WebSocketMessage
{
    public string code;
    public string owner;
    public int optionIndex;
}

public class ManualQuestionEndRequestMessage : WebSocketMessage
{
    public string owner;
    public string code;
}

public class AnswerResultsResponseMessage : WebSocketMessage
{
    public UserData[] users;
    public int correctIndex;
}

public class HostAnswerResponseMessage : WebSocketMessage
{
    public string owner;
    public int chosenIndex;
}

public class EnokiSponsorRequestMessage : WebSocketMessage
{

}

public class SponsoredTxResponseMessage : WebSocketMessage
{
    public string bytes;
    public string digest;
}

public class SponsoredSignatureRequest : WebSocketMessage
{
    public string signature;
    public string digest;
}

public class UserData
{
    public string id;
    public string name;
    public float score;
}

public class QuestionData
{
    public string question;
    public string[] options;
    public int correctIndex;
}


