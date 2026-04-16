using UnityEngine;

public static class PlacementUtils
{
    public static Vector3 ClampToNearest(Vector3 pos, float threshold, Vector3 gridOffset = new Vector3())
    {
        float t = 1f / threshold;
        Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;

        float s = threshold * 0.5f;
        v.x += s + gridOffset.x;
        v.z += s + gridOffset.y;
        return v;
    }
}