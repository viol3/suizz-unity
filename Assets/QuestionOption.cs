using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionOption : MonoBehaviour
{
    [SerializeField] private Color _clickedColor;
    [Space]
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _textComponent;
    
    public void SetText(string text)
    {
        _textComponent.text = text;
    }

    public void OnClick()
    {
        _image.color = _clickedColor;
        QuestionManager.Instance.OnOptionClicked(transform.GetSiblingIndex());
    }
}
