using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class FurniturePlacer : MonoBehaviour
{
      // core of furniture placement, made by modifying this tutorial
      // https://github.com/MinaPecheux/unity-tutorials/tree/main/Assets/07-BuildingPlacement

      public static FurniturePlacer Instance; // singleton
      public LayerMask placementLayerMask;
      public LayerMask groundLayerMask;
      public Camera gameCamera;
      public Transform furnitureHolder;
      public Transform minydisplay;
      public AudioClip placeSfx;
      
      [HideInInspector] public GameObject _objectPrefab; // honestly could make a bool property that just checks if this exists
      private GameObject _toBuild;
      private PlacementHandler _handler;

      private Ray _ray;
      private RaycastHit _hit;

      // grid placement
      private const float cellSize = 0.25f;
      private Vector2 gridOffset = new();
      private bool freemove = false;

      // y placement
      private const float minyoffset = 0.5f;
      private const float maxyoffset = 3f;
      private float currentyoffset = 0.5f;

      private Quaternion previousRotation;
      [HideInInspector] public bool isMoving = false;
      [HideInInspector] public bool onPlacement = false;
      public event Action<string> OnItemPlaced;

      void Awake()
      {
            Instance = this;
            _objectPrefab = null;
      }
      void Update()
      {
            if (!_objectPrefab) return;
            onPlacement = false;
            _ray = gameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            // if player is hovering over placement
            if (Physics.Raycast(_ray, out _hit, 1000f, placementLayerMask))
            {
                  // set active on first frame
                  if (!_toBuild.activeSelf) _toBuild.SetActive(true);
                  if (freemove)
                  {
                        _toBuild.transform.position = new Vector3(_hit.point.x,currentyoffset,_hit.point.z);
                        minydisplay.position = new Vector3(_hit.point.x, minyoffset+0.02f, _hit.point.z);
                  }
                  else
                  {
                        _toBuild.transform.position = ClampToNearest(_hit.point, cellSize);
                        minydisplay.position = new Vector3(_toBuild.transform.position.x, minyoffset+0.02f, _toBuild.transform.position.z);
                  }
                  onPlacement = true;
            }
      }
      // places the current object
      public void Place()
      {
            // check if cursor over ui
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                  return;
            // check if we have an object to place, and if the object has a valid position
            if (_objectPrefab == null || !_handler.hasValidPlacement) 
                  return;
            string itemName = _handler.itemName;

            // remove the item from the players inventory            
            InventoryHelper.Instance.RemoveItem(itemName, 1);
            // set the placement mode of the object were placing to fixed
            _handler.SetPlacementMode(PlacementMode.Fixed);
            // if were in free move make it not snap
            if (freemove)
                  _toBuild.transform.position = new Vector3(_hit.point.x,currentyoffset,_hit.point.z);
            else // else make it snap
                  _toBuild.transform.position = ClampToNearest(_hit.point, cellSize);
            
            // trigger the event
            OnItemPlaced?.Invoke(itemName);
            // play sfx
            UISFXPlayer.Instance.Play(placeSfx);
            
            _toBuild = null;
            _PrepareObject();
            if (!InventoryHelper.Instance.GetInventory().HasItem(_handler.itemName) || isMoving) // if the item isnt in players inventory
                  CancelPlacement(); // cancel the placement
      }

      // set the current furniture to the provided item name
      public void SetCurrentFurniture(string itemName)
      {
            FurnitureData furniture = FurnitureDatabase.GetData(itemName);
            if (!furniture) // validate
                  return;
            _objectPrefab = furniture.prefab;
            minydisplay.gameObject.SetActive(true);
            PlayerFlagManager.AddFlag(PlayerFlag.Placement);
            _PrepareObject();
      }
      // cancels the placement of the current object
      public void CancelPlacement()
      {
            if (_objectPrefab == null) return; // if were not placing anything, return
            // destroy what were about to build
            Destroy(_toBuild);
            // set isMoving to false
            isMoving = false;
            // dereference (Destroy function only queues for destruction)
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
      // dont think this is ever called? might remove
      public void OverrideRotation(Quaternion quaternion) { previousRotation = quaternion; }
      public void AddYOffset(float delta) { currentyoffset = Math.Clamp(currentyoffset+delta,minyoffset,maxyoffset); }
      public void SetFreemove(bool to) { freemove = to; }
      private Vector3 ClampToNearest(Vector3 pos, float threshold)
      {
            float t = 1f / threshold;
            Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;

            float s = threshold * 0.5f;
            v.x += s + gridOffset.x; // (recenter in middle of cells)
            v.y = currentyoffset;
            v.z += s + gridOffset.y;

            return v;
      }
      // prepares the selected object to be placed
      private void _PrepareObject()
      {
            // just in case
            if (_toBuild) Destroy(_toBuild);
            _toBuild = Instantiate(_objectPrefab, furnitureHolder);
            _toBuild.transform.rotation = previousRotation;
            _toBuild.SetActive(false);

            // all objects should have placement handler attached
            _handler = _toBuild.GetComponent<PlacementHandler>();
            _handler.SetPlacementMode(PlacementMode.Valid);
            _handler.isFixed = false;
      }
      // checks if a given item is placed, pretty sure this is only used during the tutorial?
      public bool IsItemPlaced(string itemName) { 
            if (furnitureHolder == null) return false;
            // loop through all children under furnitureHolder
            foreach (Transform child in furnitureHolder)
            {
                  PlacementHandler handler = child.GetComponent<PlacementHandler>();
                  if (handler != null && handler.itemName == itemName && handler.isFixed)
                        return true;
            }
            return false;
      }
}
