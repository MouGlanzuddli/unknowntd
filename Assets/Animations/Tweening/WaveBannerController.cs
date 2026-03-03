using UnityEngine;
using DG.Tweening; // dotween library

public class WaveBannerController : MonoBehaviour
{
    private RectTransform rectTransform;

    
    public Vector2 hiddenPos = new Vector2(0, 600);
    public Vector2 visiblePos = new Vector2(0, 400);

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // set anchorPos = hiddenPos
        rectTransform.anchoredPosition = hiddenPos;
    }

    [ContextMenu("Test Drop")] 
    public void PlayWaveAnimation()
    {
        // Reset position
        rectTransform.anchoredPosition = hiddenPos;

        
        Sequence s = DOTween.Sequence();

        s.Append(rectTransform.DOAnchorPos(visiblePos, 0.8f).SetEase(Ease.OutBack)) // bounce when falling down
         .AppendInterval(2f) // stop 2 seconds
         .Append(rectTransform.DOAnchorPos(hiddenPos, 0.5f).SetEase(Ease.InBack)); // back
    }
}