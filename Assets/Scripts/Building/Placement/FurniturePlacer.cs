using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class FurniturePlacer : MonoBehaviour
{
      //https://github.com/MinaPecheux/unity-tutorials/tree/main/Assets/07-BuildingPlacement
      public static FurniturePlacer Instance; 
      public LayerMask placementLayerMask; //ignore
      public LayerMask groundLayerMask;
      public Camera gameCamera;
      public Transform furnitureHolder;
      public Transform minydisplay;
      [HideInInspector] public GameObject _objectPrefab;
      private GameObject _toBuild;
      private PlacementHandler _handler;

      private Ray _ray;
      private RaycastHit _hit;

      private readonly float cellSize = 0.25f;
      private Vector2 gridOffset = new();// = new(0.25f, 0.25f);

      private readonly float minyoffset = 0.5f;
      private readonly float maxyoffset = 3f;
      private float currentyoffset = 0.5f;
      private bool freemove = false;

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
            if (!_objectPrefab) return;
            onPlacement = false;
            _ray = gameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            //if player is hover over placement
            if (Physics.Raycast(_ray, out _hit, 1000f, placementLayerMask))
            {
                  //set active on first frame
                  if (!_toBuild.activeSelf) _toBuild.SetActive(true);
                  if (freemove)
                  {
                        _toBuild.transform.position = new Vector3(_hit.point.x,currentyoffset,_hit.point.z);
                        minydisplay.position = new Vector3(_hit.point.x, minyoffset+0.02f, _hit.point.z);
                  }
                  else
                  {
                        _toBuild.transform.position = _ClampToNearest(_hit.point, cellSize);
                        minydisplay.position = new Vector3(_toBuild.transform.position.x, minyoffset+0.02f, _toBuild.transform.position.z);
                  }
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
            if (freemove)
            {
                  _toBuild.transform.position = new Vector3(_hit.point.x,currentyoffset,_hit.point.z);
            }
            else
            {
                  _toBuild.transform.position = _ClampToNearest(_hit.point, cellSize);
            }
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
            _objectPrefab = FurnitureDatabase.GetData(itemName).prefab;
            minydisplay.gameObject.SetActive(true);
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
            currentyoffset = minyoffset;
            minydisplay.gameObject.SetActive(false);
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
      public void AddYOffset(float delta)
      {
            currentyoffset = Math.Clamp(currentyoffset+delta,minyoffset,maxyoffset);
      }
      public void SetFreemove(bool to)
      {
            freemove = to;
      }
      private Vector3 _ClampToNearest(Vector3 pos, float threshold)
      {
            float t = 1f / threshold;
            Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;

            float s = threshold * 0.5f;
            v.x += s + gridOffset.x; // (recenter in middle of cells)
            v.y = currentyoffset;
            v.z += s + gridOffset.y;

            return v;
      }
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
