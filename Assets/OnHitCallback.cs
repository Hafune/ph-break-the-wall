using Lib;
using UnityEngine;
using UnityEngine.Events;

public class OnHitCallback : MonoBehaviour
{
    [SerializeField] private UnityEvent<Vector3> _onHit;
    [SerializeField] private Camera _camera;

    private Brick[] _bricks;
    private float _outOfWallBrickCount;
    private bool _wallIsBroken = false;

    private void Start()
    {
        _bricks = GetComponentsInChildren<Brick>();
        _bricks.ForEach(brick =>
        {
            brick.SetOnHitEvent(_onHit);
            brick.SetCamera(_camera);
            brick.SetOutOfWallEvent(() =>
            {
                _outOfWallBrickCount++;
                CheckBricks();
            });
        });
    }

    private void CheckBricks()
    {
        if (!_wallIsBroken && _outOfWallBrickCount / _bricks.Length > .8)
        {
            _bricks.ForEach(brick => brick.GetComponent<Rigidbody>().isKinematic = false);
            _wallIsBroken = true;
        }
    }
}