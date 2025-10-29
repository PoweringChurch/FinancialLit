using UnityEngine;
using System.Collections.Generic;
public class PlacementHandler : MonoBehaviour
{
    private Dictionary<int, GameObject> placeableObjectIds;
    //selected object
    //tell where to place
    //tell in what room to place (where to parent instantiated object)

    //place func
    //remove func

    private GameObject selectedObject;
    private GameObject currentRoom;

    public GameObject roominit;
    void Start()
    {
        placeableObjectIds= new()
        {
            [0] = Resources.Load<GameObject>("Furniture/Desk"),
        };

        SetSelectedObject(0);
        SetRoom(roominit);
    }
    public void SetSelectedObject(int id)
    {
        selectedObject = placeableObjectIds[id];
    }
    public void SetRoom(GameObject newroom)
    {
        currentRoom = newroom;
    }
    public void Place(Vector3 at)
    {
        if (IsOccupied(at))
        {
            return;
        }
        var newSelectedObj = Instantiate(selectedObject, at, Quaternion.identity);
        newSelectedObj.transform.SetParent(currentRoom.transform, true);
    }
    private bool IsOccupied(Vector3 at)
    {
        return false;
    }
}
