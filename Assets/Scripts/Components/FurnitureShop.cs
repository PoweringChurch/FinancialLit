using UnityEngine;
using System.Collections.General;
public class FurnitureShop : MonoBehaviour
{
    public static readonly Dictionary<string, FurnitureShopItem> furnitureStock = new(); //everything there is to buy
    
    public Inventory playerInventory;
    public Player player;
    
    public BuyItem(string itemName, int amount)
    {
        var shopItem = furnitureStock[itemName]
        if (shopItem == null)
        {
            Debug.Log("item does not exist");
        }
        //player.money -= shopItem.price
        //likely just gonna be a "spend" function in player
        playerInventory.AddItem(shopItem.item, amount);
    }
}
