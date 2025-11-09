using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class FurniturePlacer : MonoBehaviour
{
      //https://github.com/MinaPecheux/unity-tutorials/tree/main/Assets/07-BuildingPlacement
      public static FurniturePlacer Instance; //singleton pattern, this is fine because there will only ever be one of these present at any given time
      public LayerMask placementLayerMask; //ignore
      public LayerMask groundLayerMask;
      public Camera gameCamera;
      public Transform furnitureHolder;
      private GameObject _objectPrefab;
      private GameObject _toBuild;
      private PlacementHandler _handler;

      private Ray _ray;
      private RaycastHit _hit;

      private float cellSize = 1f;
      private Vector2 gridOffset = new(0.5f, 0.5f);

      private Vector3 previewOffset = new(0, 0.5f, 0);
      private Quaternion previousRotation;
      [HideInInspector] public bool isMoving = false;
      [HideInInspector] public bool onPlacement = false;
      void Awake()
      {
            Instance = this;
            _objectPrefab = null;
      }
      void Update()
      {
            if (_objectPrefab == null) return;
            onPlacement = false;
            _ray = gameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            //if player is hover over placement
            if (Physics.Raycast(_ray, out _hit, 1000f, placementLayerMask))
            {
                  //set active on first frame
                  if (!_toBuild.activeSelf) _toBuild.SetActive(true);
                  _toBuild.transform.position = previewOffset + _ClampToNearest(_hit.point, cellSize);
                  onPlacement = true;
            }
      }
      public void Place()
      {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                  return;
            }
            if (_objectPrefab == null || !_handler.hasValidPlacement) return;

            InventoryHelper.Instance.RemoveItem(_handler.itemName, 1);
            _handler.SetPlacementMode(PlacementMode.Fixed);
            _toBuild.transform.position = previewOffset + _ClampToNearest(_hit.point, cellSize);
            _toBuild = null;
            _PrepareObject();
            if (!InventoryHelper.Instance.GetInventory().HasItem(_handler.itemName) || isMoving) //if the item isnt in players inventory
            {
                  CancelPlacement();
                  return;
            }
      }
      public void SetCurrentFurniture(string itemName)
      {
            _objectPrefab = FurnitureDatabase.GetItem(itemName).prefab;
            PlayerStateManager.AddState(PlayerState.Placement);
            _PrepareObject();
      }
      public void CancelPlacement()
      {
            if (_objectPrefab == null) return;
            Destroy(_toBuild);
            isMoving = false;
            _toBuild = null;
            _objectPrefab = null;
            PlayerStateManager.RemoveState(PlayerState.Placement);
      }
      public void RotateFurniture()
      {
            if (_objectPrefab == null) return;
            _toBuild.transform.Rotate(Vector3.up, 90f);
            previousRotation = _toBuild.transform.rotation;
      }
      public void OverrideRotation(Quaternion quaternion)
    {
            previousRotation = quaternion;
    }
      private Vector3 _ClampToNearest(Vector3 pos, float threshold)
      {
            float t = 1f / threshold;
            Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;

            float s = threshold * 0.5f;
            v.x += s + gridOffset.x; // (recenter in middle of cells)
            v.y = 0;
            v.z += s + gridOffset.y;

            return v;
      }
      public GameObject pivotMarkerPrefab; // Assign a simple sphere/cube in Inspector
      private void _PrepareObject()
      {
            //just in case
            if (_toBuild) Destroy(_toBuild);
            _toBuild = Instantiate(_objectPrefab, furnitureHolder);
            _toBuild.transform.rotation = previousRotation;
            _toBuild.SetActive(false);
            //all objects should have placement handler attached
            _handler = _toBuild.GetComponent<PlacementHandler>();
            _handler.SetPlacementMode(PlacementMode.Valid);
            _handler.isFixed = false;
      }
}
