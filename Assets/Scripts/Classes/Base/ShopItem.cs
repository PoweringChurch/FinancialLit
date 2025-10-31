using UnityEngine;

public class ShopItem
{
    public InventoryItem item;
    public int price;
    public ShopItem(InventoryItem shopitem, int buyprice)
    {
        item = shopItem;
        price = buyprice;
    }
}
