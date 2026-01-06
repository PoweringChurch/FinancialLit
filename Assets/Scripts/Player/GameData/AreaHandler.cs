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
        PlayerFlagManager.RemoveFlag(PlayerFlag.Home);

        PetHelper.petStateMachine.SetState(PetState.Idle);
        PetHelper.petAnimation.SetBoolParameter("IsPlaying", false);
        PetHelper.petAnimation.SetBoolParameter("IsSitting", false);

        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Roaming;

        currentArea = Instantiate(area.prefab, gameSpace);
        CameraHandler.Instance.RefreshRenderers();
        
        lighting.shadows = LightShadows.None;
        PetHelper.petStats.gameObject.SetActive(false);

        if (area.isShop) {
            PlayerFlagManager.AddFlag(PlayerFlag.Shopping);
            }
        if (area.shadows) lighting.shadows = LightShadows.Soft;
        if (area.bringPet) {
            PetHelper.petStats.gameObject.SetActive(true);
            PetHelper.petMover.agent.Warp(Vector3.up);
            }

        if (area.areaName == "Park")
            {if (!SaveHandler.Instance.currentPlayerData.VisitedPark)
                {
                    string header = "Park";
                    string body = "At the park, your dog passively gains entertainment, and can get worn out if you stay here for a while. After getting worn out, you pet will sleep much easier!";
                    UIPopups.Instance.PopupInfo(header,body);
                }
            SaveHandler.Instance.currentPlayerData.VisitedPark = true;
            PetHelper.petStats.atPark = true;}
        else
            PetHelper.petStats.atPark = false;

        if (area.areaName == "Veterinary")
        {
            if (!SaveHandler.Instance.currentPlayerData.VisitedVet)
            {
                string header = "Veterinary";
                string body = "If your pet ever gets sick, you can visit the vet to cure them for a fee!";
                UIPopups.Instance.PopupInfo(header,body);
            }
            SaveHandler.Instance.currentPlayerData.VisitedVet = true;
        }
        if (area.areaName == "SmartyPets")
        {
            if (!SaveHandler.Instance.currentPlayerData.VisitedSmartyPets)
            {
                string header = "SmartyPets";
                string body = "Welcome to SmartyPets! Here you can purchase pet-related items, like pet beds, food, shampoo, or pet toys. Purchase items by selecting the item you wish to purchase and pressing buy.";
                UIPopups.Instance.PopupInfo(header,body);
            }
            SaveHandler.Instance.currentPlayerData.VisitedSmartyPets = true;
        }
        if (area.areaName == "FurnitureStore")
        {
            if (!SaveHandler.Instance.currentPlayerData.VisitedFurnitureStore)
            {
                string header = "Furniture Store";
                string body = "At the furniture store, you can purchase furniture to place in your home. Purchased furniture gets added to your inventory for placement in the placement menu.";
                UIPopups.Instance.PopupInfo(header,body);
            }
            SaveHandler.Instance.currentPlayerData.VisitedFurnitureStore = true;
        }
        UIButtons.Instance.DisableButton("Build");
    }
    public void EnterHome()
    {
        CleanupCurrentArea();

        home.SetActive(true);
        PetHelper.petStats.atPark = false;
        
        PetHelper.petStats.gameObject.SetActive(true);
        PetHelper.petMover.agent.Warp(Vector3.up);
        PlayerFlagManager.AddFlag(PlayerFlag.Home);
        PlayerFlagManager.RemoveFlag(PlayerFlag.Shopping);

        PetHelper.petStateMachine.SetState(PetState.Idle);
        PetHelper.petAnimation.SetBoolParameter("IsPlaying", false);
        PetHelper.petAnimation.SetBoolParameter("IsSitting", false);

        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;

        lighting.shadows = LightShadows.None;

        CameraHandler.Instance.RefreshRenderers();
        UIButtons.Instance.EnableButton("Build");
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
