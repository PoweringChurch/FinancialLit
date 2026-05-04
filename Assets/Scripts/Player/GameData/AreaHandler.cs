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
        public Vector3 origin;
        public float bounds = 20;
        public bool shadows;
        public bool bringPet;
        public bool isShop;
    }

    [Header("Area Setup")]
    [SerializeField] private GameObject home;
    [SerializeField] private AreaData[] areas;
    [SerializeField] private Transform gameSpace;
    [SerializeField] private Light lighting;
    private Dictionary<string, AreaData> areaDict = new();
    
    private void Awake()
    {
        Instance = this;
        foreach (var area in areas)
            areaDict[area.areaName] = area;
    }
    
    public void EnterArea(string areaName)
    {
        if (!areaDict.TryGetValue(areaName, out AreaData area))
        {
            Debug.LogError($"Area '{areaName}' not found");
            return;
        }
        PlacementManager.Instance.SetMode(PlacementManager.Mode.None);
        PlayerFlagManager.RemoveFlag(PlayerFlag.Home);

        PetHelper.petStateMachine.SetState(PetState.Idle);
        PetHelper.petAnimation.SetBoolParameter("IsPlaying", false);
        PetHelper.petAnimation.SetBoolParameter("IsSitting", false);

        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Roaming;

        CameraHandler.Instance.RefreshRenderers();
        
        //lighting.shadows = LightShadows.None;

        if (area.isShop) {
            PlayerFlagManager.AddFlag(PlayerFlag.Shopping);
            }
        //if (area.shadows) lighting.shadows = LightShadows.Soft;
        if (area.bringPet) {
            PetHelper.petMover.agent.Warp(Vector3.up);
            }

        if (area.areaName == "Park")
            {
                if (!SaveHandler.Instance.currentPlayerData.VisitedPark)
                {
                    string header = "Park";
                    string body = "At the park, your dog passively gains entertainment, and can get worn out if you stay here for a while. After getting worn out, you pet will sleep much easier!";
                    UIPopups.Instance.PopupInfo(header,body);
                }
                CameraHandler.Instance.SetCameraOrigin(area.origin, area.bounds);
                SaveHandler.Instance.currentPlayerData.VisitedPark = true;
                PetHelper.petStats.atPark = true;
            }
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
            CameraHandler.Instance.SetCameraOrigin(area.origin, area.bounds);
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
            CameraHandler.Instance.SetCameraOrigin(area.origin, area.bounds);
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
            CameraHandler.Instance.SetCameraOrigin(area.origin, area.bounds);
            SaveHandler.Instance.currentPlayerData.VisitedFurnitureStore = true;
        }
        PetHelper.CurrentActivePet.GetComponent<Collider>().enabled = area.bringPet;
        PetHelper.CurrentActivePet.transform.Find("PetModel").gameObject.SetActive(area.bringPet);
        PetHelper.CurrentActivePet.transform.Find("StinkyParticles").gameObject.SetActive(area.bringPet);
        UIButtons.Instance.DisableButton("Build");
    }
    public void EnterHome()
    {
        CameraHandler.Instance.SetCameraOrigin(Vector3.zero, 20);
        PetHelper.CurrentActivePet.GetComponent<Collider>().enabled = true;
        PetHelper.petStats.atPark = false;
        // reset pet's position
        PetHelper.petMover.agent.Warp(Vector3.up);
        PlayerFlagManager.AddFlag(PlayerFlag.Home);
        PlayerFlagManager.RemoveFlag(PlayerFlag.Shopping);

        // handle pets behaviour and animation
        PetHelper.petStateMachine.SetState(PetState.Idle);
        PetHelper.petAnimation.SetBoolParameter("IsPlaying", false);
        PetHelper.petAnimation.SetBoolParameter("IsSitting", false);

        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;

        //lighting.shadows = LightShadows.None;
        // reactivate pet
        PetHelper.CurrentActivePet.transform.Find("PetModel").gameObject.SetActive(true);
        PetHelper.CurrentActivePet.transform.Find("StinkyParticles").gameObject.SetActive(true);
        // refresh the pet's renderers
        CameraHandler.Instance.RefreshRenderers();
        UIButtons.Instance.EnableButton("Build");
    }
}
