using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[Serializable]
public class UIPopups : MonoBehaviour
{
    public static UIPopups Instance;
    public GameObject infoPanelTemplate;
    public GameObject ynPanelTemplate;
    public GameObject taskPanelTemplate;
    public Transform PopupsTransform;
    public AudioClip popupsfx;

    public void Awake()
    {
        Instance = this;
    }
    // pops up the provided info; only has a dismiss button
    public void PopupInfo(string header, string body, string dismiss = "OK", Action action = null)
    {
        UISFXPlayer.Instance.Play(popupsfx);
        
        GameObject newInfoPanel = Instantiate(infoPanelTemplate, PopupsTransform);
        var tmps = newInfoPanel.GetComponentsInChildren<TextMeshProUGUI>();

        // Header, Body, Dismiss button text
        tmps[0].text = header;
        tmps[1].text = body;
        tmps[2].text = dismiss;

        // Setup dismiss button
        Button dismissButton = newInfoPanel.GetComponentInChildren<Button>();
        dismissButton.onClick.AddListener(() => {
            Destroy(newInfoPanel);
            if (action != null) action.Invoke();
        });
    }
    // pops up the provided Yes/No info; has a yes/no button response
    public void PopupYN(string header, string body, Action onYes, Action onNo = null, string y = "Yes", string n = "No")
    {
        UISFXPlayer.Instance.Play(popupsfx);

        GameObject newYNPanel = Instantiate(ynPanelTemplate, PopupsTransform);
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
            Destroy(newYNPanel);
        });
        
        // No button
        buttons[1].onClick.AddListener(() => 
        {
            onNo?.Invoke();
            Destroy(newYNPanel);
        });
    }
    // pops up the provided task
    public GameObject PopupTask(string header, string body) 
    {
        UISFXPlayer.Instance.Play(popupsfx);

        GameObject newTaskPanel = Instantiate(taskPanelTemplate, PopupsTransform);
        var tmps = newTaskPanel.GetComponentsInChildren<TextMeshProUGUI>();
        tmps[0].text = header;
        tmps[1].text = body;

        return newTaskPanel;
    }
    // closes all popups
    public void CloseAllPopups()
    {
        if (PopupsTransform == null) return;
        for (int i = PopupsTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = PopupsTransform.GetChild(i);
            if (child != null)
                Destroy(child.gameObject);
        }
    }
}