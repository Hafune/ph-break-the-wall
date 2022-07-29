using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class PopupReward : MonoBehaviour
{
    private void OnAnimationEnd() => Destroy(gameObject);
}