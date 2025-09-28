using UnityEngine;
using UnityEngine.Events;

public static class EventBus
{
    public static UnityEvent<string, string> OnLoginSuccess = new UnityEvent<string, string>();
    public static UnityEvent OnJoinRoomSuccess = new UnityEvent();
    public static UnityEvent<string> OnCreatedRoomSuccess = new UnityEvent<string>();
    public static UnityEvent<UserData[]> OnRoomUpdate = new UnityEvent<UserData[]>();
    public static UnityEvent<string, string> OnQuestionReceived = new UnityEvent<string, string>();
    public static UnityEvent<UserData[], int> OnQuestionEnded = new UnityEvent<UserData[], int>();
    public static UnityEvent<UserData[]> OnLeaderboardReceived = new UnityEvent<UserData[]>();
    public static UnityEvent<string, string> OnSponsoredTxArrived = new UnityEvent<string, string>();
    public static UnityEvent<string, int> OnHostAnswerReceived = new UnityEvent<string, int>();

    public static UnityEvent<int> OnQuestionOptionClicked = new UnityEvent<int>();

    public static UnityEvent OnTimerReached = new UnityEvent();

    public static UnityEvent<KahootPlayer> OnPlayerDead = new UnityEvent<KahootPlayer>();
}
