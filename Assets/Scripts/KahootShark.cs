using Ali.Helper;
using DG.Tweening;
using PathCreation.Examples;
using System.Collections;
using UnityEngine;

public class KahootShark : MonoBehaviour
{
    [SerializeField] private float _jumpDistance = 6f;
    [SerializeField] private float _jumpPower = 2f;
    [SerializeField] private float _jumpDuration = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Vector3 oldPosition = new Vector3(-9999f, 0f, 0f);

    public void Hunt(KahootPlayer player)
    {
        StartCoroutine(HuntProcess(player));
    }

    private void Update()
    {
        if(oldPosition.x < -99f)
        {
            oldPosition = transform.position;
            return;
        }
        Vector3 direction = transform.position - oldPosition;
        if(direction.sqrMagnitude < 0.01f)
        {
            return;
        }
        direction.Normalize();
        transform.forward = direction;
        oldPosition = transform.position;
    }

    IEnumerator HuntProcess(KahootPlayer player)
    {
        transform.DOJump(transform.position + (transform.forward * _jumpDistance), _jumpPower, 1, _jumpDuration);
        yield return new WaitForSeconds(_jumpDuration/2.5f);
        player.Kill();
        yield return new WaitForSeconds(_jumpDuration - (_jumpDuration / 2.5f));
        transform.DOMove(transform.forward * 10f, 2f).SetRelative();
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);

    }
}
