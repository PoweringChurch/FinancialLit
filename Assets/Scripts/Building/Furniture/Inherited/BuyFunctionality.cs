using UnityEngine;
using System;
public class BuyFunctionality : BaseFunctionality
{
    public FurnitureData furniture;
    public int cost;
    protected override void Awake()
    {
        immovable = true;
        base.Awake();
        actions[$"Purchase ({cost})"] = Purchase;
    }
    protected override void Purchase()
    {
        InventoryHelper.Instance.AddItem(furniture,1);
    }
}
