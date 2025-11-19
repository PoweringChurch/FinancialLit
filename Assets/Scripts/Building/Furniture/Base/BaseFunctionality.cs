using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

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
            shoppingActions["Buy"] = Buy;
        }
    }
    protected virtual void Move()
    {
        PlacementHandler handler = GetComponent<PlacementHandler>();
        var item = FurnitureDatabase.GetData(handler.itemName);
        InventoryHelper.Instance.AddItem(item, 1);
        FurniturePlacer.Instance.isMoving = true;
        FurniturePlacer.Instance.OverrideRotation(transform.rotation);
        FurniturePlacer.Instance.SetCurrentFurniture(handler.itemName);
        Destroy(gameObject);
    }
    protected virtual void Remove()
    {
        PlacementHandler handler = GetComponent<PlacementHandler>();
        var item = FurnitureDatabase.GetData(handler.itemName);
        InventoryHelper.Instance.AddItem(item, 1);
        Debug.Log("remove");
        Destroy(gameObject);
    }
    protected virtual void Buy()
    {
        PlacementHandler handler = GetComponent<PlacementHandler>();
        var item = FurnitureDatabase.GetData(handler.itemName);
        InventoryHelper.Instance.AddItem(item, 1);
        PlayerResources.Instance.Spend(price, "Furniture");
    }
    protected void Message(string message)
    {
        GameObject textObj = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 0.5f - (Camera.main.transform.forward*2), Camera.main.transform.rotation);
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
    public Dictionary<string, Action> GetAvailableActions()
    {
        bool home = PlayerFlagManager.HasState(PlayerState.Home);
        bool shopping = PlayerFlagManager.HasState(PlayerState.Shopping);

        // Order matters, has to be consistent
        var availableActions = new Dictionary<string, Action>();

        availableActions.AddRange(globalActions);
        if (home) availableActions.AddRange(homeActions);
        if (shopping) availableActions.AddRange(shoppingActions);

        return availableActions;
    }
    protected virtual bool DefaultChecks()
    {
        if (PetBehaviour.Instance.gameObject.activeInHierarchy == false)
        {
            return true;
        }
        if (!PetStateMachine.IsInState(PetState.Idle))
        {
            Message($"{PetStats.Instance.PetName} is occupied!");
            return true;
        }
        return false;
    }
    public Vector3 PositionPetY()  { return new Vector3(transform.position.x, 1, transform.position.z);}
}