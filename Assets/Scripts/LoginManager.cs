using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private GameObject _loginPanel;
    [Space]
    [SerializeField] private Button _loginButton;
    [DllImport("__Internal")]
    private static extern void LoginGoogle();

    private void Start()
    {

    }
    public void OnLoginButtonClick()
    {
        _loginButton.gameObject.SetActive(false);
        LoadingManager.Instance.SetLoadingActive(true);
#if !UNITY_EDITOR
        LoginGoogle();
#endif
    }

    public void OnWalletConnected(string wallet)
    {
        EventBus.OnLoginSuccess?.Invoke(GameNameGenerator.GenerateName(wallet), wallet);
    }

}
