using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class Brick : MonoBehaviour, IPointerDownHandler
{
    private UnityEvent<Collider> _onHit;
    private Action _onOutOfWall;
    private Collider _collider;
    private Vector3 _basePosition;
    private bool _inWall = true;

    public void SetOnHitEvent(UnityEvent<Collider> onHit) => _onHit = onHit;

    public void SetOutOfWallEvent(Action onOutOfWall) => _onOutOfWall = onOutOfWall;

    public void OnPointerDown(PointerEventData eventData) => _onHit.Invoke(_collider);

    private void Start()
    {
        _basePosition = transform.position;
        _collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_inWall)
            return;

        _inWall = (_basePosition - transform.position).magnitude > 2;

        if (!_inWall)
            _onOutOfWall.Invoke();
    }
}