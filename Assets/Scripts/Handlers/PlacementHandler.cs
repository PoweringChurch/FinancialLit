using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlacementMode
{
    Fixed,
    Valid,
    Invalid
}
//https://github.com/MinaPecheux/unity-tutorials/blob/main/Assets/07-BuildingPlacement/Scripts/BuildingManager.cs
//modified a bit but mostly from this tutorial
public class PlacementHandler : MonoBehaviour
{
    public string itemName;
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;

    public MeshRenderer[] meshComponents;
    private Dictionary<MeshRenderer, List<Material>> initialMaterials;

    [HideInInspector] public bool hasValidPlacement;
    [HideInInspector] public bool isFixed;
    private int _nObstacles;
    private void Awake()
    {
        hasValidPlacement = true;
        isFixed = false;
        _nObstacles = 0;
        _InitializeMaterials();
    }
    void OnTriggerEnter(Collider other)
    {
        if (isFixed) return;
        if (IsPlacementOrRoomLayer(other.gameObject)) { return; }
        _nObstacles++;
        SetPlacementMode(PlacementMode.Invalid);
    }
    void OnTriggerExit(Collider other)
    {
        if (isFixed) return;
        if (IsPlacementOrRoomLayer(other.gameObject)) { return; }
        _nObstacles--;
        if (_nObstacles <= 0)
            SetPlacementMode(PlacementMode.Valid);
    }
    //no idea what this does vv
#if UNITY_EDITOR
    private void OnValidate()
    {
        _InitializeMaterials();
    }
#endif
    public void SetPlacementMode(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            isFixed = true;
            hasValidPlacement = true;
        }
        else if (mode == PlacementMode.Valid)
        {
            hasValidPlacement = true;
        }
        else
        {
            hasValidPlacement = false;
        }
        SetMaterial(mode);
    }

    public void SetMaterial(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            foreach (MeshRenderer r in meshComponents)
                r.sharedMaterials = initialMaterials[r].ToArray();
        }
        else
        {
            Material matToApply = mode == PlacementMode.Valid
                ? validPlacementMaterial : invalidPlacementMaterial;

            Material[] m; int nMaterials;
            foreach (MeshRenderer r in meshComponents)
            {
                nMaterials = initialMaterials[r].Count;
                m = new Material[nMaterials];
                for (int i = 0; i < nMaterials; i++)
                    m[i] = matToApply;
                r.sharedMaterials = m;
            }
        }
    }

    private void _InitializeMaterials()
    {
        if (initialMaterials == null)
            initialMaterials = new Dictionary<MeshRenderer, List<Material>>();
        if (initialMaterials.Count > 0)
        {
            foreach (var l in initialMaterials) l.Value.Clear();
            initialMaterials.Clear();
        }

        foreach (MeshRenderer r in meshComponents)
        {
            initialMaterials[r] = new List<Material>(r.sharedMaterials);
        }
    }
    private bool IsPlacementOrRoomLayer(GameObject o)
    {
        int combinedMask = FurniturePlacer.Instance.groundLayerMask.value | FurniturePlacer.Instance.roomLayerMask.value;
        return ((1 << o.layer) & combinedMask) != 0;
    }
}
