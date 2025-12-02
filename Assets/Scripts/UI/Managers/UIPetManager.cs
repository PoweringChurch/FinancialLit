using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIPetManager
{
    public GameObject StatusPanel;
    [SerializeField]
    private Image DisplayImage;
    [SerializeField]
    private TextMeshProUGUI DisplayText;
    [SerializeField]
    private Image HungerFill;
    [SerializeField]
    private Image HygieneFill;
    [SerializeField]
    private Image EntertainmentFill;
    [SerializeField]
    private Image EnergyFill;

    private float currentHygiene;
    private float currentHunger;
    private float currentEntertainment;
    private float currentEnergy;
    private Color currentDisplayColor;
    private float lerpSpeed = 3f;
    public void Initialize()
    {
        currentHygiene = PetStats.Instance.Status["hygiene"];
        currentHunger = PetStats.Instance.Status["hunger"];
        currentEntertainment = PetStats.Instance.Status["entertainment"];
        currentEnergy = PetStats.Instance.Status["energy"];
        UpdateUI();
    }
    public void UpdateUI()
    {
        var hygiene = PetStats.Instance.Status["hygiene"];
        var hunger = PetStats.Instance.Status["hunger"];
        var entertainment = PetStats.Instance.Status["entertainment"];
        var energy = PetStats.Instance.Status["energy"];

        currentHygiene = Mathf.Lerp(currentHygiene, hygiene, Time.deltaTime * lerpSpeed);
        currentHunger = Mathf.Lerp(currentHunger, hunger, Time.deltaTime * lerpSpeed);
        currentEntertainment = Mathf.Lerp(currentEntertainment, entertainment, Time.deltaTime * lerpSpeed);
        currentEnergy = Mathf.Lerp(currentEnergy, energy, Time.deltaTime * lerpSpeed);

        HygieneFill.fillAmount = currentHygiene;
        HungerFill.fillAmount = currentHunger;
        EntertainmentFill.fillAmount = currentEntertainment;
        EnergyFill.fillAmount = currentEnergy;

        HungerFill.color = Color.Lerp(Color.red, Color.green, currentHunger);
        EnergyFill.color = Color.Lerp(Color.red, Color.green, currentEnergy);
        HygieneFill.color = Color.Lerp(Color.red, Color.green, currentHygiene);
        EntertainmentFill.color = Color.Lerp(Color.red, Color.green, currentEntertainment);

        var total = entertainment + hygiene + energy + hunger;
        string displaytext = "OKAY";

        if (total > 3.5) displaytext = "FINE";
        if (total > 3.7) displaytext = "GREAT";
        if (total > 3.9) displaytext = "WONDERFUL";
        if (entertainment < 0.5) displaytext = "BORED";
        if (entertainment < 0.2) displaytext = "SAD";
        if (hygiene < 0.3) displaytext = "DIRTY";
        if (hunger < 0.5) displaytext = "HUNGRY";
        if (hunger < 0.3) displaytext = "STARVING";
        if (energy < 0.4) displaytext = "TIRED";
        if (energy < 0.2) displaytext = "EXHAUSTED";
        if (total < 0.8) displaytext = "CRITICAL";

        var colorDict = new Dictionary<string, Color>
        {
            ["OKAY"] = new Color(0.5f, 0.6f, 0.5f),
            ["FINE"] = new Color(0.5f, 0.8f, 0.6f),
            ["GREAT"] = new Color(0.5f, 0.9f, 0.6f),
            ["WONDERFUL"] = new Color(0.3f, 0.9f, 0.5f),
            ["BORED"] = new Color(0.6f, 0.6f, 0.8f),
            ["SAD"] = new Color(0.4f, 0.4f, 0.7f),
            ["DIRTY"] = new Color(0.6f, 0.5f, 0.3f),
            ["HUNGRY"] = new Color(0.6f, 0.4f, 0.2f),
            ["STARVING"] = new Color(0.9f, 0.3f, 0f),
            ["TIRED"] = new Color(0.7f, 0.7f, 0.5f),
            ["EXHAUSTED"] = new Color(0.3f, 0.3f, 0.3f),
            ["CRITICAL"] = Color.darkRed
        };

        Color targetColor = colorDict[displaytext];
        currentDisplayColor = Color.Lerp(currentDisplayColor, targetColor, Time.deltaTime * lerpSpeed);
        DisplayImage.color = currentDisplayColor;
        DisplayText.text = displaytext;
    }
}