using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR;

[RequireComponent(typeof(PlacementHandler))]
public class BaseFunctionality : MonoBehaviour
{
    protected Dictionary<string, Action> actions = new();
    protected bool immovable = false; //hide base actions
    protected virtual void Awake()
    {
        if (actions == null)
            actions = new Dictionary<string, Action>();
        if (!immovable)
        {
            actions["Move"] = Move;
            actions["Remove"] = Remove;
        }
    }
    protected virtual void Move()
    {
        PlacementHandler handler = GetComponent<PlacementHandler>();
        var item = ItemDatabase.GetItem(handler.itemName);
        Destroy(gameObject);
        InventoryManager.Instance.AddItem(item, 1);
        FurniturePlacer.Instance.isMoving = true;
        FurniturePlacer.Instance.SetCurrentFurniture(handler.itemName);
    }

    protected virtual void Remove()
    {
        PlacementHandler handler = GetComponent<PlacementHandler>();
        var item = ItemDatabase.GetItem(handler.itemName);
        Destroy(gameObject);
        InventoryManager.Instance.AddItem(item, 1);
    }
    //helpers
    public void InvokeAction(string actionName)
    {
        if (actions.TryGetValue(actionName, out Action action))
            action.Invoke();
        else
            Debug.LogWarning($"Action '{actionName}' not found");
    }
    public IEnumerable<string> GetAvailableActions()
    {
        return actions.Keys;
    }
}