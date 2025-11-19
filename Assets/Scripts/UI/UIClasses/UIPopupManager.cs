using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[Serializable]
public class UIPopupManager
{
    public GameObject infoPanelTemplate;
    public GameObject ynPanelTemplate;
    public GameObject taskPanelTemplate;
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
    public UnityEvent PopupTask(string header, string body, Action onComplete)
    {
        GameObject newTaskPanel = UnityEngine.Object.Instantiate(taskPanelTemplate, PopupsTransform);
        var tmps = newTaskPanel.GetComponentsInChildren<TextMeshProUGUI>();
        tmps[0].text = header;
        tmps[1].text = body;

        UnityEvent OnCompletion = new();
        OnCompletion.AddListener(() => 
        {
            onComplete.Invoke();
            UnityEngine.Object.Destroy(newTaskPanel);
        });
        return OnCompletion;
    }
}