using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class KahootPlayer : MonoBehaviour
{
    [SerializeField] private float _offsetPerDamage = -1f;
    [SerializeField] private float _damageSpeed = 0.5f;
    [Space]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private GameObject _mesh;
    [SerializeField] private ParticleSystem _splashParticle;

    private KahootTile _tile;

    private bool _dead = false;
    private bool _hunted = false;

    private int _hp = 5;

    private string _nickname = "";
    private string _address = "";
    public void InitForGameplay()
    {
        _tile = KahootGameManager.Instance.GetTile(transform.GetSiblingIndex());
        _tile.gameObject.SetActive(true);
        _tile.transform.position = transform.position;
        _tile.transform.DOMoveY(5.8f, 1f).SetEase(Ease.InOutSine);
        transform.DOMoveY(5.8f, 1f).SetEase(Ease.InOutSine);
        _hp = 5;
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
    }

    public void Hunt()
    {
        _hunted = true;
    }

    public void ApplyScore(int score)
    {
        int damage = _hp - score;
        _hp -= damage;
        if (damage > 0)
        {
            Damage(damage);
        }
    }

    public void Damage(int damage)
    {
        StartCoroutine(DamageProcess(damage));
    }

    IEnumerator DamageProcess(int damage)
    {
        float y = transform.position.y + (_offsetPerDamage * damage);
        bool death = false;
        
        if(y < 0)
        {
            y = 0f;
            death = true;
        }
        transform.DOMoveY(y, _damageSpeed).SetSpeedBased();
        yield return _tile.transform.DOMoveY(y, _damageSpeed).SetSpeedBased().WaitForCompletion();
        if(death)
        {
            _dead = true;
            _tile.gameObject.SetActive(false);
            _nameText.color = Color.red;
            EventBus.OnPlayerDead?.Invoke(this);
        }
    }
}
