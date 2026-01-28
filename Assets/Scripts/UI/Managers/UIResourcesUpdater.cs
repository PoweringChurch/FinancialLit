using System;
using TMPro;
using UnityEngine;

[Serializable]
public class UIResourcesUpdater : MonoBehaviour
{
    public static UIResourcesUpdater Instance;
    
    public TextMeshProUGUI shampooText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI moneyText;

    public void Start()
    {
        Instance = this;
        UpdateText();
    }
    public void UpdateText()
    {
        shampooText.text = PlayerResources.Instance.Shampoo.ToString();
        foodText.text = PlayerResources.Instance.Food.ToString();
        moneyText.text = $"Balance: ${PlayerResources.Instance.Money:N2}";
    }
}