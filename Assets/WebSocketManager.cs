using UnityEngine;
using NativeWebSocket;
public class WebSocketManager : MonoBehaviour
{
    private WebSocket _websocket;

    void Start()
    {
        Connect();

    }

    async void Connect()
    {
        _websocket = new WebSocket("ws://localhost:1380");

        _websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        _websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        _websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        _websocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");
            Debug.Log(bytes);
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage! " + message);
        };

        await _websocket.Connect();
    }


    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _websocket.DispatchMessageQueue();
#endif
    }
}
