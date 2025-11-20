using UnityEngine;
using System;
public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance;
    
    [Header("Manager Settings")]
    public UIButtonManager ButtonManager = new();
    public UIInventoryManager InventoryManager = new();
    public UIResourcesUpdater ItemUpdater = new();
    public UIPetManager PetUI = new();
    public UICursorHelper CursorHelper = new();
    public UIPopupManager PopupManager = new();
    public UISaveManager SaveManagerUI = new();
    public UIFlagManager FlagManager = new();
    public UIWorkManager WorkManager = new();
    public UIAnimationManager MenuAnimation = new();
    public UISettingsManager SettingsManager = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        ButtonManager.Initialize();
        InventoryManager.Initialize();
        ItemUpdater.Initialize();
        PetUI.Initialize();
        SaveManagerUI.Initialize();
        FlagManager.Initialize();
        WorkManager.Initialize();
        SettingsManager.Initialize();
    }
    void Update()
    {
        PetUI.UpdateUI();
        MenuAnimation.UpdateUI();
    }
    //intermediarys
    public void OpenBuilder()
    {
        InventoryManager.UpdateInventoryUI();
        PlayerFlagManager.AddFlag(PlayerFlag.Placement);
    }
    public void ToggleStatusPanel()
    {
        PetUI.StatusPanel.SetActive(!PetUI.StatusPanel.activeSelf);
    }
    public void NewSave()
    {
        SaveManagerUI.NewSave();
    }
    public void SaveGame()
    {
        SaveHandler.Instance.SaveGame();
        SaveManagerUI.DisplaySaves();
    }
    public void DeleteCurrentSave()
    {
        SaveManagerUI.DeleteCurrentSave();
    }
    public void CancelWorking()
    {
        WorkManager.CancelWork();
    }
    public void ToggleSpendingsPanel()
    {
        ItemUpdater.spendingsPanel.SetActive(!ItemUpdater.spendingsPanel.activeSelf);
    }
    public void CloseBuilder()
    {
        PlayerFlagManager.RemoveFlag(PlayerFlag.Placement);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ClosePopups()
    {
        PopupManager.CloseAllPopups();
    }
}