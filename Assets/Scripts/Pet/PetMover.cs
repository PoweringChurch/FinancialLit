using UnityEngine;

public class PetMover : MonoBehaviour
{
    public static PetMover Instance;
    public Transform petModel;
    private float moveSpeed = 6f;
    private Vector3 goalPosition;
    private bool reachedGoal;
    private float stoppingDistance = 0.4f;
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
        if (!reachedGoal && !PetStateManager.HasState(PetState.Sitting))
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
            return;
        }
        
        var movement = moveSpeed * Time.deltaTime * direction.normalized;
        if (PetStateManager.HasState(PetState.Sick)) movement *= 0.5f;
        // apply movement
        transform.position += movement;

        if (petModel != null && direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            petModel.rotation = Quaternion.Slerp(petModel.rotation, targetRotation, Time.deltaTime * moveSpeed);
        }
    }
}