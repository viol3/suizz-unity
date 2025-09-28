using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionOption : MonoBehaviour
{
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _clickedColor;
    [SerializeField] private Color _correctColor;
    [Space]
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _badgeParent;
    [SerializeField] private TMP_Text _textComponent;

    private bool _chosen = false;

    public bool IsChosen()
    {
        return _chosen;
    }

    public void Light()
    {
        _image.color = _correctColor;
    }
    
    public void SetText(string text)
    {
        _textComponent.text = text;
    }

    public void PutPlayerBadge(Color color)
    {
        for (int i = 0; i < _badgeParent.childCount; i++)
        {
            if(!_badgeParent.GetChild(i).gameObject.activeInHierarchy)
            {
                _badgeParent.GetChild(i).gameObject.SetActive(true);
                _badgeParent.GetChild(i).GetComponent<Image>().color = color;
                return;
            }
        }
    }

    public void ResetChosen()
    {
        _chosen = false;
        _image.color = _normalColor;

        for (int i = 0; i < _badgeParent.childCount; i++)
        {
            _badgeParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        if(_chosen || QuestionManager.Instance.IsAnyButtonChosen() || KahootGameManager.Instance.IsHost())
        {
            return;
        }
        _image.color = _clickedColor;
        QuestionManager.Instance.OnOptionClicked(transform.GetSiblingIndex());
        _chosen = true;
    }
}
