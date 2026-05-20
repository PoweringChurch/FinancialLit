using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[Serializable]
public class FinanceUI : MonoBehaviour
{
    public static FinanceUI Instance;
    public GameObject itemPrefab;
    public Transform spendingHolder;
    public Transform earningHolder;
    
    private Dictionary<string, GameObject> spendingItems = new();
    private Dictionary<string, GameObject> earningItems = new();
    void Start()
    {
        Instance = this;
        foreach (var kvp in FinancialSpending.Instance.spending)
        {
            GameObject spendingItem = Instantiate(itemPrefab, spendingHolder);
            var tmp = spendingItem.GetComponentsInChildren<TextMeshProUGUI>();
            tmp[0].text = kvp.Key;
            tmp[1].text = "$0.00";
            spendingItems.Add(kvp.Key, spendingItem);
        }
        foreach (var kvp in FinancialSpending.Instance.earning)
        {
            GameObject earningItem = Instantiate(itemPrefab, earningHolder);
            var tmp = earningItem.GetComponentsInChildren<TextMeshProUGUI>();
            tmp[0].text = kvp.Key;
            tmp[1].text = "$0.00";
            earningItems.Add(kvp.Key, earningItem);
        }
    }
    public TextMeshProUGUI sumSpendingsTmp;
    public TextMeshProUGUI sumEarningsTmp;
    public TextMeshProUGUI netIncomeTmp;

    public void UpdateItem(string key, bool isSpending = true) 
    {
        if (isSpending)
            spendingItems[key].GetComponentsInChildren<TextMeshProUGUI>()[1].text = $"{FinancialSpending.Instance.spending[key]:C}";
        else
            earningItems[key].GetComponentsInChildren<TextMeshProUGUI>()[1].text = $"{FinancialSpending.Instance.earning[key]:C}";
        float sumSpendings = 0;
        float sumEarnings = 0;
        foreach (var kvp in FinancialSpending.Instance.spending)
            sumSpendings += kvp.Value;
        foreach (var kvp in FinancialSpending.Instance.earning)
            sumEarnings += kvp.Value;
        
        sumSpendingsTmp.text = $"Total Spendings: {sumSpendings:C}";
        sumEarningsTmp.text = $"Total Earnings: {sumEarnings:C}";
        string netString = $"{sumEarnings-sumSpendings:C}";
        if (sumEarnings-sumSpendings < 0)
            netString = "-"+netString;
        netIncomeTmp.text = "Net Income: "+netString;
    }
    public void UpdateAll()
    {
        float sumSpendings = 0;
        foreach (var kvp in FinancialSpending.Instance.spending) {
            spendingItems[kvp.Key].GetComponentsInChildren<TextMeshProUGUI>()[1].text = $"{FinancialSpending.Instance.spending[kvp.Key]:C}";
            sumSpendings += kvp.Value;
        }
        float sumEarnings = 0;
        foreach (var kvp in FinancialSpending.Instance.earning) {
            earningItems[kvp.Key].GetComponentsInChildren<TextMeshProUGUI>()[1].text = $"{FinancialSpending.Instance.earning[kvp.Key]:C}";
            sumEarnings += kvp.Value;
        }
        sumSpendingsTmp.text = $"Total Spendings: {sumSpendings:C}";
        sumEarningsTmp.text = $"Total Earnings: {sumEarnings:C}";
        string netString = $"{sumEarnings-sumSpendings:C}";
        if (sumEarnings-sumSpendings < 0)
            netString = "-"+netString;
        netIncomeTmp.text = "Net Income: "+netString;
    }
}