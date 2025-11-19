using UnityEngine;
using System.Collections.Generic;

public enum Items { Ball, Brush, Treat, Shampoo }

public class OrderHandler : MonoBehaviour
{
    public static OrderHandler Instance;
    public AudioClip nextOrder;
    private List<Items> currentOrder;
    private int completedOrderCount = 0;
    private int totalOrders = 10;
    private float orderTimer;
    private float timePerOrder = 8f;
    private float totalEarned = 0f;
    private float countdown = 0f;
    private bool shiftActive = false;
    
    void Start()
    {
        Instance = this;
    }
    
    void Update()
    {
        // Handle countdown before shift starts
        if (countdown > 0)
        {
            countdown -= Time.deltaTime;
            UIHandler.Instance.WorkManager.UpdateTimer(countdown);
            
            if (countdown <= 0)
            {
                // Countdown finished, start first order
                shiftActive = true;
                NextOrder();
            }
            return;
        }
        
        // Handle active orders
        if (shiftActive && currentOrder != null && currentOrder.Count > 0)
        {
            orderTimer -= Time.deltaTime;
            UIHandler.Instance.WorkManager.UpdateTimer(orderTimer);
            
            if (orderTimer <= 0)
            {
                NextOrder();
            }
        }
    }
    
    public void BeginShift()
    {
        countdown = 5f;
        completedOrderCount = 0;
        totalEarned = 0f;
        shiftActive = false;
        UIHandler.Instance.WorkManager.UpdateCompletedOrders(0, totalOrders);
    }
    
    public void NextOrder()
    {
        // dont play completed order if its not ocmpleted because you dont get the fun jingle if u dont work pal
        if (completedOrderCount > 0)
        {
            UISFXPlayer.Instance.Play(nextOrder);
        }
        // Check if shift is complete
        if (completedOrderCount >= totalOrders)
        {
            EndShift();
            return;
        }
        
        // Generate new order
        currentOrder = GenerateRandomOrder();
        orderTimer = timePerOrder;
                
        UIHandler.Instance.WorkManager.UpdateOrderDisplay(currentOrder);
        UIHandler.Instance.WorkManager.UpdateTimer(orderTimer);

        UIHandler.Instance.WorkManager.NextBox();
    }
    
    private List<Items> GenerateRandomOrder()
    {
        List<Items> order = new List<Items>();
        int itemCount = Random.Range(3, 6); 
        
        for (int i = 0; i < itemCount; i++)
        {
            Items randomItem = (Items)Random.Range(0, System.Enum.GetValues(typeof(Items)).Length);
            order.Add(randomItem);
        }
        
        return order;
    }
    
    public bool CheckItem(Items droppedItem)
    {
        if (currentOrder == null || !shiftActive) return false;
        
        // Check if this item is in the current order
        if (currentOrder.Contains(droppedItem))
        {
            currentOrder.Remove(droppedItem);
            UIHandler.Instance.WorkManager.UpdateOrderDisplay(currentOrder);
            
            // If order is complete
            if (currentOrder.Count == 0)
            {
                CompleteOrder();
            }
            
            return true;
        }
        
        return false;
    }
    
    private void CompleteOrder()
    {
        completedOrderCount++;
        
        // Calculate payment based on time remaining
        float timeBonus = Mathf.Clamp01(orderTimer / timePerOrder);
        float payment = 10f + (timeBonus * 10f); // Base 25 + up to 25 bonus
        totalEarned += payment;
        // Update UI with new count
        UIHandler.Instance.WorkManager.UpdateCompletedOrders(completedOrderCount, totalOrders);
        NextOrder();
    }
    
    public void CancelShift()
    {
        shiftActive = false;
        countdown = 0f;
        totalEarned = 0f;
        currentOrder = null;
    }
    private void EndShift()
    {
        shiftActive = false;
        UIHandler.Instance.WorkManager.EndShift(totalEarned);
        PlayerResources.Instance.AddMoney(totalEarned);
        totalEarned = 0f;
    }
}