using UnityEngine;
using System.Collections.Generic;
/*
["FuelSage Rewards"] = false, // reduces travel costs
["Petsy Health Maxx"] = false, // reduces vet expenses
["SmartyPets Delivery"] = false, // receive 4 food and 2 soap weekly
["Furniture Points"] = false, // reduces furniture costs
["Speedy Shops"] = false // increases money earned while working
*/
public class Membership
{
    public string name; // for use in display
    public float cost; // weekly
    public float signupFee; // when signing up
    public bool scheduledCancel = false;

    public Membership(string setname, float setcost, float setsignupFee)
    {
        name = setname;
        cost = setcost;
        signupFee = setsignupFee;
    }
}
// handles memberships
public class Memberships : MonoBehaviour
{
    public static Memberships Instance;

    // id, membership obj
    private Dictionary<string, Membership> activeMemberships = new();
    
    void Awake() { Instance = this; }
    // add a membership; does not check if player has enough to spend
    public void AddMembership(string id, Membership newMembership)
    {
        activeMemberships[id] = newMembership;
    }
    public void ScheduleCancel(string id)
    {
        activeMemberships[id].scheduledCancel = true;
    }
}