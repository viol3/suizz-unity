using Ali.Helper;
using System.Collections;
using TMPro;
using UnityEngine;

public class CounterManager : LocalSingleton<CounterManager>
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _counterText;

    public void StartCounter(int max)
    {
        StartCoroutine(CounterProcess(max));
    }

    public void StopCounter()
    {
        StopAllCoroutines();
        _panel.gameObject.SetActive(false);
    }

    IEnumerator CounterProcess(int max)
    {
        _panel.gameObject.SetActive(true);
        for (int i = max; i >= 0; i--)
        {
            _counterText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        _panel.gameObject.SetActive(false);
        EventBus.OnTimerReached?.Invoke();
    }
}
