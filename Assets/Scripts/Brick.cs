using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class Brick : MonoBehaviour, IPointerDownHandler
{
    private UnityEvent<Vector3> _onHit;
    private Action _onOutOfWall;
    private Camera _camera;
    private Vector3 _basePosition;
    private bool _inWall = true;
    private float _outOfWallDistance;

    public void SetOnHitEvent(UnityEvent<Vector3> onHit) => _onHit = onHit;

    public void SetCamera(Camera camera) => _camera = camera;

    public void SetOutOfWallEvent(Action onOutOfWall) => _onOutOfWall = onOutOfWall;

    public void OnPointerDown(PointerEventData eventData)
    {
        var ray = _camera.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit info))
            _onHit.Invoke(info.point);
    }

    private void Start()
    {
        _basePosition = transform.position;
        _outOfWallDistance = GetComponent<Collider>().bounds.size.sqrMagnitude;
    }

    private void OnCollisionEnter()
    {
        if (!_inWall)
            return;

        _inWall = (_basePosition - transform.position).sqrMagnitude > _outOfWallDistance;

        if (!_inWall)
            _onOutOfWall.Invoke();
    }
}