using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class ObjectPlacer : MonoBehaviour
{
      //https://github.com/MinaPecheux/unity-tutorials/tree/main/Assets/07-BuildingPlacement
      //following this tut (sorta gonna mod a bit)
      public static ObjectPlacer instance; //singleton pattern, this is fine because there will only ever be one of these present at any given time
      public LayerMask groundLayerMask;
      public Camera gameCamera;

      private GameObject _objectPrefab;
      private GameObject _toBuild;
      
      private Ray _ray;
      private RaycastHit _hit;
      
      private float cellSize = 1f;
      private Vector2 gridOffset = new(1f,1f);

      private Vector3 previewOffset = new(0,0.5f,0);
      void Awake()
      {
            instance = this;
            _objectPrefab = null;
      }
      void Update()
      {
            //if we have something to place
            if (_objectPrefab != null)
            {
                  // cancel build mode (upd later to work with system)
                  if (Keyboard.current.qKey.wasPressedThisFrame)
                  {
                        Destroy(_toBuild);
                        _toBuild = null;
                        _objectPrefab = null;
                        return;
                  }
                  //rotate (upd later to work with system)
                  if (Keyboard.current.rKey.wasPressedThisFrame)
                  {
                        _toBuild.transform.Rotate(Vector3.up, 90);
                  }
                  _ray = gameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                  //if player is hover over placement
                  if (Physics.Raycast(_ray, out _hit, 1000f, groundLayerMask))
                  {
                        if (!_toBuild.activeSelf) _toBuild.SetActive(true);
                        //move building to mouse
                        _toBuild.transform.position = previewOffset + _ClampToNearest(_hit.point, cellSize);
                        //place (upd later to work with system)
                        if (Keyboard.current.eKey.wasPressedThisFrame)
                        {
                              PlacementHandler handler = _toBuild.GetComponent<PlacementHandler>();
                              _toBuild.transform.position = _hit.point;
                              _toBuild = null; // (to avoid destruction)
                              _PrepareObject();
                        }
                  }
            }
      }

      public void SetObjectPrefab(GameObject setPrefab)
      {
            _objectPrefab = setPrefab;
            _PrepareObject();
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
      protected virtual void _PrepareObject()
      {
            //just in case
            if (_toBuild) Destroy(_toBuild);
            _toBuild = Instantiate(_objectPrefab);
            _toBuild.SetActive(false);
            //all objects should have placement handler attached
            PlacementHandler m = _toBuild.GetComponent<PlacementHandler>();
            m.isFixed = false;
      }
}
