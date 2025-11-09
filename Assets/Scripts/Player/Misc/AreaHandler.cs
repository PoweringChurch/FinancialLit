using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaHandler : MonoBehaviour
{
    [Serializable]
    public class AreaData
    {
        public string areaName;
        public bool shadows;
        public bool bringPet;
        public bool isShop;
        public GameObject prefab;
    }

    [Header("Area Setup")]
    [SerializeField] private GameObject home;
    [SerializeField] private AreaData[] areas;
    [SerializeField] private Transform gameSpace;
    [SerializeField] private Light lighting;
    [SerializeField] private Renderer petRenderer;
    [SerializeField] private PetFunctionality petFunctionality;

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
        if (home && home.activeSelf)
        {
            home.SetActive(false);
        }
        CleanupCurrentArea();
        PlayerStateManager.RemoveState(PlayerState.Home);

        PetStateManager.RemoveState(PetState.Playing);
        PetStateManager.RemoveState(PetState.Sitting);
        PetStateManager.RemoveState(PetState.Sleeping);
        PetBehaviour.Instance.activeBehaviour = Behaviour.Roaming;

        currentArea = Instantiate(area.prefab, gameSpace);
        CameraHandler.Instance.RefreshRenderers();
        

        lighting.shadows = LightShadows.None;
        petRenderer.enabled = false;
        petFunctionality.enabled = false;

        if (area.isShop) PlayerStateManager.AddState(PlayerState.Shopping);
        if (area.shadows) lighting.shadows = LightShadows.Soft;
        if (area.bringPet) {
            petRenderer.enabled = true;
            petFunctionality.enabled = true;
            petFunctionality.transform.position = new Vector3(0, 1, 0);
            }

        UIHandler.Instance.ButtonManager.DisableButton("Build");
    }
    public void EnterHome()
    {
        CleanupCurrentArea();

        if (home)
        {
            home.SetActive(true);
        }
        
        petRenderer.enabled = true;
        petFunctionality.enabled = true;
        petFunctionality.transform.position = new Vector3(0, 1, 0);
        PlayerStateManager.AddState(PlayerState.Home);
        PlayerStateManager.RemoveState(PlayerState.Shopping);

        PetStateManager.RemoveState(PetState.Playing);
        PetStateManager.RemoveState(PetState.Sitting);
        PetStateManager.RemoveState(PetState.Sleeping);
        PetBehaviour.Instance.activeBehaviour = Behaviour.Roaming;

        lighting.shadows = LightShadows.None;

        CameraHandler.Instance.RefreshRenderers();
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
