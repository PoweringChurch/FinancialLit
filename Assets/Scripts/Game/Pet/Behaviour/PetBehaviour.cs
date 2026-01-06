using Unity.VisualScripting;
using UnityEngine;
public enum Behaviour {Default,Roaming, Occupied}
public class PetBehaviour : MonoBehaviour
{
    public PetMover petMover;
    public PetFlagManager petFlagManager;
    public PetAnimation petAnimation;
    public PetStats petStats;

    public AudioClip[] barks;
    public AudioClip[] whimper;
    private Behaviour activeBehaviour;
    public Behaviour ActiveBehaviour
    {
        set { activeBehaviour = value; }
        get { return activeBehaviour;}
    }
    void Awake()
    {
        activeBehaviour = Behaviour.Roaming;
    }

    private float actionTimer = 5f; //time until pet does something
    void Update()
    {
        if (!petMover.reachedGoal) return;
        if (!CameraHandler.Instance.GameCamEnabled()) return;
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
                    actionTimer = 5f;
                    break;
            }
        }
    }
    void RoamingAction()
    {
        int action = Random.Range(0, 10);
        switch (action)
        {
            case 0:
                if (petFlagManager.HasFlag(PetFlag.Sick))
                {
                    actionTimer = 6;
                    SFXPlayer.Instance.Play(whimper[Random.Range(0,whimper.Length)]);
                    break;
                }
                actionTimer = 2f;
                SFXPlayer.Instance.Play(barks[Random.Range(0,barks.Length)]); 
                break;
            case 1:
                // Idle in place
                actionTimer = 2;
                break;
            case 2:
                // Sit down for a bit
                petAnimation.SetBoolParameter("IsSitting", true);
                actionTimer = Random.Range(4f, 8f);
                break;
            default:
                // Stand up if  sitting
                petAnimation.SetBoolParameter("IsSitting", false);
                
                // Try twice, else give up
                var targetPos = RandomPosition(20f);
                if (!VectorOverInteractable(targetPos)) targetPos = RandomPosition(10f);
                if (!VectorOverInteractable(targetPos)) break;
                petMover.SetGoalPosition(targetPos);
                actionTimer = Random.Range(3f, 6f);
                break;
        }
        float energyMult = petStats.Status["energy"] < 0.8f 
            ? 1.0f + (0.8f - petStats.Status["energy"]) * 0.1875f 
            : 1.0f;
        
        actionTimer *= energyMult;
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