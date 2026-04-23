using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[Serializable]
public class FurnitureObjectData
{
    public Vector3 position;
    public Quaternion rotation;
    public string itemName; // to know which prefab to spawn

    // furniture-specific data
    public bool isFilled = false; // for food bowl
}

public class FurniturePlacement : MonoBehaviour
{
      // made by modifying this tutorial
      // https://github.com/MinaPecheux/unity-tutorials/tree/main/Assets/07-BuildingPlacement
      public Transform furnitureHolder;
      public Transform minydisplay;
      public AudioClip placeSfx;

      [HideInInspector] public GameObject _objectPrefab;
      private GameObject _toBuild;
      private PlacementData _handler;
      public LayerMask furniturePlacementMask;

      // grid placement
      private const float cellSize = 0.25f;
      private Vector3 gridOffset = new (.25f,0,.25f);
      private bool freemove = false;

      // y placement
      private const float minyoffset = 0f;
      private const float maxyoffset = 3f;
      private float currentyoffset = 0f;

      private Quaternion previousRotation;
      [HideInInspector] public bool isMoving = false;
      public event Action<string> OnItemPlaced;
      
      public void Tick(RaycastHit hit)
      {
            if (!_objectPrefab) return;
            
            if (!_toBuild.activeSelf) 
                  _toBuild.SetActive(true);
            if (freemove)
            {
                  _toBuild.transform.position = new Vector3(hit.point.x, minyoffset, hit.point.z);
                  minydisplay.position = new Vector3(hit.point.x, minyoffset + 0.02f, hit.point.z);
            }
            else
            {
                  _toBuild.transform.position = PlacementUtils.ClampToNearest(hit.point, cellSize, gridOffset);
                  minydisplay.position = new Vector3(_toBuild.transform.position.x, minyoffset + 0.02f, _toBuild.transform.position.z);
            }

            // clamp offset between 0 and max, base y is already minyoffset from both branches
            _toBuild.transform.position += new Vector3(0, Math.Clamp(currentyoffset, 0, maxyoffset - minyoffset), 0);
      }
      public void LoadFurniture(FurnitureObjectData[] furnitureData)
      {
            // clear existing furniture
            for (int i = furnitureHolder.childCount - 1; i >= 0; i--)
                  Destroy(furnitureHolder.GetChild(i).gameObject);
            if (furnitureData == null)
                  return;
            // spawn saved furniture
            foreach (var furniture in furnitureData)
            {
                  FurnitureData furnitureItem = FurnitureDatabase.GetData(furniture.itemName);
                  if (furnitureItem == null)
                        continue;

                  GameObject spawnedFurniture = Instantiate(furnitureItem.prefab, furnitureHolder);
                  spawnedFurniture.transform.SetPositionAndRotation(furniture.position, furniture.rotation);

                  // restore furniture data
                  var functionality = spawnedFurniture.GetComponent<BaseFunctionality>();
                  var placementHandler = spawnedFurniture.GetComponent<PlacementData>();
                  placementHandler.SetPlacementMode(PlacementData.State.Fixed);

                  if (functionality is FeedingFunctionality feedingFunctionality)
                        feedingFunctionality.SetFilled(furniture.isFilled);
            }
      }
      public FurnitureObjectData[] GetPlacedFurniture()
      {
            FurnitureObjectData[] placedFurnitureData = new FurnitureObjectData[furnitureHolder.childCount];
            for (int i = 0; i < furnitureHolder.childCount; i++)
            {
                  var childTransform = furnitureHolder.GetChild(i);
                  var placementHandler = childTransform.GetComponent<PlacementData>();
                  if (placementHandler == null) continue; // skip if no PlacementHandler

                  FurnitureObjectData newFurnitureObjData = new()
                  {
                        position = childTransform.position,
                        rotation = childTransform.rotation,
                        itemName = placementHandler.itemName
                  };

                  var childFunctionality = childTransform.GetComponent<BaseFunctionality>();
                  if (childFunctionality is FeedingFunctionality feedingFunctionality)
                        newFurnitureObjData.isFilled = feedingFunctionality.filled;
                  placedFurnitureData[i] = newFurnitureObjData;
            }
            return placedFurnitureData;
      }
      // places the current object
      public void TryPlace(LayerMask placementLayerMask)
      {
            // check if we have an object to place, and if the object has a valid position
            if (_objectPrefab == null || !_handler.hasValidPlacement) 
                  return;
            string itemName = _handler.itemName;
            var groundRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(groundRay, out RaycastHit hit, 1000f, placementLayerMask)) return;
            // remove the item from the players inventory            
            InventoryHelper.Instance.RemoveItem(itemName, 1);
            // set the placement mode of the object were placing to fixed
            _handler.SetPlacementMode(PlacementData.State.Fixed);
            // if were in free move make it not snap
            if (freemove)
                  _toBuild.transform.position = new Vector3(hit.point.x,0,hit.point.z);
            else // else make it snap
                  _toBuild.transform.position = PlacementUtils.ClampToNearest(hit.point, cellSize, gridOffset);
            _toBuild.transform.position += new Vector3(0,Math.Clamp(currentyoffset,minyoffset,maxyoffset),0);

