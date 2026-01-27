using Unity.VisualScripting;
using UnityEngine;
public enum Behaviour {Default, Roaming, Occupied}
// Default enum might be depracated, as it serves the exact same purpose as Roaming
public class PetBehaviour : MonoBehaviour
{
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
    // time until pet does something
    private float actionTimer = 5f;
    void Update()
    {
        // do not do anything if moving
        if (!PetHelper.petMover.reachedGoal) return;
        // just in case, but shouldnt be an issue
        if (!CameraHandler.Instance.GameCamEnabled()) return;
        // increment timer
        actionTimer -= Time.deltaTime;
        if (actionTimer <= 0)
        {
            // check what the active behaviour is and determine what to do accordingly
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
    // the function that is calle when the pet is in the roaming state and the action timer reaches 0
    void RoamingAction()
    {
        // choose a number 0-10
        int action = Random.Range(0, 10);
        // check what number equals (might adjust to a float value and just use decimal values instead)
        switch (action)
        {
            // 1 / 10 chance for pet to bark / whimper
            case 0:
                if (PetHelper.petFlagManager.HasFlag(PetFlag.Sick))
                {
                    actionTimer = 6;
                    SFXPlayer.Instance.Play(whimper[Random.Range(0,whimper.Length)]);
                    break;
                }
                actionTimer = 2f;
                SFXPlayer.Instance.Play(barks[Random.Range(0,barks.Length)]); 
                break;
            // 1 / 10 chance to idle in place for 2 seconds
            case 1:
                actionTimer = 2;
                break;
            // 1 / 10 chance to sit down for 4-8 seconds
            case 2:
                PetHelper.petAnimation.SetBoolParameter("IsSitting", true);
                actionTimer = Random.Range(4f, 8f);
                break;
            // 7 / 10 chance (if not the other three) move around to a random nearby position, wait for 3-6 seconds
            default:
                PetHelper.petAnimation.SetBoolParameter("IsSitting", false);
                
                // try twice to get a valid position to move to, else give up
                var targetPos = RandomPosition(20f);
                if (!VectorOverInteractable(targetPos)) targetPos = RandomPosition(10f);
                if (!VectorOverInteractable(targetPos)) break;
                // set goal position to target position
                PetHelper.petMover.SetGoalPosition(targetPos);
                actionTimer = Random.Range(3f, 6f);
                break;
        }
        // action timer will increase as energy decreases
        float energyMult = PetHelper.petStats.Status["energy"] < 0.8f 
            ? 1.0f + (0.8f - PetHelper.petStats.Status["energy"]) * 0.1875f 
            : 1.0f;
        
        actionTimer *= energyMult;
    }
    // selected a random position in radius around pet transform, by default y is set to 0 unless false is passed as second argument
    private Vector3 RandomPosition(float radius, bool setyzero = true)
    {
        Vector3 randomCircle = Random.insideUnitCircle * radius;
        // if setyzero is true set y to zero
        if (setyzero)
            return transform.position + new Vector3(randomCircle.x, 0, randomCircle.z);
        else
            return transform.position + randomCircle;
    }
    [SerializeField] private LayerMask interactableLayer;
    // determines if the passed vector is over the interactable layer
    private bool VectorOverInteractable(Vector3 vector) { return Physics.Raycast(vector, Vector3.down, Mathf.Infinity, interactableLayer); }
}
