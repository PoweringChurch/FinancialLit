using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    private UIPopupManager PopupManager => UIHandler.Instance.PopupManager;
    private GameObject currentTaskUI;
    public Button moodDisplay;
    void Awake()
    {
        Instance = this;
    }
    private void OnDestroy()
    {
        CleanupEventListeners();
    }
    
    public void AskTutorial()
    {
        UIHandler.Instance.PopupManager.PopupYN("Tutorial", 
            "Would you like to learn how to play?", 
            () => StartTutorial(), 
            null, 
            "Teach me!", 
            "Skip");
    }
    
    public void StartTutorial()
    {
        PlayerFlagManager.AddFlag(PlayerFlag.Tutorial);
        CleanupEventListeners();
        ShowWelcome();
    }
    
    private void CleanupEventListeners()
    {
        if (moodDisplay != null)
            moodDisplay.onClick.RemoveListener(ShowNeedsAcknowledgement);
        
        if (FurniturePlacer.Instance != null)
            FurniturePlacer.Instance.OnItemPlaced -= ShowPlacementAcknowledgement;
        
        if (OrderHandler.Instance != null)
        {
            OrderHandler.Instance.OnWorkStarted -= OnWorkStartedHandler;
            OrderHandler.Instance.OnWorkEnded -= ShowWorkAcknowledgement;
        }
        
        if (currentTaskUI != null)
            Destroy(currentTaskUI);
    }
    
    // Step 1
    void ShowWelcome()
    {
        string header = "Welcome";
        string body = "Let's learn how to take care of your new pet!";
        string dismiss = "OK";
        PopupManager.PopupInfo(header, body, dismiss, ShowControls);
    }
    // Step 1.5
    void ShowControls()
    {
        string header = "Controls";
        string body = "You can use WASD to move the camera around, use the scroll wheel to zoom in and out.\n\n"+
        "You can interact with objects by clicking on them with the left mouse button.";
        string dismiss = "OK";
        PopupManager.PopupInfo(header, body, dismiss, ShowPetCareBasics);
    }
    // Step 2
    void ShowPetCareBasics()
    {
        string header = "The basics";
        string body = "Keep your pet healthy by managing four needs:\n" +
            "• Hygiene - Bathe regularly\n" +
            "• Hunger - Feed when hungry\n" +
            "• Energy - Let them rest\n" +
            "• Fun - Play with toys or at the park";
        string dismiss = "Next";
        PopupManager.PopupInfo(header, body, dismiss, ShowHealthWarning);
    }
    
    void ShowHealthWarning()
    {
        string header = "Pet health";
        string body = "If your pet's needs ever get too low, they might get sick and need a trip to the vet, which can be expensive!\n\n" +
            "You can check out your pet's stats by clicking on the mood display in the bottom left.";
        string dismiss = "Next";
        PopupManager.PopupInfo(header, body, dismiss, TaskOpenNeeds);
    }
    
    void TaskOpenNeeds()
    {
        string header = "View pet needs";
        string body = "Press on the mood display on the bottom left to view your pet's needs";
        currentTaskUI = PopupManager.PopupTask(header, body);
        
        if (moodDisplay != null)
            moodDisplay.onClick.AddListener(ShowNeedsAcknowledgement);
    }
    
    void ShowNeedsAcknowledgement()
    {
        if (moodDisplay != null)
            moodDisplay.onClick.RemoveListener(ShowNeedsAcknowledgement);
        
        string header = "Pet health";
        string body = "Great!\nThese stats are viewable at any time.";
        string dismiss = "Got it";
        
        if (currentTaskUI != null)
            Destroy(currentTaskUI);
        
        PopupManager.PopupInfo(header, body, dismiss, ShowPlacement);
    }
    
    // Step 3
    void ShowPlacement()
    {
        string header = "Placement system";
        string body = "Your home is empty!\n" +
            "To start building, expand the arrow on the bottom right and select the yellow button to enter placement mode.";
        string dismiss = "Next";
        PopupManager.PopupInfo(header, body, dismiss, TaskPlaceOldMonitor);
    }
    
    void TaskPlaceOldMonitor()
    {
        string header = "Placement system";
        string body = "Open the placement menu and place down an old monitor";
        currentTaskUI = PopupManager.PopupTask(header, body);
        
        if (FurniturePlacer.Instance != null)
            FurniturePlacer.Instance.OnItemPlaced += ShowPlacementAcknowledgement;
        if (FurniturePlacer.Instance.IsItemPlaced("Old Monitor"))
        {
            ShowPlacementAcknowledgement("Old Monitor");
        }
    }
    
    void ShowPlacementAcknowledgement(string itemName)
    {
        if (itemName != "Old Monitor")
            return;
        
        if (FurniturePlacer.Instance != null)
            FurniturePlacer.Instance.OnItemPlaced -= ShowPlacementAcknowledgement;
        
        string header = "Nice work!";
        string body = "You've successfully placed your first item. You can use this system to decorate your home with furniture and equipment.";
        string dismiss = "Got it";
        
        if (currentTaskUI != null)
            Destroy(currentTaskUI);
        
        PopupManager.PopupInfo(header, body, dismiss, ShowWorking);
    }
    
    // Step 4
    void ShowWorking()
    {
        string header = "Money";
        string body = "You can work from home to earn money! To begin working, click any monitor and press \"Go to work\".";
        string dismiss = "Next";
        PopupManager.PopupInfo(header, body, dismiss, TaskBeginWorking);
    }
    
    void TaskBeginWorking()
    {
        string header = "Money";
        string body = "Close the placement menu, and select the placed monitor to begin working";
        currentTaskUI = PopupManager.PopupTask(header, body);
        
        if (OrderHandler.Instance != null)
        {
            OrderHandler.Instance.OnWorkStarted += OnWorkStartedHandler;
            OrderHandler.Instance.OnWorkEnded += ShowWorkAcknowledgement;
        }
    }
    
    void OnWorkStartedHandler()
    {
        if (currentTaskUI != null)
            Destroy(currentTaskUI);
    }
    
    void ShowWorkAcknowledgement()
    {
        if (OrderHandler.Instance != null)
        {
            OrderHandler.Instance.OnWorkStarted -= OnWorkStartedHandler;
            OrderHandler.Instance.OnWorkEnded -= ShowWorkAcknowledgement;
        }
        
        string header = "Money";
        string body = "You can view how you spend your money by clicking on your balance in the top left.\n\nBe sure to spend responsibly!";
        string dismiss = "Got it";
        PopupManager.PopupInfo(header, body, dismiss, ShowResources);
    }
    // Step 5
    void ShowResources()
    {
        string header = "Resources";
        string body = "Underneath your balance shows the amount of pet shampoo and pet food you have. \n\n"+
        "Make sure you have enough of both of these at all times, as you are unable to bathe or feed your pet if you run out.";
        string dismiss = "Next";
        PopupManager.PopupInfo(header,body,dismiss,ShowTravel);
    }
    // Step 6
    void ShowTravel()
    {
        string header = "Travel";
        string body = "You can visit other areas by selecting the blue travel button in the bottom right panel, above the placement button.";
        string dismiss = "Next";
        PopupManager.PopupInfo(header, body, dismiss, ShowTravelAdditionalInfo);
    }
    
    void ShowTravelAdditionalInfo()
    {
        string header = "Travel";
        string body = "Different areas offer different services. Explore them all!";
        string dismiss = "Great";
        PopupManager.PopupInfo(header, body, dismiss, ShowFinal);
    }
    
    // Step 6
    void ShowFinal()
    {
        string header = "Take on the world";
        string body = "That's everything you need to know! Have fun taking care of your pet!";
        string dismiss = "I'm ready";
        PopupManager.PopupInfo(header, body, dismiss, CloseTutorial);
    }
    public void CloseTutorial()
    {
        PlayerFlagManager.RemoveFlag(PlayerFlag.Tutorial);
        CleanupEventListeners();
        PopupManager.CloseAllPopups();
        currentTaskUI = null;
    }
}