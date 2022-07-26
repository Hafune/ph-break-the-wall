using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Hands : MonoBehaviour
{
    [SerializeField] private Hand _leftHand;
    [SerializeField] private Hand _rightHand;

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

    private void Start()
    {
        Assert.IsNotNull(_leftHand);
        Assert.IsNotNull(_rightHand);

        Application.targetFrameRate = 1000;
    }
}