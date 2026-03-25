using UnityEngine;
using UnityEngine.EventSystems;
using PrimeTween;
// grow on hover
public class GrowOnHover : UIAnimation
{
    // Hover started
    public void OnPointerEnter(PointerEventData eventData) {
        Tween.Scale(transform, endValue: 1.5f, duration: inDuration, ease: Ease.InSine);
        print("entered "+gameObject.name);
    }
    // Hover ended
    public void OnPointerExit(PointerEventData eventData) {
        Tween.Scale(transform, endValue: 1f, duration: outDuration, ease: Ease.OutSine);
        print("exited "+gameObject.name);
    }
}