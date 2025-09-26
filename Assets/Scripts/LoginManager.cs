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

    public void OnLoginButtonClick()
    {
        StartCoroutine(LoginProcess());
    }

    IEnumerator LoginProcess()
    {
        _loginButton.gameObject.SetActive(false);
        LoadingManager.Instance.SetLoadingActive(true);
        yield return null;
    }
}
