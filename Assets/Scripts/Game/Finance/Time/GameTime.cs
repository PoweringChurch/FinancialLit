using UnityEngine;
using System;
// in progress

// solely responsible for time related things, such as date
public class GameTime : MonoBehaviour
{
    public static GameTime Instance;
    private int minute = 0; // 0 - 1440, increments every second
    private int day = 0; // 0 - 6, each unit is a 1440 minutes (6 is max because of 0 indexing)
    private int week = 0; // 0 - maxint, each unit is 7 days

    private float elapsed = 0;
    private float minuteDuration = 1;

    public float Minute { get { return minute; }  }
    public float Day    { get { return day; }     }
    public float Week   { get { return week; }    }
    
    public float MinuteDuration 
    {
        get 
        {
            return minuteDuration;
        }
        set 
        {
            // minutes will never last longer than 1 second
            minuteDuration = Math.Min(1,value);
        }
    }
    void Start()
    {
        Instance = this;
        PetHelper.petStateMachine.OnStateChanged += ApplyStateMod;
        Debug.Log("awake");
    }
    // for use when onstatechanged is called in petStateMachine
    void ApplyStateMod(PetState oldState, PetState newState)
    {
        if (newState == PetState.Sleeping)
            MinuteDuration = 0.5f;
        else
            MinuteDuration = 1;
    }
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= minuteDuration)
        {
            ElapseTime(1);
            Debug.Log("minute: " + minute);
            elapsed = 0;
        }
    }
    // elapses time by given amount in passedMinutes
    public void ElapseTime(int passedMinutes) 
    {
        // calculate minute
        minute += passedMinutes;
        int newMinute = minute%1440;
        // calculate days
        int passedDays = minute/1440;
        day += passedDays;
        int newDay = day%7;

        int passedWeeks = day/7;

        minute = newMinute;
        day = newDay;
        week += passedWeeks;
    }
}
