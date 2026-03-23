// grow on hover
public class GrowOnHover : UIAnimation
{
    public void OnPointerEnter(PointerEventData eventData) {
        Tween.Scale(transform, endValue: 1.1f, duration: inDuration,ease: Ease.InSine);
        // Hover started
    }
    public void OnPointerExit(PointerEventData eventData) {
        Tween.Scale(transform, endValue: 1f, duration: outDuration, ease: Ease.OutSine);
        // Hover ended
    }
}