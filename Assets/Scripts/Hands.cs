﻿using Cinemachine;
using Lib;
using UnityEngine;
using UnityEngine.Assertions;

public class Hands : MonoBehaviour
{
    private static readonly int IsMove = Animator.StringToHash("IsMove");
    private static readonly int CycleOffset = Animator.StringToHash("CycleOffset");

    private const float BaseAttackSpeed = .4f;
    private const float AttackSpeedMultiplier = .1f;
    private const float HitPowerMultiplier = 4f;
    private const float CycleOffsetValue = .1f;
    private const float ForceDistanceMultiplier = 20;
    private const float FrictionPerHitMultiplier = .2f;

    [SerializeField] private Camera _camera;
    [SerializeField] private Canvas _popupRewardCanvas;
    [SerializeField] private CapsuleCollider _hitArea;
    [SerializeField] private Hand _leftHand;
    [SerializeField] private Hand _rightHand;
    [SerializeField] private ParticlePool _particlePool;
    [SerializeField] private GameObject _popupReward;
    [SerializeField] private Transform _forcePoint;
    [SerializeField] private int _attackSpeed = 3;
    [SerializeField] private int _hitPower = 4;

    private Animator _animator;
    private Animator _leftHandAnimator;
    private Animator _rightHandAnimator;
    private CinemachineImpulseSource _impulse;
    private Collider[] _hitBuffer = new Collider[50];
    private Vector3 _nextHitPosition;
    private bool _hasNextHit;
    private float _explosionRadius;
    private Hand _currentActiveHand;
    private int _currentHitCount;
    private int _maxHitCount = 2;

    public int HitPower => _hitPower;

    public void AddHitPower() => _hitPower += 1;

    public int AttackSpeed => _attackSpeed;

    public void AddAttackSpeed()
    {
        _attackSpeed += 1;
        UpdateAttackSpeed();
    }

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


        if (_currentActiveHand == _leftHand)
        {
            _rightHand.PlayHitSupportAnimation();
            _leftHand.HitTo(position);
        }
        else
        {
            _leftHand.PlayHitSupportAnimation();
            _rightHand.HitTo(position);
        }

        _currentHitCount++;

        if (_currentHitCount < _maxHitCount)
            return;

        _currentHitCount = 0;
        _maxHitCount = (int) (1.5f + Random.value);

        _currentActiveHand = _currentActiveHand == _leftHand ? _rightHand : _leftHand;
    }

    public void LaunchHitProcesses(Vector3 hitPosition)
    {
        _particlePool.GetEffect().transform.position = hitPosition;

        var pos = _camera.WorldToScreenPoint(hitPosition);
        Instantiate(_popupReward, pos, Quaternion.identity, _popupRewardCanvas.transform);

        _impulse.GenerateImpulse((hitPosition - transform.position).normalized);

        float totalPower = _hitPower * HitPowerMultiplier;
        _hitArea.transform.position = hitPosition;
        _forcePoint.transform.parent.position = hitPosition;

        var hitCount = _hitArea.OverlapCapsuleNonAlloc(_hitBuffer);

        for (int i = 0; i < hitCount; i++)
        {
            var body = _hitBuffer[i].attachedRigidbody;
            if (!body)
                continue;

            body.isKinematic = false;
            body.mass = 10;
            var collider = body.GetComponent<Collider>();
            body.AddExplosionForce(totalPower - totalPower * collider.material.dynamicFriction * .9f,
                _forcePoint.position, _explosionRadius, 0f, ForceMode.VelocityChange);
            var friction = collider.material.dynamicFriction * FrictionPerHitMultiplier;
            collider.material.dynamicFriction = friction;
            collider.material.staticFriction = friction;
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

    private void UpdateAttackSpeed()
    {
        _rightHandAnimator.speed = BaseAttackSpeed + _attackSpeed * AttackSpeedMultiplier;
        _leftHandAnimator.speed = BaseAttackSpeed + _attackSpeed * AttackSpeedMultiplier;
    }

    private void Start()
    {
        Assert.IsNotNull(_particlePool);
        Assert.IsNotNull(_popupReward);
        Assert.IsNotNull(_popupRewardCanvas);
        Assert.IsNotNull(_leftHand);
        Assert.IsNotNull(_rightHand);

        _hitArea.gameObject.SetActive(false);
        _forcePoint.gameObject.SetActive(false);

        _animator = GetComponent<Animator>();
        _impulse = GetComponent<CinemachineImpulseSource>();

        _leftHandAnimator = _leftHand.GetComponent<Animator>();
        _rightHandAnimator = _rightHand.GetComponent<Animator>();
        _rightHandAnimator.SetFloat(CycleOffset, CycleOffsetValue);

        UpdateAttackSpeed();

        _explosionRadius = _forcePoint.localPosition.magnitude * ForceDistanceMultiplier;
        _currentActiveHand = _rightHand;
        Application.targetFrameRate = 1000;
    }

    private void RunNextHitIfExist()
    {
        if (!_hasNextHit)
            return;

        _hasNextHit = false;
        HitTo(_nextHitPosition);
    }

    private void OnEnable()
    {
        _leftHand._onHitCompleted += RunNextHitIfExist;
        _rightHand._onHitCompleted += RunNextHitIfExist;
    }

    private void OnDisable()
    {
        _leftHand._onHitCompleted -= RunNextHitIfExist;
        _rightHand._onHitCompleted -= RunNextHitIfExist;
    }
}