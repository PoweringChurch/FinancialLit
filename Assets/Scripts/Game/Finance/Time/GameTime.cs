using UnityEngine;

// in progress

// solely responsible for time related things, such as date
public class GameTime : MonoBehaviour
{
    private int minute = 0; // 0 - 1440, increments every second
    private int day = 0; // 0 - 7, each unit is a 1440 minutes
    private int week = 0; // 0 - maxint, each unit is 7 days

    private float elapsed = 0;
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= 1) // every one second
        {
            ElapseTime(1);
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
    }
}
