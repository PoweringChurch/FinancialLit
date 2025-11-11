using System;
using UnityEngine;

public class PetMover : MonoBehaviour
{
    public static PetMover Instance;
    public Transform petModel;
    
    [HideInInspector] public bool reachedGoal;

    private float moveSpeed = 3f;
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
        PetAnimation.Instance.SetBoolParameter("IsMoving", true);
        reachedGoal = false;
    }
    void MovePet()
    {
        Vector3 direction = goalPosition - transform.position;
        float distance = direction.magnitude;

        if (distance <= stoppingDistance)
        {
            reachedGoal = true;
            PetAnimation.Instance.SetBoolParameter("IsMoving", false);
            OnReachedGoal?.Invoke();
            return;
        }

        float energyMult = (float)(1 - Math.Max(0,3*Math.Log(-Math.Clamp(PetStats.Instance.Status["energy"],0,1)+1.4f)));
        var movement = energyMult*moveSpeed * Time.deltaTime * direction.normalized;
        // apply movement
        transform.position += movement;

        if (petModel != null && direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            petModel.rotation = Quaternion.Slerp(petModel.rotation, targetRotation, Time.deltaTime * moveSpeed*4f);
        }
    }
}