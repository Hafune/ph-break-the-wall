using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class PopupReward : MonoBehaviour
{
    private void Start()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        var rectTransform = GetComponent<RectTransform>();

        var sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOMoveY(rectTransform.position.y + 200, 1f));
        sequence.Insert(0, canvasGroup.DOFade(0, 1));
        sequence.OnComplete(() => Destroy(gameObject));
    }
}