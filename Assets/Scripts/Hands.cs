using Cinemachine;
using Lib;
using UnityEngine;
using UnityEngine.Assertions;

public class Hands : MonoBehaviour
{
    private static readonly int IsMove = Animator.StringToHash("IsMove");

    [SerializeField] private Camera _camera;
    [SerializeField] private Canvas _popupRewardCanvas;
    [SerializeField] private CapsuleCollider _hitArea;
    [SerializeField] private Hand _leftHand;
    [SerializeField] private Hand _rightHand;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private PopupReward _popupReward;
    [SerializeField] private Transform _forcePoint;
    [SerializeField] private int _attackSpeed = 3;
    [SerializeField] private int _hitPower = 4;

    private Animator _animator;
    private CinemachineImpulseSource _impulse;
    private Collider[] _hitBuffer = new Collider[50];
    private Vector3 _nextHitPosition;
    private bool _hasNextHit;
    private float _hitPowerMultyplier = 10;

    public int HitPower => _hitPower;
    
    public void AddHitPower() => _hitPower +=1;
    
    public int AttackSpeed => _attackSpeed;
    
    public void AddAttackSpeed() => _attackSpeed +=1;

    public void HitTo(Vector3 position)
    {
        if (_animator.GetBool(IsMove))
            return;

        if (!_leftHand.IsReady || !_rightHand.IsReady)
        {
            _hasNextHit = true;
            _nextHitPosition = position;
            return;
        }

        var leftDistance = (position - _leftHand.Position).sqrMagnitude;
        var rightDistance = (position - _rightHand.Position).sqrMagnitude;

        if (leftDistance < rightDistance)
        {
            _rightHand.PlayHitSupportAnimation();
            _leftHand.HitTo(position);
        }
        else
        {
            _leftHand.PlayHitSupportAnimation();
            _rightHand.HitTo(position);
        }
    }

    public void LaunchHitProcesses(Vector3 hitPosition)
    {
        Instantiate(_hitEffect).transform.position = hitPosition;

        var pos = _camera.WorldToScreenPoint(hitPosition);
        Instantiate(_popupReward, pos, Quaternion.identity, _popupRewardCanvas.transform);

        _impulse.GenerateImpulse((hitPosition - transform.position).normalized / 2);

        float totalPower = _hitPower * _hitPowerMultyplier;
        _hitArea.transform.position = hitPosition;
        _forcePoint.transform.parent.position = hitPosition;

        var count = _hitArea.OverlapCapsuleNonAlloc(_hitBuffer);

        for (int i = 0; i < count; i++)
        {
            var body = _hitBuffer[i].attachedRigidbody;
            if (!body)
                continue;

            body.isKinematic = false;

            var direction = (body.position - _forcePoint.position).normalized;
            body.AddForce(direction * totalPower, ForceMode.VelocityChange);
        }
    }

    public void SetMoveAnimation()
    {
        _leftHand.SetMoveAnimation();
        _rightHand.SetMoveAnimation();
        _animator.SetBool(IsMove, true);
    }

    public void SetIdleAnimation()
    {
        _leftHand.SetIdleAnimation();
        _rightHand.SetIdleAnimation();
        _animator.SetBool(IsMove, false);
    }

    private void Start()
    {
        _hitArea.gameObject.SetActive(false);
        _forcePoint.gameObject.SetActive(false);

        _animator = GetComponent<Animator>();
        _impulse = GetComponent<CinemachineImpulseSource>();

        Assert.IsNotNull(_hitEffect);
        Assert.IsNotNull(_popupReward);
        Assert.IsNotNull(_popupRewardCanvas);
        Assert.IsNotNull(_leftHand);
        Assert.IsNotNull(_rightHand);

        Application.targetFrameRate = 1000;
    }

    private void CheckNextHit()
    {
        if (!_hasNextHit)
            return;

        _hasNextHit = false;
        HitTo(_nextHitPosition);
    }

    private void OnEnable()
    {
        _leftHand._onHitCompleted += CheckNextHit;
        _rightHand._onHitCompleted += CheckNextHit;
    }

    private void OnDisable()
    {
        _leftHand._onHitCompleted -= CheckNextHit;
        _rightHand._onHitCompleted -= CheckNextHit;
    }
}