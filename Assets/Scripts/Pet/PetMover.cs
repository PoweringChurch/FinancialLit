using System;
using UnityEngine;

public class PetMover : MonoBehaviour
{
    public static PetMover Instance;
    public Transform petModel;
    
    [HideInInspector] public bool reachedGoal;

    private float moveSpeed = 6f;
    private Vector3 goalPosition;
    private float stoppingDistance = 0.4f;

    public event Action OnReachedGoal;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        goalPosition = transform.position;
        reachedGoal = true;
    }
    void Update()
    {
        if (!reachedGoal)
        {
            MovePet();
        }
    }
    public void SetGoalPosition(Vector3 to)
    {
        goalPosition = to;
        reachedGoal = false;
    }
    
    void MovePet()
    {
        Vector3 direction = goalPosition - transform.position;
        float distance = direction.magnitude;
        
        if (distance <= stoppingDistance)
        {
            reachedGoal = true;
            OnReachedGoal?.Invoke();
            return;
        }
        
        var movement = moveSpeed * Time.deltaTime * direction.normalized;
        // apply movement
        transform.position += movement;

        if (petModel != null && direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            petModel.rotation = Quaternion.Slerp(petModel.rotation, targetRotation, Time.deltaTime * moveSpeed);
        }
    }
}