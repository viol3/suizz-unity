using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionOption : MonoBehaviour
{
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _clickedColor;
    [Space]
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _textComponent;

    private bool _chosen = false;
    
    public void SetText(string text)
    {
        _textComponent.text = text;
    }

    public void ResetChosen()
    {
        _chosen = false;
        _image.color = _normalColor;
    }

    public void OnClick()
    {
        if(_chosen)
        {
            return;
        }
        _image.color = _clickedColor;
        QuestionManager.Instance.OnOptionClicked(transform.GetSiblingIndex());
        _chosen = true;
    }
}
