using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaHandler : MonoBehaviour
{
    [Serializable]
    public class AreaData
    {
        public string areaName;
        public GameObject areaPrefab;
    }

    [Header("Area Setup")]
    [SerializeField] private GameObject homeArea;
    [SerializeField] private List<AreaData> areas;
    [SerializeField] private Transform gameSpace;
    
    [Header("References")]
    [SerializeField] private Pet pet;
    
    private GameObject currentArea;
    private Dictionary<string, GameObject> areaDict;

    private void Awake()
    {
        // Build dictionary for easy lookup
        areaDict = new Dictionary<string, GameObject>();
        foreach (var area in areas)
        {
            if (area.areaPrefab != null)
            {
                areaDict[area.areaName] = area.areaPrefab;
            }
        }
    }

    private void Start()
    {
        if (homeArea) homeArea.SetActive(true);
    }

    public void EnterArea(string areaName)
    {
        if (!areaDict.TryGetValue(areaName, out GameObject areaPrefab))
        {
            Debug.LogError($"Area '{areaName}' not found!");
            return;
        }

        CleanupCurrentArea();

        if (homeArea && homeArea.activeSelf)
        {
            homeArea.SetActive(false);
        }
        
        currentArea = Instantiate(areaPrefab, gameSpace);
        PlayerStateHandler.Instance.SetState(PlayerState.View);
        CameraHandler.Instance.RefreshWallRenderers();
        ButtonToggler.Instance.DisableButton("Build");
    }
    public void EnterHome()
    {
        CleanupCurrentArea();
        
        if (homeArea)
        {
            homeArea.SetActive(true);
        }

        PlayerStateHandler.Instance.SetState(PlayerState.Home);
        ButtonToggler.Instance.EnableButton("Build");
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