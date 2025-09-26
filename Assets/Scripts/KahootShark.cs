using Ali.Helper;
using DG.Tweening;
using PathCreation.Examples;
using System.Collections;
using UnityEngine;

public class KahootShark : MonoBehaviour
{
    [SerializeField] private float _directionDistance = 6f;
    [SerializeField] private float _huntSpeed = 3f;
    [Space]
    [SerializeField] private PathFollower _pathFollower;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void Hunt(KahootPlayer player)
    {
        player.Hunt();
        StartCoroutine(HuntProcess(player));
    }

    IEnumerator HuntProcess(KahootPlayer player)
    {
        _pathFollower.enabled = false;
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();
        transform.forward = direction;
        yield return transform.DOMove((direction * _directionDistance) / 2, _huntSpeed).SetSpeedBased().SetRelative().WaitForCompletion();
        player.Kill();
        yield return transform.DOMove((direction * _directionDistance) / 2, _huntSpeed).SetSpeedBased().SetRelative().WaitForCompletion();
        transform.forward = -direction;
        yield return transform.DOMove(-direction * _directionDistance, _huntSpeed).SetSpeedBased().SetRelative().WaitForCompletion();
        _pathFollower.enabled = true;
    }
}
