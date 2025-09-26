using UnityEngine;

public class FakeProcess : MonoBehaviour
{

    private int _playerCounter = 1;
   
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            EventBus.OnLoginSuccess?.Invoke("0xplayer0", "0x8880");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            EventBus.OnCreatedRoomSuccess?.Invoke("ec33gf");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            EventBus.OnNewPlayerJoined?.Invoke(-1, "trey" + _playerCounter, "0x888" + _playerCounter);
            _playerCounter++;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            EventBus.OnGameStarted?.Invoke();
            EventBus.OnQuestionReceived?.Invoke("Which blockchain is best", "Sui|Sui|Sui|Sui");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            EventBus.OnQuestionEnded?.Invoke(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2));
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            EventBus.OnQuestionReceived?.Invoke("Which blockchain is best", "Sui|Sui|Sui|Sui");
        }
    }
}
