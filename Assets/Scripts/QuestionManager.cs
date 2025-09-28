using Ali.Helper;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class QuestionManager : LocalSingleton<QuestionManager>
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private RectTransform _questionBack;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private QuestionOption[] _options;

    public void Animation()
    {
        StartCoroutine(AnimationProcess());
    }

    public void ChosenFromAPlayer(Color color, int index)
    {
        _options[index].PutPlayerBadge(color);
    }

    public void LightOption(int index)
    {
        _options[index].Light();
    }

    public bool IsAnyButtonChosen()
    {
        for (int i = 0; i < _options.Length; i++)
        {
            if (_options[i].IsChosen())
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator AnimationProcess()
    {
        _questionText.gameObject.SetActive(false);
        for (int i = 0; i < _options.Length; i++)
        {
            _options[i].gameObject.SetActive(false);
        }
        Vector2 oldSizeDelta = _questionBack.sizeDelta;
        _questionBack.sizeDelta = new Vector2(20, 20f);
        Vector2 targetSizeDelta = _questionBack.sizeDelta;
        targetSizeDelta.y = oldSizeDelta.y;
        _questionBack.DOSizeDelta(targetSizeDelta, 0.5f);
        yield return new WaitForSeconds(0.5f);
        targetSizeDelta.x = oldSizeDelta.x;
        _questionBack.DOSizeDelta(targetSizeDelta, 0.5f);
        yield return new WaitForSeconds(0.5f);
        _questionText.gameObject.SetActive(true);
        for (int i = 0; i < _options.Length; i++)
        {
            _options[i].gameObject.SetActive(true);
            _options[i].transform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 6);
            yield return new WaitForSeconds(0.2f);
        }

    }

    public void SetQuestion(string question)
    {
        _questionText.text = question;
    }

    public void SetAnswers(params string[] answers)
    {

        for (int i = 0; i < _options.Length; i++)
        {
            _options[i].SetText(answers[i]);
            _options[i].ResetChosen();
        }
    }

    public void OnOptionClicked(int index)
    {
        EventBus.OnQuestionOptionClicked?.Invoke(index);
        for (int i = 0; i < _options.Length; i++)
        {
            if(i == index)
            {
                continue;
            }
        }
    }

    public void SetPanelActive(bool active)
    {
        _panel.gameObject.SetActive(active);
    }
}
