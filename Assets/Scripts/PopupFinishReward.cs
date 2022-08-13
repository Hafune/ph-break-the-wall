using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

[RequireComponent(typeof(Image))]
public class PopupFinishReward : MonoBehaviour
{
    [SerializeField] private RectTransform _topPoint;
    [SerializeField] private RectTransform _horizontalPoint;
    [SerializeField] private RectTransform _verticalOffsetPoint;

    private float _fadeInTime = .5f;
    private float _fadeOutTime = .5f;

    private void Start()
    {
        var middlePosition = new Vector2
        {
            x = new Random().Next((int) _horizontalPoint.localPosition.x * 2) -
                _horizontalPoint.localPosition.x,
            y = _horizontalPoint.localPosition.y + new Random().Next((int) _verticalOffsetPoint.localPosition.y * 2) -
                _verticalOffsetPoint.localPosition.y
        };

        var rect = GetComponent<RectTransform>();
        DOTween.Sequence()
            .Append(rect.DOLocalMove(middlePosition, _fadeInTime))
            .Join(GetComponent<CanvasGroup>().DOFade(1f, _fadeInTime))
            .Append(rect.DOLocalMove(_topPoint.localPosition, _fadeOutTime))
            .Join(GetComponent<CanvasGroup>().DOFade(0f, _fadeOutTime));
    }
}