using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class PetMover : MonoBehaviour
{
    [HideInInspector] public bool reachedGoal; // could use a property here, but there isnt really a reason to

    const float moveSpeed = 1.75f;
    const float stoppingDistance = 0.4f;
    
    public event Action OnReachedGoal;
    public NavMeshAgent agent;
    public Transform petTransform;

    void Start()
    {
        // set variables
        petTransform = transform;
        reachedGoal = true;
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.angularSpeed = 0;
            agent.updateRotation = false;
        }
    }

    void Update()
    {
        if (!reachedGoal && agent != null)
        {
            // check if reached destination
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                reachedGoal = true;
                PetHelper.petAnimation.SetBoolParameter("IsMoving", false);
                OnReachedGoal?.Invoke();
            }
            else
            {
                // handle rotation
                if (agent.velocity.magnitude > 0.01f && transform != null)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed * 4f);
                }
            }

            // apply stats multiplier to speed
            float energyMult = 0.5f + 0.5f * (PetHelper.petStats.Status["energy"]/100);
            float sickMult = PetHelper.petFlagManager.HasFlag(PetFlag.Sick) ? 0.5f : 1f;
            float lovedMult = PetHelper.petFlagManager.HasFlag(PetFlag.Loved) ? 1.05f : 1f;
            float playfulMult = PetHelper.petFlagManager.HasFlag(PetFlag.Playful) ? 1.05f : 1f;
            agent.speed = moveSpeed * energyMult * sickMult * lovedMult * playfulMult;
        }
    }
    // set the pet's goal position
    public void SetGoalPosition(Vector3 to)
    {
        agent.SetDestination(to);
        PetHelper.petAnimation.SetBoolParameter("IsSitting", false);
        PetHelper.petAnimation.SetBoolParameter("IsMoving", true);
        reachedGoal = false;
    }
}