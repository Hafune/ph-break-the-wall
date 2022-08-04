using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int IsMove = Animator.StringToHash("IsMove");
    private static readonly int HitSupport = Animator.StringToHash("HitSupport");

    [SerializeField] private UnityEvent<Vector3> _onHit;

    public Action _onHitCompleted;

    private Animator _animator;
    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    private Transform _pivot;
    private float _defaultDistance;
    private float _maxDistance;
    private bool _isReady = true;

    public bool IsReady => _isReady;

    public void HitTo(Vector3 position)
    {
        if (_isReady)
            StartCoroutine(DealHit(position));
    }

    public void SetMoveAnimation() => _animator.SetBool(IsMove, true);

    public void SetIdleAnimation() => _animator.SetBool(IsMove, false);

    public void PlayHitSupportAnimation() => _animator.SetTrigger(HitSupport);

    private IEnumerator DealHit(Vector3 position)
    {
        _isReady = false;

        var direction = position - _pivot.position;
        float distance = Mathf.Min(_maxDistance - _defaultDistance,direction.magnitude - _defaultDistance);
        var newPosition = _pivot.position + direction.normalized * distance;
        var skipFrame = new WaitForNextFrameUnit();

        _animator.SetTrigger(Hit);

        yield return skipFrame;

        float time = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / _animator.speed;
        float halfTime = time / 2;

        var sequence = DOTween.Sequence();
        sequence.Append(_pivot.DOMove(newPosition, halfTime))
            .Join(_pivot.DOLookAt(position, halfTime).OnComplete(() => _onHit.Invoke(position)))
            .Append(_pivot.DOLocalMove(_defaultPosition, halfTime))
            .Join(_pivot.DORotateQuaternion(_defaultRotation, halfTime));

        sequence.OnComplete(() =>
        {
            _isReady = true;
            _onHitCompleted.Invoke();
        });
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();

        _pivot = transform.parent;
        _defaultPosition = _pivot.localPosition;
        _defaultRotation = _pivot.rotation;

        Physics.Raycast(_pivot.position, _pivot.forward, out RaycastHit info);

        _defaultDistance = info.distance;
        _maxDistance = _defaultDistance * 1.5f;
    }
}