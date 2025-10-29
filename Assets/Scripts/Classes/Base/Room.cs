using UnityEngine;

public class Room : MonoBehaviour
{
    private Vector2 gridAnchor = new Vector2(2,2); //size of each grid tile
    private Vector2[] occupiedTiles = new Vector2[];
    public void PlaceObject(Vector2 position, Vector2 size, int orientation) //0-3, each corresponding to 90d rot
    {
        
    }
    public Vector2[] OccupiedTiles {
        get {
            return occupiedTiles;
        }
    }
}
