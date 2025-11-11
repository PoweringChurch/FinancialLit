using UnityEngine;
public enum Behaviour {Default,Roaming, Occupied}
public class PetBehaviour : MonoBehaviour
{
    public static PetBehaviour Instance;
    public Behaviour activeBehaviour;
    void Awake()
    {
        Instance = this;
        activeBehaviour = Behaviour.Roaming;
    }

    private float actionTimer = 10f; //time until pet does something
    void Update()
    {
        if (!PetMover.Instance.reachedGoal) return;
        actionTimer -= Time.deltaTime;
        if (actionTimer <= 0)
        {
            switch (activeBehaviour)
            {
                case Behaviour.Roaming:
                case Behaviour.Default:
                    RoamingAction();
                    break;
                case Behaviour.Occupied:
                    actionTimer = 3f;
                    break;
            }
        }
    }
    void RoamingAction()
    {
        int action = Random.Range(0, 5);
        switch (action)
        {
            case 0:
            case 1:
                actionTimer = 4;
                break;
            default:
                //try twice, else give up
                var targetPos = RandomPosition(20f);
                if (!VectorOverInteractable(targetPos)) targetPos = RandomPosition(10f);
                if (!VectorOverInteractable(targetPos)) break;
                PetMover.Instance.SetGoalPosition(targetPos);
                actionTimer = Random.Range(4f, 9f);
                break;
        }
    }
    Vector3 RandomPosition(float radius, bool conserveY = true)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;

        if (conserveY)
        {
            return transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
        else
        {
            Vector3 randomSphere = Random.insideUnitSphere * radius;
            return transform.position + randomSphere;
        }
    }
    [SerializeField] private LayerMask interactableLayer;
    bool VectorOverInteractable(Vector3 vector)
    {
        return Physics.Raycast(vector, Vector3.down, Mathf.Infinity, interactableLayer);
    }
}