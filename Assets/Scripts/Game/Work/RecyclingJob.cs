using System.Collections.Generic;
using UnityEngine;

public class RecyclingJob : MonoBehaviour
{
    public static RecyclingJob Instance;
    public Transform trashHolder;
    public GameObject trashPrefab;
    
    private float trashAppearChance = 0.02f; // every tick
    private float elapsed = 0f;
    public int trashCount = 0;
    private int maxTrashCount = 10;
    private void Awake() =>
        Instance = this;
    public void Tick(int count)
    {
        float spawnHit = UnityEngine.Random.Range(0f, 1f) * count;
        if (spawnHit < trashAppearChance && trashCount < maxTrashCount)
        {
            trashCount++;
            float x = UnityEngine.Random.Range(-32f,32f);
            float z = UnityEngine.Random.Range(-32f,32f);
            Vector3 position = new(x,1,z);
            Vector3 parkOrigin = new(0,0,-510);
            Instantiate(trashPrefab, parkOrigin+position, Quaternion.identity, trashHolder);
        }
    }
}