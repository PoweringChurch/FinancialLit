using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR;
using TMPro;
using System.Collections;
[RequireComponent(typeof(PlacementHandler))]
public class BaseFunctionality : MonoBehaviour
{
    protected Dictionary<string, Action> actions = new();
    protected Dictionary<string, Action> shoppingActions = new();
    protected GameObject floatingTextPrefab;
    protected bool immovable = false; //hide base actions
    public float price;
    protected virtual void Awake()
    {
        floatingTextPrefab = Resources.Load<GameObject>("UITemplates/Message");
        if (actions == null)
            actions = new Dictionary<string, Action>();
            shoppingActions = new Dictionary<string, Action>();
        if (!immovable)
        {
            actions["Move"] = Move;
            actions["Remove"] = Remove;
        }
        shoppingActions[$"Purchase ({price})"] = Purchase;
    }
    protected virtual void Move()
    {
        PlacementHandler handler = GetComponent<PlacementHandler>();
        var item = FurnitureDatabase.GetItem(handler.itemName);
        Destroy(gameObject);
        InventoryHelper.Instance.AddItem(item, 1);
        FurniturePlacer.Instance.isMoving = true;
        FurniturePlacer.Instance.SetCurrentFurniture(handler.itemName);
    }
    protected virtual void Remove()
    {
        PlacementHandler handler = GetComponent<PlacementHandler>();
        var item = FurnitureDatabase.GetItem(handler.itemName);
        Destroy(gameObject);
        InventoryHelper.Instance.AddItem(item, 1);
    }
    protected virtual void Purchase()
    {
        PlacementHandler handler = GetComponent<PlacementHandler>();
        var item = FurnitureDatabase.GetItem(handler.itemName);
        InventoryHelper.Instance.AddItem(item, 1);
    }
    protected void Message(string message)
    {
        GameObject textObj = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 0.5f, Camera.main.transform.rotation);
        TextMeshPro tmp = textObj.GetComponent<TextMeshPro>();
        tmp.text = message;

        StartCoroutine(AnimateMessage(textObj, tmp));
    }
    private IEnumerator AnimateMessage(GameObject textObj, TextMeshPro tmp)
    {
        float duration = 1.5f;
        float riseSpeed = 2f;
        float elapsed = 0f;
        
        Vector3 startPos = textObj.transform.position;
        Color startColor = tmp.color;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            textObj.transform.position = startPos + Vector3.up * (riseSpeed * elapsed);
            
            Color color = startColor;
            color.a = 1f - t;
            tmp.color = color;
            
            textObj.transform.rotation = Camera.main.transform.rotation;
            
            yield return null;
        }
        Destroy(textObj);
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
    public IEnumerable<string> GetShoppingActions()
    {
        return shoppingActions.Keys;
    }
}