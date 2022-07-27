using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    private static readonly int Hit = Animator.StringToHash("Hit");

    [SerializeField] private UnityEvent<Vector3> _onHit;
    [SerializeField] private float _hitPower = 50f;

    private Animator _animator;
    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    private Transform _parentTransform;
    private float _defaultDistance;
    private bool _isReady = true;
    private Collider[] _hitBuffer = new Collider[20];

    public bool IsReady => _isReady;

    public Vector3 Position => _animator.transform.position;

    public void HitTo(Vector3 position)
    {
        if (_isReady)
            StartCoroutine(DealHit(position));
    }

    private IEnumerator DealHit(Vector3 position)
    {
        _isReady = false;

        var direction = position - _parentTransform.position;
        float distance = direction.magnitude - _defaultDistance;
        var newPosition = _parentTransform.position + direction.normalized * distance;
        var skipFrame = new WaitForNextFrameUnit();

        _animator.SetTrigger(Hit);

        yield return skipFrame;

        float time = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        float halfTime = time / 2;

        var sequence = DOTween.Sequence();
        sequence.Append(_parentTransform.DOMove(newPosition, halfTime)
                .OnComplete(() =>
                {
                    float totalPower = _hitPower * Time.deltaTime * .3f;

                    int count = Physics.OverlapSphereNonAlloc(position, 2, _hitBuffer);

                    for (int i = 0; i < count; i++)
                    {
                        if (!_hitBuffer[i].attachedRigidbody)
                            continue;

                        _hitBuffer[i].attachedRigidbody.isKinematic = false;
                        _hitBuffer[i].attachedRigidbody
                            .AddExplosionForce(totalPower, position, 2);
                    }

                    _onHit.Invoke(position);
                }))
            .Append(_parentTransform.DOLocalMove(_defaultPosition, halfTime));

        sequence.Insert(0, _parentTransform.DOLookAt(position, halfTime))
            .Append(_parentTransform.DORotateQuaternion(_defaultRotation, halfTime));

        sequence.OnComplete(() => _isReady = true);
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();

        _parentTransform = transform.parent;
        _defaultPosition = _parentTransform.localPosition;
        _defaultRotation = _parentTransform.rotation;

        Physics.Raycast(_parentTransform.position, _parentTransform.forward, out RaycastHit info);

        _defaultDistance = info.distance;
    }
}