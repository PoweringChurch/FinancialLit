using System;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

[Serializable]
public class UIAnimations : MonoBehaviour
{
    public Transform flag;

    public RawImage bgScrollerImage;
    public Vector2 scrollSpeed = new Vector2(0.1f, 0.1f);

    public Transform corgi;
    public Transform cur;
    public Transform pug;

    public void Update()
    {
        //rotate flag
        float angle = Mathf.Sin(Time.time * 0.5f) * 5f;
        flag.localRotation = Quaternion.Euler(0, 0, angle);

        //move bg scroller
        bgScrollerImage.uvRect = new Rect(
            bgScrollerImage.uvRect.position + scrollSpeed * Time.deltaTime,
            bgScrollerImage.uvRect.size
        );
    }
    float spinDuration = 1.5f;
    public void SpinBreeds() 
    {
        corgi.rotation = Quaternion.Euler(0,90,0);
        cur.rotation = Quaternion.Euler(0,90,0);
        pug.rotation = Quaternion.Euler(0,90,0);

        corgi.localScale = new Vector3(.1f, .1f, .1f);
        cur.localScale = new Vector3(.1f, .1f, .1f);
        pug.localScale = new Vector3(.1f, .1f, .1f);


        Tween.Rotation(corgi, endValue: Quaternion.Euler(0,-180,0),duration:spinDuration, ease: Ease.OutSine);
        Tween.Rotation(cur, endValue: Quaternion.Euler(0,-180,0),duration:spinDuration, ease: Ease.OutSine);
        Tween.Rotation(pug, endValue: Quaternion.Euler(0,-180,0),duration:spinDuration, ease: Ease.OutSine);

        Tween.Scale(corgi, endValue: 1.5f, duration: spinDuration,ease: Ease.OutSine);
        Tween.Scale(cur, endValue: 1.5f, duration: spinDuration,ease: Ease.OutSine);
        Tween.Scale(pug, endValue: 1.5f, duration: spinDuration,ease: Ease.OutSine);
    }
}
