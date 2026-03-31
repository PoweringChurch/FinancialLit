using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIOverlay : MonoBehaviour
{
    public static UIOverlay Instance;
    // resources
    public TextMeshProUGUI shampooText;
    public TextMeshProUGUI foodText;
    // balance
    public TextMeshProUGUI moneyText;
    // date
    public TextMeshProUGUI dayWeekText;
    public TextMeshProUGUI hourMinText;
    public GameObject speedUpNotif;
    public void Start()
    {
        Instance = this;
        UpdateResourcesAndBal();
    }

    public void OpenFurnitureTab()
    {
        PlayerFlagManager.AddFlag(PlayerFlag.Placement);
        PlayerFlagManager.RemoveFlag(PlayerFlag.WallPlacement);
        WallPlacement.Instance.CancelPlacement();
    }
    public void OpenRoomsTab()
    {
        PlayerFlagManager.AddFlag(PlayerFlag.WallPlacement);
        PlayerFlagManager.RemoveFlag(PlayerFlag.Placement);
        FurniturePlacer.Instance.CancelPlacement();
    }
    public void ClosePlacement()
    {
        PlayerFlagManager.RemoveFlag(PlayerFlag.WallPlacement);
        PlayerFlagManager.RemoveFlag(PlayerFlag.Placement);
        FurniturePlacer.Instance.CancelPlacement();
        WallPlacement.Instance.CancelPlacement();
    }
    // update the player's resources and balance
    public void UpdateResourcesAndBal()
    {
        shampooText.text = PlayerResources.Instance.Shampoo.ToString();
        foodText.text = PlayerResources.Instance.Food.ToString();
        moneyText.text = $"Balance: ${FinancialSpending.Instance.Balance:N2}";
    }
    // updates the ingame time
    public void UpdateTime()
    {
        int minute = GameTime.Instance.Minute; // 0 - 1439, 0 indexed
        int day = GameTime.Instance.Day; // 0-6, 0 indexed
        int week = GameTime.Instance.Week; // inf p much
        
        speedUpNotif.SetActive(GameTime.Instance.IsFastForwarding);
        // convert day index to day name
        string[] dayNames = { "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "SUNDAY" };
        dayWeekText.text = $"{dayNames[day]}, WEEK {week}";
        
        // convert minute (0-1439) to hour and minute
        int hour = minute / 60;  // 0-23
        int min = minute % 60;   // 0-59
        
        // convert to 12-hour format
        string period = hour >= 12 ? "PM" : "AM";
        int displayHour = hour % 12;
        if (displayHour == 0) displayHour = 12; // 0 should be 12
        
        hourMinText.text = $"{displayHour:D2}:{min:D2} {period}";

    }
}