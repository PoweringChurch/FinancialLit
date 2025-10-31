using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryUI : MonoBehaviour
{
    public GameObject itemButtonTemplate;
    public Transform contentTransform;
    public Inventory inventory;
    public ObjectPlacer objectPlacer;

    private Dictionary<string, GameObject> inventoryItemUI = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in inventory.GetInventoryItemsToDisplay())
        {
            GameObject newTemplate = Instantiate(itemButtonTemplate, contentTransform);
            Button itemButton = newTemplate.GetComponent<Button>();
            Transform displayNameText = newTemplate.transform.Find("DisplayName");
            Transform countText = newTemplate.transform.Find("Count");

            displayNameText.GetComponent<TextMeshProUGUI>().text = item.displayName;
            countText.GetComponent<TextMeshProUGUI>().text = $"{item.Count}";
            itemButton.onClick.AddListener(() => OnItemButtonClicked(item));
            inventoryItemUI.Add(item.displayName, newTemplate);
        }
    }
    public void UpdateInventoryItem(string itemname)
    {
        InventoryItem item = Inventory.GetItem(itemname);
        GameObject itemUi = inventoryItemUI[itemname];
        if (item == null)
        {
            Debug.Log("THERE IS NO ITEM AHHH AHHHHHH");
            Destroy(itemUi); //no idea if this works?
            return;
        }
        Transform countText = newTemplate.transform.Find("Count");
        countText.GetComponent<TextMeshProUGUI>().text = $"{item.Count}";
        
    }
    private void OnItemButtonClicked(InventoryItem item)
    {
        objectPlacer.SetObjectPrefab(item.furnitureGameObject);
    }
}
