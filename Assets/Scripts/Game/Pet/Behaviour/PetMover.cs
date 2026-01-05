using UnityEngine;
using UnityEngine.AI;
using System;

public class PetMover : MonoBehaviour
{
    public PetAnimation petAnimation;
    public PetFlagManager petFlagManager;
    public PetStats petStats;

    public Transform petTransform;
    
    [HideInInspector] public bool reachedGoal;
    private float moveSpeed = 1.75f;
    private float stoppingDistance = 0.4f;
    
    public NavMeshAgent agent;
    public event Action OnReachedGoal;
    
    void Awake()
    {
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
                petAnimation.SetBoolParameter("IsMoving", false);
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
            float energyMult = 0.5f + 0.5f * petStats.Status["energy"];
            float sickMult = petFlagManager.HasFlag(PetFlag.Sick) ? 0.5f : 1f;
            float lovedMult = petFlagManager.HasFlag(PetFlag.Loved) ? 1.05f : 1f;
            float playfulMult = petFlagManager.HasFlag(PetFlag.Playful) ? 1.05f : 1f;
            agent.speed = moveSpeed * energyMult * sickMult * lovedMult * playfulMult;
        }
    }
    
    public void SetGoalPosition(Vector3 to)
    {
        agent.SetDestination(to);
        petAnimation.SetBoolParameter("IsSitting", false);
        petAnimation.SetBoolParameter("IsMoving", true);
        reachedGoal = false;
    }
}