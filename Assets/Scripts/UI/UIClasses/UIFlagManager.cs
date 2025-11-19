using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class UIFlagManager
{

    [Serializable]
    public class FlagIcon
    {
        public GameObject gameObject; //object with rawimage
        public string Name;
        public string Effect; //Positive, Mixed, Negative
        public string Description;
        public PetFlag petFlag;
    }
    public Transform popupsContainer;
    public Transform flagContainerTransform; 
    public GameObject descriptionDisplayPrefab; //panel w/ three tmpro children, [0] = name, [1] = effect, [2] = desc
    public FlagIcon[] flagIcons;

    private GameObject currentDescription;
    private Dictionary<PetFlag, FlagIcon> flagIconMap;
    private FlagIcon currentDescriptionIcon;
    //icon should show description when hovered over by mouse
    public void Initialize()
    {
    flagIconMap = new Dictionary<PetFlag, FlagIcon>();
    foreach (var icon in flagIcons)
    {
        flagIconMap[icon.petFlag] = icon;
        icon.gameObject.SetActive(false);
        
        var eventTrigger = icon.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null) eventTrigger = icon.gameObject.AddComponent<EventTrigger>();
        
        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => ShowDescription(icon));
        eventTrigger.triggers.Add(pointerEnter);
        
        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => HideDescription());
        eventTrigger.triggers.Add(pointerExit);
        }
        
        PetFlagManager.OnFlagChanged += UpdateFlags;
        UpdateFlags();
    }
    void UpdateFlags()
    {
        foreach (var icon in flagIcons)
        {
            bool hasFlag = PetFlagManager.HasFlag(icon.petFlag);
            icon.gameObject.SetActive(hasFlag);
            
            if (!hasFlag && currentDescription != null && currentDescriptionIcon == icon)
            {
                HideDescription();
            }
        }
    }
    void ShowDescription(FlagIcon icon)
    {
        HideDescription();
        currentDescriptionIcon = icon;
        currentDescription = UnityEngine.Object.Instantiate(descriptionDisplayPrefab, popupsContainer);
        var texts = currentDescription.GetComponentsInChildren<TMP_Text>();
        texts[0].text = icon.Name;
        texts[1].text = icon.Effect;
        texts[2].text = icon.Description;
    }
    void HideDescription()
    {
        if (currentDescription != null) UnityEngine.Object.Destroy(currentDescription);
    }
    
    void OnDestroy()
    {
        PetFlagManager.OnFlagChanged -= UpdateFlags;
    }
}