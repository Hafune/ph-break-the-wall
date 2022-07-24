using UnityEngine;
using UnityEngine.Assertions;

public class Hands : MonoBehaviour
{
    [SerializeField] private Hand _leftHand;
    [SerializeField] private Hand _rightHand;

    public void HitTo(Collider box)
    {
        var leftDistance = (box.transform.position - _leftHand.Position).sqrMagnitude;
        var rightDistance = (box.transform.position - _rightHand.Position).sqrMagnitude;

        if (leftDistance < rightDistance)
        {
            if (_leftHand.IsReady)
                _leftHand.HitTo(box);
            else if (_rightHand.IsReady)
                _rightHand.HitTo(box);
        }
        else
        {
            if (_rightHand.IsReady)
                _rightHand.HitTo(box);
            else if (_leftHand.IsReady)
                _leftHand.HitTo(box);
        }
    }

    private void Start()
    {
        Assert.IsNotNull(_leftHand);
        Assert.IsNotNull(_rightHand);
    }
}