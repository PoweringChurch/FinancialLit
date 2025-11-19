using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaHandler : MonoBehaviour
{
    public static AreaHandler Instance;
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
    private Dictionary<string, AreaData> areaDict = new();
    
    [Header("References")]
    [SerializeField] private PetStats pet;

    private GameObject currentArea;
    private void Awake()
    {
        Instance = this;
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
        PlayerFlagManager.RemoveFlag(PlayerState.Home);

        PetStateMachine.SetState(PetState.Idle);
        PetAnimation.Instance.SetBoolParameter("IsPlaying", false);
        PetAnimation.Instance.SetBoolParameter("IsSitting", false);

        PetBehaviour.Instance.ActiveBehaviour = Behaviour.Roaming;

        currentArea = Instantiate(area.prefab, gameSpace);
        CameraHandler.Instance.RefreshRenderers();
        
        lighting.shadows = LightShadows.None;
        PetStats.Instance.gameObject.SetActive(false);

        if (area.isShop) PlayerFlagManager.AddFlag(PlayerState.Shopping);
        if (area.shadows) lighting.shadows = LightShadows.Soft;
        if (area.bringPet) {
            PetStats.Instance.gameObject.SetActive(true);
            PetMover.Instance.agent.Warp(Vector3.up);
            }

        if (area.areaName == "Park")
            PetStats.Instance.atPark = true;
        else
            PetStats.Instance.atPark = false;

        UIHandler.Instance.ButtonManager.DisableButton("Build");
    }
    public void EnterHome()
    {
        CleanupCurrentArea();

        home.SetActive(true);
        PetStats.Instance.atPark = false;
        
        PetStats.Instance.gameObject.SetActive(true);
        PetMover.Instance.agent.Warp(Vector3.up);
        PlayerFlagManager.AddFlag(PlayerState.Home);
        PlayerFlagManager.RemoveFlag(PlayerState.Shopping);

        PetStateMachine.SetState(PetState.Idle);
        PetAnimation.Instance.SetBoolParameter("IsPlaying", false);
        PetAnimation.Instance.SetBoolParameter("IsSitting", false);

        PetBehaviour.Instance.ActiveBehaviour = Behaviour.Default;

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
