using UnityEngine;

public class DestroyObjectAnimationEvent : MonoBehaviour
{
    private void OnAnimationEnd() => Destroy(gameObject);
}