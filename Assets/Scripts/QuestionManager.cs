using Ali.Helper;
using TMPro;
using UnityEngine;

public class QuestionManager : LocalSingleton<QuestionManager>
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private QuestionOption[] _options;


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
    }

    public void SetPanelActive(bool active)
    {
        _panel.gameObject.SetActive(active);
    }
}
