using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class MoveToNextWall : MonoBehaviour
{
    [SerializeField] private UnityEvent _onWallBreak;
    [SerializeField] private Hands _hands;
    [SerializeField] private Wall _wall;
    [SerializeField] private Wall _nextWall;

    private Vector3 _positionFromWall;

    public void BeginMovement() => StartCoroutine(BeginMovementAnimation());

    private IEnumerator BeginMovementAnimation()
    {
        _onWallBreak.Invoke();
        yield return new WaitForSeconds(1f);
        _hands.SetMoveAnimation();
        transform.DOMove(_nextWall.transform.position - _positionFromWall, 3f)
            .OnComplete(() => _hands.SetIdleAnimation());
    }

    private void Start() => _positionFromWall = _wall.transform.position - transform.position;
}