using UnityEngine;
using UnityEngine.AI;
using System;

public class PetMover : MonoBehaviour
{
    public static PetMover Instance;
    public Transform petModel;
    
    [HideInInspector] public bool reachedGoal;
    private float moveSpeed = 3f;
    private float stoppingDistance = 0.4f;
    
    private NavMeshAgent agent;
    public event Action OnReachedGoal;
    
    void Awake()
    {
        Instance = this;
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.angularSpeed = 0; // We'll handle rotation manually
            agent.updateRotation = false;
        }
    }
    
    void Start()
    {
        reachedGoal = true;
    }
    
    void Update()
    {
        if (!reachedGoal && agent != null)
        {
            // Check if reached destination
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                reachedGoal = true;
                PetAnimation.Instance.SetBoolParameter("IsMoving", false);
                OnReachedGoal?.Invoke();
            }
            else
            {
                // Handle rotation
                if (agent.velocity.magnitude > 0.01f && petModel != null)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
                    petModel.rotation = Quaternion.Slerp(petModel.rotation, targetRotation, Time.deltaTime * moveSpeed * 4f);
                }
            }
            
            // Apply energy multiplier to speed
            float energyMult = (float)(1 - Math.Max(0, 3 * Math.Log(-Math.Clamp(PetStats.Instance.Status["energy"], 0, 1) + 1.4f)));
            agent.speed = moveSpeed * energyMult;
        }
    }
    
    public void SetGoalPosition(Vector3 to)
    {
        if (agent != null)
        {
            agent.SetDestination(to);
            PetAnimation.Instance.SetBoolParameter("IsMoving", true);
            reachedGoal = false;
        }
    }
}