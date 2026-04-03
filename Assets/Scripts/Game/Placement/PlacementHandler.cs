using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    // really should be a field and have property 
    public string itemName;
    
    private Material validPlacementMaterial;
    private Material invalidPlacementMaterial;
    // to put the materials on
    public MeshRenderer[] meshComponents;
    // so that we can set our materials to their originals when placing
    private Dictionary<MeshRenderer, List<Material>> initialMaterials;

    [HideInInspector] public bool hasValidPlacement = true;
    [HideInInspector] public bool isFixed = true;
    private int _nObstacles = 0;

    private void Awake()
    {
        validPlacementMaterial = Resources.Load<Material>("Materials/Furniture/ValidPlacement");
        invalidPlacementMaterial = Resources.Load<Material>("Materials/Furniture/InvalidPlacement");
        _InitializeMaterials();
    }
    void OnTriggerEnter(Collider other)
    {
        // if the object is placed, return
        if (isFixed) return;
        if (IsIgnored(other.gameObject)) return;
        // increment the number of obstacles by 1
        _nObstacles++;
        /* if (UISave.Instance.debugToggle.isOn) 
            { print("debug mode is enabled"); return; }*/
        SetPlacementMode(PlacementMode.Invalid);
    }
    void OnTriggerExit(Collider other)
    {
        // if the object is placed, return
        if (isFixed) return;
        if (IsIgnored(other.gameObject)) return;

        // decrease the number of obstacles by 1
        _nObstacles--;
        if (_nObstacles <= 0)
            SetPlacementMode(PlacementMode.Valid);
    }
    // try to init materials in the editor
#if UNITY_EDITOR
    private void OnValidate()
    {
        _InitializeMaterials();
    }
#endif
    // sets this furniture's placement mode to the provided mode
    public void SetPlacementMode(PlacementMode mode)
    {
        bool hasNavMesh = GetComponent<NavMeshObstacle>() != null;
        if (mode == PlacementMode.Fixed)
        {
            isFixed = true;
            hasValidPlacement = true;
            if (hasNavMesh)
                GetComponent<NavMeshObstacle>().enabled = true;
        }
        else if (mode == PlacementMode.Valid)
        {
            hasValidPlacement = true;
            if (hasNavMesh)
                GetComponent<NavMeshObstacle>().enabled = false;
        }
        else // mode == invalid
        {
            hasValidPlacement = false;
            if (hasNavMesh)
                GetComponent<NavMeshObstacle>().enabled = false;
        }
        SetMaterial(mode);
    }
    // sets the material of this object to the passed mode's associated material
    public void SetMaterial(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            // loop through meshes and
            foreach (MeshRenderer r in meshComponents)
                // apply the furnitures material
                r.sharedMaterials = initialMaterials[r].ToArray();
        }
        else
        {
            // determine what material to apply
            Material matToApply = mode == PlacementMode.Valid
                ? validPlacementMaterial : invalidPlacementMaterial;
                
            Material[] m; int nMaterials;
            // loop through materials and 
            foreach (MeshRenderer r in meshComponents)
            {
                nMaterials = initialMaterials[r].Count;
                m = new Material[nMaterials];
                // apply the mode's associated material
                for (int i = 0; i < nMaterials; i++)
                    m[i] = matToApply;
                r.sharedMaterials = m;
            }
        }
    }
    // init
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
            initialMaterials[r] = new List<Material>(r.sharedMaterials);
    }
    private bool IsIgnored(GameObject o)
    {
        int combinedMask = FurniturePlacer.Instance.groundLayerMask.value | FurniturePlacer.Instance.placementLayerMask.value;
        return ((1 << o.layer) & combinedMask) != 0;
    }
}
