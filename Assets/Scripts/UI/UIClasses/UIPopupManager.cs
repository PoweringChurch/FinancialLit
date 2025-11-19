using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIPopupManager
{
    public GameObject infoPanelTemplate;
    public GameObject ynPanelTemplate;
    public Transform PopupsTransform;

    public void PopupInfo(string header, string body, string dismiss = "OK", Action action = null)
    {
        GameObject newInfoPanel = UnityEngine.Object.Instantiate(infoPanelTemplate, PopupsTransform);
        var tmps = newInfoPanel.GetComponentsInChildren<TextMeshProUGUI>();

        // Header, Body, Dismiss button text
        tmps[0].text = header;
        tmps[1].text = body;
        tmps[2].text = dismiss;

        // Setup dismiss button
        Button dismissButton = newInfoPanel.GetComponentInChildren<Button>();
        dismissButton.onClick.AddListener(() => {
            UnityEngine.Object.Destroy(newInfoPanel);
            if (action != null) action.Invoke();
        });
    }
    public void PopupYN(string header, string body, Action onYes, Action onNo = null, string y = "Yes", string n = "No")
    {
        GameObject newYNPanel = UnityEngine.Object.Instantiate(ynPanelTemplate, PopupsTransform);
        var tmps = newYNPanel.GetComponentsInChildren<TextMeshProUGUI>();
        
        // Header, Body, Yes text, No text
        tmps[0].text = header;
        tmps[1].text = body;
        tmps[2].text = y;
        tmps[3].text = n;
        
        Button[] buttons = newYNPanel.GetComponentsInChildren<Button>();
        
        // Yes button
        buttons[0].onClick.AddListener(() => 
        {
            onYes?.Invoke();
            UnityEngine.Object.Destroy(newYNPanel);
        });
        
        // No button
        buttons[1].onClick.AddListener(() => 
        {
            onNo?.Invoke();
            UnityEngine.Object.Destroy(newYNPanel);
        });
    }

    public void AskTutorial()
    {
        PopupYN("Tutorial", 
        "Would you like to learn how to play?", 
        () => StartTutorial(), 
        null, 
        "Teach me!", 
        "Skip");
    }
    public void StartTutorial()
    {
        // Welcome
        PopupInfo("Welcome!", 
            "Let's learn how to take care of your new pet!", 
            "Next", 
            () => {
                // Pet care basics
                PopupInfo("Pet Care Basics", 
                    "Keep your pet healthy by managing four needs:\n\n" +
                    "• Hygiene - Bathe your pet regularly\n" +
                    "• Hunger - Feed your pet when hungry\n" +
                    "• Energy - Send them to rest on a dog bed\n" +
                    "• Entertainment - Play at the park or with toys", 
                    "Got it!", 
                    () => {
                        // Health warning
                        PopupInfo("Stay Healthy", 
                            "If your pet's needs get too low, they might get sick and need a trip to the vet. Keep those stats up! \n\n" +
                            "You're able to view your pet's needs at any time by pressing on the mood display on the bottom left.", 
                            "Understood", 
                            () => {
                                // Working from home
                                PopupInfo("Money", 
                                    "You can work from home to earn money! Just click any monitor and press \"Go to work\". \n\n" +
                                    "You can also see how you spend your earnings by pressing on your balance in the top left!",
                                    "Nice!", 
                                    () => {
                                        // Placement mode
                                        PopupInfo("Furniture & Items", 
                                            "Want to add furniture and decor?\n\n" +
                                            "Expand the arrow on the bottom right and select the yellow button with tools to enter placement mode.\n\n" +
                                            "You've been given some basic furniture items to start building up your home with.", 
                                            "Cool!", 
                                            () => {
                                                // Travel
                                                PopupInfo("Explore & Shop", 
                                                    "Travel to different locations by expanding the arrow on the bottom right and selecting the blue map.\n\n" +
                                                    "Different areas offer different services - explore them all!", 
                                                    "Let's go!", 
                                                    () => {
                                                        // Final message
                                                        PopupInfo("You're Ready!", 
                                                            "That's everything you need to know. Have fun taking care of your pet!", 
                                                            "Enter game");
                                                    });
                                            });
                                    });
                            });
                    });
            });
    }
}