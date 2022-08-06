using System.Collections;
using System.Collections.Generic;
using Lib;
using UnityEngine;

public class JellyTank : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        GetComponentsInChildren<HingeJoint>().ForEach(joint =>
        {
            // var n = joint.transform.InverseTransformDirection(Quaternion.Euler(-90, 0, 0) * joint.transform.localPosition.Copy(x: 0f));
            var n = joint.transform.InverseTransformDirection(joint.transform.localPosition.Copy(x: 0f));
            joint.axis = n;
        });
    }

    // Update is called once per frame
    void Update()
    {
    }
}