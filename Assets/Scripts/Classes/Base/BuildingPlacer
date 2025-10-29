using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridPlacer : MonoBehaviour
{
      public static BuildingPlacer instance;
      private GameObject _buildingPrefab;
      private GameObject _toBuild;

      private Camera _gameCamera;

      private Ray _ray;
      private RaycastHit _hit;
      
      void Awake()
      {
          instance = this;
          _gameCamera = null; //set this l8r
          _buildingPrefab = null;
      }
}
