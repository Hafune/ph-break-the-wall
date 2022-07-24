using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private static readonly int Hit = Animator.StringToHash("Hit");

    [SerializeField] private Animator _hand;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private float _hitPower = 50f;

    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    private float _defaultDistance;
    private Transform _parent;
    private bool _isReady = true;

    public bool IsReady => _isReady;

    public Vector3 Position => _hand.transform.position;

    public void HitTo(Vector3 position)
    {
        if (_isReady)
            StartCoroutine(DealHit(position));
    }

    private IEnumerator DealHit(Vector3 position)
    {
        _isReady = false;

        var direction = position - _defaultPosition;
        float distance = direction.magnitude - _defaultDistance;
        var newPosition = _defaultPosition + direction.normalized * distance;
        var skipFrame = new WaitForNextFrameUnit();

        _hand.SetTrigger(Hit);

        yield return skipFrame;

        float time = _hand.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        float halfTime = time / 2;

        var arr = new Collider[20];

        var sequence = DOTween.Sequence();
        sequence.Append(_parent.DOMove(newPosition, halfTime)
                .OnComplete(() =>
                {
                    float totalPower = _hitPower * Time.deltaTime * .3f;
                    var explosionPosition = position;

                    int count = Physics.OverlapSphereNonAlloc(position, 2, arr);
                    _hitEffect.Play();
                    _hitEffect.transform.position = explosionPosition;

                    for (int i = 0; i < count; i++)
                    {
                        if (arr[i].attachedRigidbody)
                        {
                            arr[i].attachedRigidbody
                                .AddExplosionForce(totalPower, explosionPosition, 2);
                            arr[i].attachedRigidbody.isKinematic = false;
                        }
                    }
                }))
            .Append(_parent.DOMove(_defaultPosition, halfTime));

        sequence.Insert(0, _parent.DOLookAt(position, halfTime))
            .Append(_parent.DORotateQuaternion(_defaultRotation, halfTime));

        sequence.OnComplete(() => _isReady = true);
    }

    private void Start()
    {
        _parent = transform.parent;
        _defaultPosition = _parent.position;
        _defaultRotation = _parent.rotation;

        Physics.Raycast(_parent.position, _parent.forward, out RaycastHit info);

        _defaultDistance = info.distance;
    }
}