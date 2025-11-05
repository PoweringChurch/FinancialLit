using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PetUI : MonoBehaviour
{
    public Image HungerFill;
    public Image HygieneFill;
    public Image EntertainmentFill;
    public Image EnergyFill;
    public GameObject warningPrefab;
    public Transform warningContent;

    private Dictionary<string, GameObject> activeWarnings = new();
    private void Start()
    {
        UpdateUI();
        AddWarning("Hello, world!");
    }
    private float elapsed = 0;
    private float time = 0.1f;
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > time)
        {
            elapsed = 0;
            UpdateUI(); // Your function gets called every 0.1 seconds
        }
    }
    void UpdateUI()
    {
        HygieneFill.fillAmount = Pet.Instance.Status["hygiene"];
        HungerFill.fillAmount = Pet.Instance.Status["hunger"];
        EntertainmentFill.fillAmount = Pet.Instance.Status["entertainment"];
        EnergyFill.fillAmount = Pet.Instance.Status["energy"];
    }
    public void AddWarning(string message)
    {
        var newWarningMessage = Instantiate(warningPrefab, warningContent);
        TextMeshProUGUI messagetext = newWarningMessage.GetComponentInChildren<TextMeshProUGUI>();
        messagetext.text = message;

        activeWarnings[message] = newWarningMessage;
    }
    public void ResolveWarning(string message)
    {
        Destroy(activeWarnings[message]);
        activeWarnings.Remove(message);   
    }
}