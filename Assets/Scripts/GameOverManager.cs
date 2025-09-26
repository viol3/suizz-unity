using Ali.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : LocalSingleton<GameOverManager>
{
    [SerializeField] private GameObject _panel;
    [Space]
    [SerializeField] private GameObject _trophy;
    [SerializeField] private GameObject _gameOverText;
    [Space]
    [SerializeField] private TMP_Text _rankText;
    [Space]
    [SerializeField] private RankPlayerUI[] _rankPlayerUIs;

    public void SetPanelActive(bool active)
    {
        _panel.SetActive(active);
    }

    public void SetRankNames(params string[] names)
    {
        for (int i = 0; i < _rankPlayerUIs.Length; i++)
        {
            _rankPlayerUIs[i].SetName(names[i]);
        }
    }

    public void SetRank(int rank)
    {
        if(rank == 1)
        {
            _trophy.SetActive(true);
            _gameOverText.SetActive(false);
            _rankText.text = "1st";
        }
        else if (rank == 2)
        {
            _rankText.text = "2st";
        }
        else if (rank == 3)
        {
            _rankText.text = "3rd";
        }
        else if (rank == 4)
        {
            _rankText.text = "4th";
        }
    }

    public void OnHomeButtonClick()
    {
        SceneManager.LoadScene("Main");
    }
}
