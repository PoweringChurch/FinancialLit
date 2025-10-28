using UnityEngine;
using System.Collections.Generic;
public class PlacementHandler : MonoBehaviour
{
    public static readonly Dictionary<int, Furniture> FurnitureIDs= new()
    {
        [0] = new Furniture(),
        [1] = new Furniture()
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private Furniture selectedFurniture;
    public void SelectFurniture(int furnitureId)
    {
        selectedFurniture = FurnitureIDs[furnitureId];
    }
    public void PlaceFurniture(Vector3 cursorPosition, GameObject room)
    {
        Furniture newFurniture = Instantiate(selectedFurniture, cursorPosition,Quaternion.identity);
        newFurniture.transform.SetParent(room.transform);
    }
}