using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggler : MonoBehaviour
{
    public static ButtonToggler Instance;
    [Serializable]
    public class ToggleableButton
    {
        public string name;
        public Button button;
        public RawImage unavailableImage;
    }
    
    [SerializeField] private List<ToggleableButton> buttons;
    private Dictionary<string, ToggleableButton> buttonDict;

    private void Awake()
    {
        Instance = this;
        buttonDict = new Dictionary<string, ToggleableButton>();
        foreach (var btn in buttons)
        {
            buttonDict[btn.name] = btn;
        }
    }

    public void DisableButton(string buttonName) => SetButtonState(buttonName, false);
    public void EnableButton(string buttonName) => SetButtonState(buttonName, true);

    private void SetButtonState(string buttonName, bool enabled)
    {
        if (!buttonDict.TryGetValue(buttonName, out ToggleableButton targetButton))
        {
            Debug.LogWarning($"Button '{buttonName}' not found!");
            return;
        }

        if (targetButton.button) targetButton.button.enabled = enabled;
        if (targetButton.unavailableImage) targetButton.unavailableImage.enabled = !enabled;
    }
}