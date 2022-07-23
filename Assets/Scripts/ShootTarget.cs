using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class ShootTarget : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UnityEvent<Collider> _onClick;

    private Collider _collider;

    public void OnPointerClick(PointerEventData eventData) => _onClick.Invoke(_collider);

    private void Start() => _collider = GetComponent<Collider>();

    private void OnCollisionEnter(Collision collision)
    {
        // _collider.attachedRigidbody.isKinematic =
        //     !(!_collider.attachedRigidbody.isKinematic || collision.relativeVelocity.sqrMagnitude > .0001);
    }
}