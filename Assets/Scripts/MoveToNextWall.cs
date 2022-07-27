using DG.Tweening;
using UnityEngine;

public class MoveToNextWall : MonoBehaviour
{
    [SerializeField] private Hands _hands;
    [SerializeField] private Wall _wall;
    [SerializeField] private Wall _nextWall;

    private Vector3 _positionFromWall;

    public void BeginMovement()
    {
        _hands.SetMoveAnimation();
        transform.DOMove(_nextWall.transform.position - _positionFromWall, 3f);
    }

    private void Start() => _positionFromWall = _wall.transform.position - transform.position;
}