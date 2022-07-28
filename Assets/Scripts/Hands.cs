using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Camera))]
public class Hands : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private CinemachineShake _cinemachineShake;
    [SerializeField] private PopupReward _popupReward;
    [SerializeField] private Canvas _popupRewardCanvas;
    [SerializeField] private Hand _leftHand;
    [SerializeField] private Hand _rightHand;
    [SerializeField] private int _hitPower = 1;

    private Collider[] _hitBuffer = new Collider[20];
    private Camera _camera;

    public void HitTo(Vector3 position)
    {
        var leftDistance = (position - _leftHand.Position).sqrMagnitude;
        var rightDistance = (position - _rightHand.Position).sqrMagnitude;

        if (leftDistance < rightDistance)
        {
            if (_leftHand.IsReady)
                _leftHand.HitTo(position);
            else if (_rightHand.IsReady)
                _rightHand.HitTo(position);
        }
        else
        {
            if (_rightHand.IsReady)
                _rightHand.HitTo(position);
            else if (_leftHand.IsReady)
                _leftHand.HitTo(position);
        }
    }

    public void LaunchHitProcesses(Vector3 hitPosition)
    {
        _cinemachineShake.Shake();
        Instantiate(_hitEffect).transform.position = hitPosition;

        var pos = _camera.WorldToScreenPoint(hitPosition);
        Instantiate(_popupReward, pos, Quaternion.identity, _popupRewardCanvas.transform);

        float totalPower = _hitPower;

        int count = Physics.OverlapSphereNonAlloc(hitPosition, 2, _hitBuffer);

        for (int i = 0; i < count; i++)
        {
            if (!_hitBuffer[i].attachedRigidbody)
                continue;

            _hitBuffer[i].attachedRigidbody.isKinematic = false;
            _hitBuffer[i].attachedRigidbody
                .AddExplosionForce(totalPower, hitPosition, 3, 0, ForceMode.VelocityChange);
        }
    }

    public void SetMoveAnimation()
    {
    }

    private void Start()
    {
        _camera = GetComponent<Camera>();

        Assert.IsNotNull(_hitEffect);
        Assert.IsNotNull(_cinemachineShake);
        Assert.IsNotNull(_popupReward);
        Assert.IsNotNull(_popupRewardCanvas);
        Assert.IsNotNull(_leftHand);
        Assert.IsNotNull(_rightHand);

        Application.targetFrameRate = 1000;
    }
}