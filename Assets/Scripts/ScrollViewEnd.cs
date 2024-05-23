using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScrollViewEndDetector : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentRectTransform;
    [SerializeField] private float endThreshold = 0.1f;

    public UnityEvent onEndReached;

    private void Start()
    {
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void OnScrollValueChanged(Vector2 normalizedPosition)
    {
        float contentWidth = contentRectTransform.rect.width;
        float viewportWidth = scrollRect.GetComponent<RectTransform>().rect.width;

        if (contentWidth - viewportWidth - scrollRect.horizontalNormalizedPosition * (contentWidth - viewportWidth) < endThreshold)
        {
            onEndReached.Invoke();
        }
    }
}