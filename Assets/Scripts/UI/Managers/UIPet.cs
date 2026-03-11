using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIPet : MonoBehaviour
{
    public GameObject StatusPanel;
    public GameObject collapseArrow;
    public GameObject expandArrow;
    [SerializeField]
    private Image DisplayImage;
    [SerializeField]
    private TextMeshProUGUI DisplayText;
    [SerializeField]
    private TextMeshProUGUI NameText;
    [SerializeField]
    private Image HungerFill;
    [SerializeField]
    private Image HygieneFill;
    [SerializeField]
    private Image EntertainmentFill;
    [SerializeField]
    private Image EnergyFill;

    private float lerpHygiene;
    private float lerpHunger;
    private float lerpFun;
    private float lerpEnergy;
    private Color currentDisplayColor;
    private float lerpSpeed = 3f;

    private Color highStatsColor = new(0.37f,0.94f,0.57f);
    private Color lowStatsColor = new(0.93f,0.49f,0.37f);
    
    private void Update()
    {
        if (PetHelper.CurrentActivePet == null) return;
        // get stats
        var hygiene = PetHelper.petStats.Status["hygiene"];
        var hunger = PetHelper.petStats.Status["hunger"];
        var fun = PetHelper.petStats.Status["entertainment"];
        var energy = PetHelper.petStats.Status["energy"];

        // lerp the stats
        lerpHygiene = Mathf.Lerp(lerpHygiene, hygiene, Time.deltaTime * lerpSpeed);
        lerpHunger = Mathf.Lerp(lerpHunger, hunger, Time.deltaTime * lerpSpeed);
        lerpFun = Mathf.Lerp(lerpFun, fun, Time.deltaTime * lerpSpeed);
        lerpEnergy = Mathf.Lerp(lerpEnergy, energy, Time.deltaTime * lerpSpeed);
        // set the fill amount
        HygieneFill.fillAmount = lerpHygiene / 100;
        HungerFill.fillAmount = lerpHunger / 100;
        EntertainmentFill.fillAmount = lerpFun / 100;
        EnergyFill.fillAmount = lerpEnergy / 100;
        // set the color
        HungerFill.color = Color.Lerp(lowStatsColor, highStatsColor, lerpHunger/100);
        EnergyFill.color = Color.Lerp(lowStatsColor, highStatsColor, lerpEnergy/100);
        HygieneFill.color = Color.Lerp(lowStatsColor, highStatsColor, lerpHygiene/100);
        EntertainmentFill.color = Color.Lerp(lowStatsColor, highStatsColor, lerpFun/100);

        // display mood
        float total = (fun + hygiene + energy + hunger) / 4f;
        string displaytext = "OKAY";

        // positive moods (based on total)
        if (total > 87.5f) displaytext = "HAPPY";
        if (total > 92.5f) displaytext = "CHEERFUL";
        if (total > 97.5f) displaytext = "JOYFUL";

        // negative moods (priority based on specific stats)
        if (fun < 50f) displaytext = "BORED";
        if (fun < 20f) displaytext = "LONELY";
        if (hygiene < 30f) displaytext = "STINKY";
        if (hygiene < 15f) displaytext = "FILTHY";
        if (hunger < 50f) displaytext = "HUNGRY";
        if (hunger < 30f) displaytext = "STARVING";
        if (energy < 40f) displaytext = "SLEEPY";
        if (energy < 20f) displaytext = "EXHAUSTED";

        // multistat moods
        if (energy < 30f && fun < 30f) displaytext = "MISERABLE";
        if (hunger < 25f && hygiene < 25f) displaytext = "UNWELL";

        // critical state overrides everything
        if (total < 20f) displaytext = "CRITICAL";
        var colorDict = new Dictionary<string, Color>
        {
            ["OKAY"] = new Color(0.7f, 0.8f, 0.7f, 0.75f),        // greenish gray
            ["HAPPY"] = new Color(0.5f, 0.85f, 0.6f, 0.75f),      // light green
            ["CHEERFUL"] = new Color(0.4f, 0.9f, 0.5f, 0.75f),    // bright green
            ["JOYFUL"] = new Color(1f, 0.85f, 0.2f, 0.75f),       // golden yellow
            ["BORED"] = new Color(0.6f, 0.6f, 0.75f, 0.75f),      // dull blue gray
            ["LONELY"] = new Color(0.45f, 0.45f, 0.65f, 0.75f),   // sad blue
            ["STINKY"] = new Color(0.65f, 0.55f, 0.3f, 0.75f),    // muddy brown
            ["FILTHY"] = new Color(0.5f, 0.4f, 0.2f, 0.75f),      // dark brown
            ["HUNGRY"] = new Color(0.8f, 0.5f, 0.3f, 0.75f),      // orange
            ["STARVING"] = new Color(0.9f, 0.3f, 0.1f, 0.75f),    // red orange
            ["SLEEPY"] = new Color(0.6f, 0.5f, 0.7f, 0.75f),      // soft purple
            ["EXHAUSTED"] = new Color(0.4f, 0.35f, 0.4f, 0.75f),  // dark gray purple
            ["MISERABLE"] = new Color(0.5f, 0.3f, 0.5f, 0.75f),   // dark purple
            ["UNWELL"] = new Color(0.6f, 0.45f, 0.3f, 0.75f),     // bleghh brown
            ["CRITICAL"] = new Color(0.8f, 0.1f, 0.1f, 0.75f)     // danger red
        };
        // set the color
        Color targetColor = colorDict[displaytext];
        currentDisplayColor = Color.Lerp(currentDisplayColor, targetColor, Time.deltaTime * lerpSpeed);
        DisplayImage.color = currentDisplayColor;
        DisplayText.text = displaytext;
        NameText.text = PetHelper.petStats.petName;
    }
    // toggle the status panel
    public void ToggleStatusPanel()
    {
        bool panelActive = StatusPanel.activeSelf;
        StatusPanel.SetActive(!panelActive);
        collapseArrow.SetActive(!panelActive);
        expandArrow.SetActive(panelActive);
    }
}