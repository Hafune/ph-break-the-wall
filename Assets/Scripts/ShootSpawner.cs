using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class ShootSpawner : MonoBehaviour
{
    private static readonly int Hit = Animator.StringToHash("Hit");

    [SerializeField] private Animator _leftHand;
    [SerializeField] private Animator _rightHand;

    private float _hitPower = 20f;
    private Vector3 _defaultHandPosition;
    private float _defaultDistance;

    public void ShootTo(Collider box)
    {
        box.attachedRigidbody.isKinematic = false;
        box.attachedRigidbody.velocity = transform.forward * _hitPower;

        var direction = box.transform.position - _defaultHandPosition;
        float distance = direction.magnitude - _defaultDistance;

        // _leftHand.transform.parent.position = _defaultHandPosition + direction.normalized * dif;

        _leftHand.transform.parent.LookAt(box.transform);
        _leftHand.SetTrigger(Hit);

        StartCoroutine(Play(_defaultHandPosition + direction.normalized * distance, distance));
    }

    private IEnumerator Play(Vector3 newPosition, float dis)
    {
        var wait = new WaitForNextFrameUnit();
        yield return wait;
        var v = _leftHand.GetCurrentAnimatorClipInfo(0);

        var time = v[0].clip.length;
        Debug.Log(time);
        float total = 0f;

        bool forward = true;
        bool complete = false;

        while (!complete)
        {
            total += Time.deltaTime;
            float t = dis * Time.deltaTime / (time / 2);

            forward = total < time / 2;
            Debug.Log(forward);
            if (forward)
            {
                _leftHand.transform.parent.position =
                    Vector3.MoveTowards(_leftHand.transform.parent.position, newPosition, t);
            }
            else
            {
                _leftHand.transform.parent.position =
                    Vector3.MoveTowards(_leftHand.transform.parent.position, _defaultHandPosition, t);
            }

            if (_leftHand.transform.parent.position == _defaultHandPosition)
                complete = true;
            yield return wait;
        }
    }

    private void Start()
    {
        _defaultHandPosition = _leftHand.transform.parent.position;
        Physics.Raycast(_leftHand.transform.parent.position, _leftHand.transform.parent.forward, out RaycastHit info);

        _defaultDistance = info.distance;
    }
}