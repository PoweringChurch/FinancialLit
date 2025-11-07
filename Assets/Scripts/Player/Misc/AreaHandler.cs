using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaHandler : MonoBehaviour
{
    [Serializable]
    public class AreaData
    {
        public string areaName;
        public bool isShop;
        public GameObject prefab;
    }
    [Header("Area Setup")]
    [SerializeField] private GameObject home;
    [SerializeField] private AreaData[] areas;
    [SerializeField] private Transform gameSpace;

    private Dictionary<string, AreaData> areaDict = new();
    
    [Header("References")]
    [SerializeField] private PetStats pet;

    private GameObject currentArea;
    private void Awake()
    {
        foreach (var area in areas)
        {
            areaDict[area.areaName] = area;
        }
    }
    private void Start()
    {
        if (home) home.SetActive(true);
    }

    public void EnterArea(string areaName)
    {
        AreaData area;
        if (!areaDict.TryGetValue(areaName, out area))
        {
            Debug.LogError($"Area '{areaName}' not found!");
            return;
        }

        CleanupCurrentArea();
        if (home && home.activeSelf)
        {
            home.SetActive(false);
        }

        currentArea = Instantiate(area.prefab, gameSpace);
        PlayerStateManager.RemoveState(PlayerState.Home);
        if (area.isShop) PlayerStateManager.AddState(PlayerState.Shopping);
        CameraHandler.Instance.RefreshWallRenderers();
        UIHandler.Instance.ButtonManager.DisableButton("Build");
    }
    public void EnterHome()
    {
        CleanupCurrentArea();
        
        if (home)
        {
            home.SetActive(true);
        }

        PlayerStateManager.AddState(PlayerState.Home);
        PlayerStateManager.RemoveState(PlayerState.Shopping);

        CameraHandler.Instance.RefreshWallRenderers();
        UIHandler.Instance.ButtonManager.EnableButton("Build");
    }

    private void CleanupCurrentArea()
    {
        if (currentArea != null)
        {
            Destroy(currentArea);
            currentArea = null;
        }
    }

    private void OnDestroy()
    {
        CleanupCurrentArea();
    }
}
