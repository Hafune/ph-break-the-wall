using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class MoveToNextWall : MonoBehaviour
{
    [SerializeField] private UnityEvent _onWallBreak;
    [SerializeField] private Hands _hands;
    [SerializeField] private Transform _currentPosition;
    [SerializeField] private Transform _nextPosition;

    private Vector3 _positionFromWall;

    public void BeginMovement() => StartCoroutine(BeginMovementAnimation());

    private IEnumerator BeginMovementAnimation()
    {
        _onWallBreak.Invoke();
        yield return new WaitForSeconds(1.5f);
        _hands.SetMoveAnimation();
        transform.DOMove(_nextPosition.transform.position - _positionFromWall, 3f)
            .OnComplete(() => _hands.SetIdleAnimation());
    }

    private void Start() => _positionFromWall = _currentPosition.transform.position - transform.position;
}