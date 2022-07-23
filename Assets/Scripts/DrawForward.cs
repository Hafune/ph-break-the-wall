using UnityEngine;

public class DrawForward : MonoBehaviour
{
    private void Update()
    {
        Debug.DrawRay(transform.position,transform.forward * 100);
    }
}