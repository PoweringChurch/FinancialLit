using System;
using TMPro;
using UnityEngine;

[Serializable]
public class UIResourcesUpdater : MonoBehaviour
{
    public static UIResourcesUpdater Instance;
    
    public GameObject spendingsPanel;

    public TextMeshProUGUI shampooText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI moneyText;

    public TextMeshProUGUI foodspendings;
    public TextMeshProUGUI hygieneSpendings;
    public TextMeshProUGUI furnitureSpendings;
    public TextMeshProUGUI healthcareSpendings;

    public void Start()
    {
        Instance = this;
        UpdateText();
    }
    public void ToggleSpendingsPanel()
    {
        spendingsPanel.SetActive(!spendingsPanel.activeSelf);
    }
    public void UpdateText()
    {
        shampooText.text = PlayerResources.Instance.Shampoo.ToString();
        foodText.text = PlayerResources.Instance.Food.ToString();
        moneyText.text = $"Balance: ${PlayerResources.Instance.Money:N2}";

        foodspendings.text = $"${PlayerResources.Instance.Spendings["Food"].ToString()}";
        hygieneSpendings.text = $"${PlayerResources.Instance.Spendings["Hygiene"].ToString()}";
        healthcareSpendings.text = $"${PlayerResources.Instance.Spendings["Healthcare"].ToString()}";
        furnitureSpendings.text = $"${PlayerResources.Instance.Spendings["Furniture"].ToString()}";
    }
}