            // trigger the event
            OnItemPlaced?.Invoke(itemName);
            // play sfx
            UISFXPlayer.Instance.Play(placeSfx);
            
            _toBuild = null;
            _PrepareObject();
            if (isMoving)
                  { isMoving = false; PlacementManager.Instance.SetMode(PlacementManager.Mode.None); Cancel(); }
            if (!InventoryHelper.Instance.GetInventory().HasItem(_handler.itemName) || isMoving) // if the item isnt in players inventory
                  Cancel(); // cancel the placement
      }

      // set the current furniture to the provided item name
      public void SetCurrentFurniture(string itemName)
      {
            FurnitureData furniture = FurnitureDatabase.GetData(itemName);
            if (!furniture) // validate
                  return;
            _objectPrefab = furniture.prefab;
            minydisplay.gameObject.SetActive(true);
            _PrepareObject();
      }
      // cancels the placement of the current object
      public void Cancel()
      {
            if (_objectPrefab == null) return; // if were not placing anything, return
            // destroy what were about to build
            Destroy(_toBuild);
            // set isMoving to false
            if (isMoving)
                  { isMoving = false; PlacementManager.Instance.SetMode(PlacementManager.Mode.None); }
            // dereference
            _toBuild = null; 
            _objectPrefab = null;
            // set y offset to min and hide the display for y min
            currentyoffset = minyoffset;
            minydisplay.gameObject.SetActive(false);
      }
      // rotates the furniture
      public void RotateFurniture()
      {
            if (_objectPrefab == null) return;
            _toBuild.transform.Rotate(Vector3.up, 90f);
            previousRotation = _toBuild.transform.rotation;
      }
     
      public void OverrideRotation(Quaternion quaternion) { previousRotation = quaternion; }
      public void AddYOffset(float delta) { currentyoffset = Math.Clamp(currentyoffset+delta,minyoffset,maxyoffset); }
      public void SetFreemove(bool to) { freemove = to; }
      
      // prepares the selected object to be placed
      private void _PrepareObject()
      {
            // just in case
            if (_toBuild) Destroy(_toBuild);
            _toBuild = Instantiate(_objectPrefab, furnitureHolder);
            _toBuild.transform.rotation = previousRotation;
            _toBuild.SetActive(false);

            // all objects should have placement handler attached
            _handler = _toBuild.GetComponent<PlacementData>();
            _handler.SetPlacementMode(PlacementData.State.Valid);
            _handler.isFixed = false;
      }
      // checks if a given item is placed, only used during the tutorial
      public bool IsItemPlaced(string itemName) { 
            if (furnitureHolder == null) return false;
            // loop through all children under furnitureHolder
            foreach (Transform child in furnitureHolder)
            {
                  PlacementData handler = child.GetComponent<PlacementData>();
                  if (handler != null && handler.itemName == itemName && handler.isFixed)
                        return true;
            }
            return false;
      }
}
