using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

using PrimeTween;

[RequireComponent(typeof(PlacementData))]
public class BaseFunctionality : MonoBehaviour
{
    protected AudioClip purchaseSfx; // the purchasing sfx played when user buys a piece of furniture
    // add global actions, and then other actions for other states
    protected Dictionary<string, Action> globalActions = new();
    protected Dictionary<string, Action> homeActions = new();
    protected Dictionary<string, Action> shoppingActions = new();

    protected GameObject floatingTextPrefab; // prefab for floating text, used in Message function
    protected bool ignoreBase = false; // hide base actions
    public float price = 30f; // price of an item, 30f is placeholder
    protected virtual void Awake()
    {
        // assign variables using resources api
        floatingTextPrefab = Resources.Load<GameObject>("UITemplates/Message");
        purchaseSfx = Resources.Load<AudioClip>("SoundFX/Notification/purchase");
        // check if the ignoreBase variable was not overridden
        if (!ignoreBase)
        {
            // assign base actions that the furniture can have
            homeActions["Move"] = Move;
            homeActions["Remove"] = Remove;
            shoppingActions["Buy"] = Buy;
        }
    }
    // allows user to move the piece of furniture; base action
    protected virtual void Move()
    {
        PlacementData handler = GetComponent<PlacementData>();
        // move object
        var item = FurnitureDatabase.GetData(handler.itemName);
        
        InventoryHelper.Instance.AddItem(item, 1); // add the item back into inventory so furniture can be placed
        PlacementManager.Instance.Furniture.isMoving = true; // set moving on the furnitureplacer
        PlacementManager.Instance.Furniture.OverrideRotation(transform.rotation); // override the player's furniture rotation for a more natural experience
        PlacementManager.Instance.SetMode(PlacementManager.Mode.Furniture);

        PlacementManager.Instance.Furniture.SetCurrentFurniture(handler.itemName); // set the current active furniture to the object that is being moved

        // queue previously set down furniture for destruction so that the player cannot duplicate items
        Destroy(gameObject);
    }
    // removes selected piece of furniture; base action
    protected virtual void Remove()
    {
        PlacementData handler = GetComponent<PlacementData>();
        // remove object
        var item = FurnitureDatabase.GetData(handler.itemName);
        InventoryHelper.Instance.AddItem(item, 1);

        // queue previously placed furniture for destruction so that the player cannot duplicate items
        Destroy(gameObject);
    }
    // buy the furniture object, only for use when the player is in a shop; base action
    protected virtual void Buy()
    {
        // check if player can afford
        if (!FinancialSpending.Instance.CanAfford(price)) return;
        // play purchase sfx
        SFXPlayer.Instance.Play(purchaseSfx);
        // add item to inventory
        PlacementData handler = GetComponent<PlacementData>();
        var item = FurnitureDatabase.GetData(handler.itemName);
        InventoryHelper.Instance.AddItem(item, 1);
        // spend money
        if (!FinancialSpending.Instance.CanAfford(price)) return;
        FinancialSpending.Instance.Spend(price, "Home decor");
    }
    // helper functions
    public Dictionary<string, Action> GetAvailableActions() 
    {
        // get player states
        bool home = PlayerFlagManager.HasFlag(PlayerFlag.Home);
        bool shopping = PlayerFlagManager.HasFlag(PlayerFlag.Shopping);

        // order matters, has to be consistent
        var availableActions = new Dictionary<string, Action>();

        // add to returned dict based on states
        availableActions.AddRange(globalActions);
        if (home) availableActions.AddRange(homeActions);
        if (shopping) availableActions.AddRange(shoppingActions);

        return availableActions;
    }
    // default checks for use in derived classes, returns true if check if failed. most furniture objects dont use this
    // to be removed
    protected virtual bool DefaultChecks()
    {
        // check if pet is idle
        if (!PetHelper.petStateMachine.IsInState(PetState.Idle))
        {
            PlacementUtils.Message($"{PetHelper.petStats.petName} is occupied!", transform.position);
            return true;
        }
        return false;
    }
    // shorthand for getting the position of the furniture on the pet's y axis
    public Vector3 PositionPetY()  { return new Vector3(transform.position.x, 1, transform.position.z);}
}