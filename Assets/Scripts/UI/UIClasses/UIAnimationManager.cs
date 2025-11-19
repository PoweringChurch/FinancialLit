using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIAnimationManager
{
    public Transform flag;

    public RawImage bgScrollerImage;
    public Vector2 scrollSpeed = new Vector2(0.1f, 0.1f);
    public void UpdateUI()
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
}
