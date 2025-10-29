using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPlacer : MonoBehaviour
{
      //https://github.com/MinaPecheux/unity-tutorials/tree/main/Assets/07-BuildingPlacement
      //following this tut (sorta gonna mod a bit)
      public static ObjectPlacer instance; //singleton pattern, this is fine because there will only ever be one of these present at any given time
      private GameObject _objectPrefab;
      private GameObject _toBuild;

      private Camera _gameCamera;

      private Ray _ray;
      private RaycastHit _hit;
      
      void Awake()
      {
            instance = this;
            _gameCamera = null; //set this l8r
            _objectPrefab = null;
      }
      void Update()
      {
      }

      public void SetObjectPrefab(GameObject setPrefab)
      {
             _buildingPrefab = setPrefab;
             _PrepareBuilding();
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
            if (_toBuild) Destroy(_toBuild);
            _toBuild = Instantiate(_buildingPrefab);
            _toBuild.SetActive(false);
            
            BuildingManager m = _toBuild.GetComponent<BuildingManager>();
            m.isFixed = false;
      }
}
