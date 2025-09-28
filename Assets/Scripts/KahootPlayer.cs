using Ali.Helper;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class KahootPlayer : MonoBehaviour
{
    [SerializeField] private Color _color;
    [SerializeField] private float _offsetPerDamage = -1f;
    [SerializeField] private float _damageSpeed = 0.5f;
    [Space]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private ParticleSystem _splashParticle;
    [SerializeField] private KahootShark _shark;

    private KahootTile _tile;

    private bool _dead = false;
    private bool _hunted = false;

    private float _hp = 5;

    private string _nickname = "";
    private string _address = "";
    public void InitForGameplay()
    {
        _tile = KahootGameManager.Instance.GetTile(transform.GetSiblingIndex());
        _tile.gameObject.SetActive(true);
        _tile.transform.position = transform.position;
        _tile.transform.DOMoveY(6f, 1f).SetEase(Ease.InOutSine);
        transform.DOMoveY(6f, 1f).SetEase(Ease.InOutSine);
        _hp = 5;
    }

    public Color GetColor()
    {
        return _color;
    }

    public bool IsDead()
    {
        return _dead;
    }

    public bool IsHunted()
    {
        return _hunted;
    }

    public void SetData(string name, string address)
    {
        _nameText.text = name;
        _address = address;
        _nickname = name;
    }

    public bool IsMe()
    {
        return _address.Equals(KahootGameManager.Instance.GetAddress());
    }

    public string GetName()
    {
        return _nickname;
    }

    public string GetAddress()
    {
        return _address;
    }

    public void Kill()
    {
        _mesh.gameObject.SetActive(false);
        _nameText.gameObject.SetActive(false);
        _splashParticle.Play();
        _tile.gameObject.SetActive(false);
    }

    void Hunt()
    {
        _hunted = true;
        _shark.gameObject.SetActive(true);
        _shark.Hunt(this);
    }

    public void ApplyScore(float score)
    {
        float y = GameUtility.GetValueFromRatio(score / 5f, 0f, 6f);
        _hp = score;
        StartCoroutine(ApplyScoreProcess(y));
    }

    IEnumerator ApplyScoreProcess(float newY)
    {
        bool death = false;
        
        if(newY < 0)
        {
            death = true;
        }
        transform.DOMoveY(newY, _damageSpeed).SetSpeedBased();
        yield return _tile.transform.DOMoveY(newY, _damageSpeed).SetSpeedBased().WaitForCompletion();
        if(death)
        {
            _dead = true;
            _tile.gameObject.SetActive(false);
            _nameText.color = Color.red;
            EventBus.OnPlayerDead?.Invoke(this);
            Hunt();
        }
    }
}
