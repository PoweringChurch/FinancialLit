using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR;
using TMPro;
using System.Collections;
using System.Linq;

public class BaseFunctionality : MonoBehaviour
{
    //add global actions, and then other actions for statuses
    protected Dictionary<string, Action> globalActions = new();
    protected Dictionary<string, Action> homeActions = new();
    protected Dictionary<string, Action> shoppingActions = new();

    protected GameObject floatingTextPrefab;
    protected bool ignoreBase = false; //hide base actions
    public float price = 0.15f;
    protected virtual void Awake()
    {
        floatingTextPrefab = Resources.Load<GameObject>("UITemplates/Message");
        if (!ignoreBase)
        {
            homeActions["Move"] = Move;
            homeActions["Remove"] = Remove;
            shoppingActions["Purchase"] = Purchase;
        }
    }
    protected virtual void Move()
    {
        PlacementHandler handler = GetComponent<PlacementHandler>();
        var item = FurnitureDatabase.GetItem(handler.itemName);
        InventoryHelper.Instance.AddItem(item, 1);
        FurniturePlacer.Instance.isMoving = true;
        FurniturePlacer.Instance.SetCurrentFurniture(handler.itemName);
        Destroy(gameObject);
    }
    protected virtual void Remove()
    {
        PlacementHandler handler = GetComponent<PlacementHandler>();
        var item = FurnitureDatabase.GetItem(handler.itemName);
        InventoryHelper.Instance.AddItem(item, 1);
        Debug.Log("remove");
        Destroy(gameObject);
    }
    protected virtual void Purchase()
    {
        PlacementHandler handler = GetComponent<PlacementHandler>();
        var item = FurnitureDatabase.GetItem(handler.itemName);
        InventoryHelper.Instance.AddItem(item, 1);
        PlayerResources.Instance.Spend(price, "Furniture");
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
    public Dictionary<string,Action> GetAvailableActions()
    {
        bool home = PlayerStateManager.HasState(PlayerState.Home);
        bool shopping = PlayerStateManager.HasState(PlayerState.Shopping);

        // Order matters, has to be consistent
        var availableActions = new Dictionary<string, Action>();
        
        availableActions.AddRange(globalActions);
        if (home) availableActions.AddRange(homeActions);
        if (shopping) availableActions.AddRange(shoppingActions);
        
        return availableActions;
    }
}