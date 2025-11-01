using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class FurniturePlacer : MonoBehaviour
{
      //https://github.com/MinaPecheux/unity-tutorials/tree/main/Assets/07-BuildingPlacement
      public static FurniturePlacer Instance; //singleton pattern, this is fine because there will only ever be one of these present at any given time
      public LayerMask groundLayerMask;
      public LayerMask roomLayerMask;
      public Camera gameCamera;
      private GameObject _objectPrefab;
      private GameObject _toBuild;
      private PlacementHandler _handler;

      private Ray _ray;
      private RaycastHit _hit;

      private float cellSize = 1f;
      private Vector2 gridOffset = new(0f, 0f);

      private Vector3 previewOffset = new(0, 0.5f, 0);
      private int currentRot = 0;
      public bool isMoving = false;
      void Awake()
      {
            Instance = this;
            _objectPrefab = null;
      }
      void Update()
      {
            //if we have something to place
            if (_objectPrefab == null) return;
            // cancel build mode (upd later to work with system)
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                  CancelPlacement();
                  return;
            }
            //rotate (upd later to work with system)
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                  currentRot++;
                  _toBuild.transform.Rotate(Vector3.up, 90);
            }
            _ray = gameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            //if player is hover over placement
            if (Physics.Raycast(_ray, out _hit, 1000f, groundLayerMask))
            {
                  //set active on first frame
                  if (!_toBuild.activeSelf) _toBuild.SetActive(true);
                  _toBuild.transform.position = previewOffset + _ClampToNearest(_hit.point, cellSize);
                  if (Mouse.current.leftButton.wasPressedThisFrame) Place();
            }
      }
      void Place()
      {
            if (_objectPrefab == null || !_handler.hasValidPlacement) return;

            InventoryManager.Instance.RemoveItem(_handler.itemName, 1);
            _handler.SetPlacementMode(PlacementMode.Fixed);
            _toBuild.transform.position = previewOffset + _ClampToNearest(_hit.point, cellSize);
            _toBuild = null;
            _PrepareObject();
            if (!InventoryManager.Instance.GetInventory().HasItem(_handler.itemName) || isMoving) //if the item isnt in players inventory
            {
                  CancelPlacement();
                  return;
            }
      }
      public void SetCurrentFurniture(string itemName)
      {
            _objectPrefab = ItemDatabase.GetItem(itemName).prefab;
            PlayerStateHandler.Instance.SetState(PlayerState.Placement);
            _PrepareObject();
      }
      private void CancelPlacement()
      {
            Destroy(_toBuild);
            isMoving = false;
            _toBuild = null;
            _objectPrefab = null;
            PlayerStateHandler.Instance.SetState(PlayerState.Game);
      }
      private Vector3 _ClampToNearest(Vector3 pos, float threshold)
      {
            float t = 1f / threshold;
            Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;

            float s = threshold * 0.5f;
            v.x += s + gridOffset.x; // (recenter in middle of cells)
            v.z += s + gridOffset.y;

            return v;
      }
      private void _PrepareObject()
      {
            //just in case
            if (_toBuild) Destroy(_toBuild);
            _toBuild = Instantiate(_objectPrefab);
            _toBuild.transform.Rotate(Vector3.up, currentRot * 90);
            _toBuild.SetActive(false);
            //all objects should have placement handler attached
            _handler = _toBuild.GetComponent<PlacementHandler>();
            _handler.SetPlacementMode(PlacementMode.Valid);
            _handler.isFixed = false;
      }
}
