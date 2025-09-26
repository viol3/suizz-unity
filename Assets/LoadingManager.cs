using Ali.Helper;
using UnityEngine;

public class LoadingManager : GenericSingleton<LoadingManager>
{
    [SerializeField] private GameObject _panel;
    
    public void SetLoadingActive(bool value)
    {
        _panel.SetActive(value);
    }
}
