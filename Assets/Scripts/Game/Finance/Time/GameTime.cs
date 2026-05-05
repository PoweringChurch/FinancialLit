using UnityEngine;
using System;
// in progress

// responsible for time related things, such as date
public class GameTime : MonoBehaviour
{
    public static GameTime Instance;
    public bool IsFastForwarding { 
        get
        {
            return minuteDuration != 1;
        }
    }
    private Light mainLight;
    private int minute = 0; // 0 - 1440, increments every second
    private int day = 0; // 0 - 6, each unit is a 1440 minutes
    private int week = 0; // 0 - maxint, each unit is 7 days

    private float elapsed = 0;
    private float minuteDuration = 1;

    public int Minute { get { return minute; }  }
    public int Day    { get { return day; }     }
    public int Week   { get { return week; }    }
    
    public float MinuteDuration 
    {
        get 
        {
            return minuteDuration;
        }
    }
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        mainLight = transform.parent.Find("WorldLight").GetComponent<Light>();
        PetHelper.petStateMachine.OnStateChanged += ApplyStateMod;
        ElapseTime(0);
    }
    // for use when onstatechanged is called in petStateMachine
    void ApplyStateMod(PetState oldState, PetState newState)
    {
        switch (newState)
        {
            case PetState.Sleeping:
                minuteDuration = 0.2f;
                break;
            case PetState.Bathing:
                minuteDuration = 0.35f;
                break;
            case PetState.Playing:
                minuteDuration = 0.8f;
                break;
            default:
                minuteDuration = 1;
                break;
        }
    }
    void Update()
    {
        // check if were on a shift (pause time updates if work is happening)
        if (WorkHandler.Instance.ShiftActive)
            return;
        elapsed += Time.deltaTime;
        if (elapsed >= minuteDuration)
        {
            ElapseTime(1);
            elapsed = 0;
        }
    }
    // elapses time by given amount in passedMinutes
    public void ElapseTime(int passedMinutes, bool atWork = false) 
    {
        // tick the pet's stats
        PetHelper.petStats.Tick(passedMinutes,atWork);
        
        // add passed minutes to total
        minute += passedMinutes;
        
        // calculate how many days passed
        int passedDays = minute / 1440;
        minute %= 1440;
        
        // add passed days to total
        day += passedDays;
        
        // calculate how many weeks passed
        int passedWeeks = day / 7;
        day %= 7;
        
        // add passed weeks to total
        week += passedWeeks;

        // progress day
        mainLight.intensity = 0.3f + 0.95f * Mathf.Sin((minute / 1440f) * Mathf.PI);
        mainLight.transform.rotation = Quaternion.Euler((minute / 1440f)*180,0,0);
        
        UIOverlay.Instance.UpdateTime();
    }
    // sets the time without ticking game, really only used when loading the game
    public void SetTime(int newMinute, int newDay, int newWeek)
    {
        minute = newMinute;
        day = newDay;
        week = newWeek;
    }
}
