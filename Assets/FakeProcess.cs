using UnityEngine;

public class FakeProcess : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            EventBus.OnLoginSuccess?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            EventBus.OnCreatedRoomSuccess?.Invoke("ec33gf");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            EventBus.OnNewPlayerJoined?.Invoke(-1, "0xali");
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
