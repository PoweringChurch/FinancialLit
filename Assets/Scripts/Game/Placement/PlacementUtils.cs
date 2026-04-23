using System.Collections;
using PrimeTween;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public static class PlacementUtils
{
    private static GameObject floatingTextPrefab;
    public static void Init() =>
        floatingTextPrefab = Resources.Load<GameObject>("UITemplates/Message");
    public static Vector3 ClampToNearest(Vector3 pos, float threshold, Vector3 gridOffset = new Vector3())
    {
        float t = 1f / threshold;
        Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;

        float s = threshold * 0.5f;
        v.x += s + gridOffset.x;
        v.z += s + gridOffset.y;
        return v;
    }
    // show message popup; different from an info popup
    public static void Message(string message, Vector3? at = null, Color? startColor = null)
    {
        var (location, overInteractableLayer) = (at != null) ? // if there is a set position
            (at, true) : // use the set position
            CursorUtils.CursorToVector3(1); // otherwise dont
        // create object from prefab
        GameObject textObj = Object.Instantiate(floatingTextPrefab, location.Value - (Camera.main.transform.forward*2), Camera.main.transform.rotation);
        TextMeshPro tmp = textObj.GetComponent<TextMeshPro>();
        // set message and color
        tmp.text = message;
        if (startColor.HasValue)
            tmp.color = startColor.Value;
        // move up and fade message
        const float tweenDur = 1.5f;

        Vector3 goalPos = new Vector3(0,5,0) + textObj.transform.position;
        Tween.Position(textObj.transform, endValue: goalPos, duration: tweenDur, ease: Ease.InSine);
        Tween.Color(tmp, endValue: new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0f), duration: tweenDur)
            .OnComplete(() => Object.Destroy(textObj));
        // delete after 1.5 sec
    }
}