using TMPro;
using UnityEngine;

public class RankPlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    
    public void SetName(string name)
    {
        _nameText.text = name;
    }
}
