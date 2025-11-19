using UnityEngine;
using UnityEngine.AI;
using System;

public class PetMover : MonoBehaviour
{
    public static PetMover Instance;
    public Transform petTransform;
    
    [HideInInspector] public bool reachedGoal;
    private float moveSpeed = 2.5f;
    private float stoppingDistance = 0.4f;
    
    public NavMeshAgent agent;
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
                if (agent.velocity.magnitude > 0.01f && petTransform != null)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
                    petTransform.rotation = Quaternion.Slerp(petTransform.rotation, targetRotation, Time.deltaTime * moveSpeed * 4f);
                }
            }

            // Apply energy multiplier to speed
            float energyMult = 0.5f + 0.5f * PetStats.Instance.Status["energy"];
            float sickMult = PetFlagManager.HasFlag(PetFlag.Sick) ? 0.5f : 1f;
            float lovedMult = PetFlagManager.HasFlag(PetFlag.Loved) ? 1.05f : 1f;
            float playfulMult = PetFlagManager.HasFlag(PetFlag.Playful) ? 1.05f : 1f;
            agent.speed = moveSpeed * energyMult * sickMult * lovedMult * playfulMult;
        }
    }
    
    public void SetGoalPosition(Vector3 to)
    {
        agent.SetDestination(to);
        PetAnimation.Instance.SetBoolParameter("IsMoving", true);
        reachedGoal = false;
    }
}