using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class UIStatusEffects : MonoBehaviour
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
    void Start()
    {
        flagIconMap = new Dictionary<PetFlag, FlagIcon>();
        // for each icon in the flag icon
        foreach (var icon in flagIcons)
        {
            flagIconMap[icon.petFlag] = icon;
            icon.gameObject.SetActive(false);

            var eventTrigger = icon.gameObject.GetComponent<EventTrigger>();
            
            if (eventTrigger == null) 
                eventTrigger = icon.gameObject.AddComponent<EventTrigger>();
            
            // when the pointer enters a status effect icon
            var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnter.callback.AddListener((data) => ShowDescription(icon));
            eventTrigger.triggers.Add(pointerEnter);
            // when the pointer exits an icon
            var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExit.callback.AddListener((data) => HideDescription());
            eventTrigger.triggers.Add(pointerExit);
        }
        PetHelper.OnPetLoad += OnPetLoad;
    }
    void OnPetLoad(GameObject pet)
    {
        pet.GetComponent<PetFlagManager>().OnFlagChanged += UpdateFlags;
    }
    // update the flags, occurs whenever flags are changed
    void UpdateFlags(PetFlag _)
    {
        foreach (var icon in flagIcons)
        {
            bool hasFlag = PetHelper.petFlagManager.HasFlag(icon.petFlag);
            icon.gameObject.SetActive(hasFlag);
            
            if (!hasFlag && currentDescription != null && currentDescriptionIcon == icon)
                HideDescription();
        }
    }
    // shows the description of the status effect
    void ShowDescription(FlagIcon icon)
    {
        HideDescription();
        currentDescriptionIcon = icon;
        currentDescription = Instantiate(descriptionDisplayPrefab, popupsContainer);
        var texts = currentDescription.GetComponentsInChildren<TMP_Text>();
        texts[0].text = icon.Name;
        texts[1].text = icon.Effect;
        texts[2].text = icon.Description;
    }
    // hides the description of the status effect
    void HideDescription()
    {
        if (currentDescription != null) UnityEngine.Object.Destroy(currentDescription);
    }
    
    void OnDestroy() //should never even happen
    {
        PetHelper.petFlagManager.OnFlagChanged -= UpdateFlags;
    }
}