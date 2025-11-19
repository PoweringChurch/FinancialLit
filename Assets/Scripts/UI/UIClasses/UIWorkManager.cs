using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class UIWorkManager
{
    public GameObject workoverlayUI;
    public GameObject ingameOverlayUI;
    public TextMeshProUGUI completedOrders;
    public TextMeshProUGUI timerText;
    public GameObject ballItemUI;
    public GameObject brushItemUI;
    public GameObject treatItemUI;
    public GameObject shampooItemUI;        
    public Transform reqHolder;
    public RectTransform boxTransform;
    private Dictionary<Items, GameObject> itemUIMap;        
    public void Initialize()
    {            
        itemUIMap = new Dictionary<Items, GameObject>
        {
            { Items.Ball, ballItemUI },
            { Items.Brush, brushItemUI },
            { Items.Treat, treatItemUI },
            { Items.Shampoo, shampooItemUI }
        };
    }
    
    public void NextBox()
    {
        UIHandler.Instance.StartCoroutine(BoxTransitionAnimation());
    }
    private IEnumerator BoxTransitionAnimation()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        
        // Get screen width for off-screen positions
        float screenWidth = Screen.width;
        Vector2 centerPos = boxTransform.anchoredPosition;
        Vector2 rightOffScreen = new Vector2(screenWidth, centerPos.y);
        Vector2 leftOffScreen = new Vector2(-screenWidth, centerPos.y);
        
        // Slide current box off to the right
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            boxTransform.anchoredPosition = Vector2.Lerp(centerPos, rightOffScreen, t);
            yield return null;
        }
        
        // Teleport box to left side (off screen)
        boxTransform.anchoredPosition = leftOffScreen;
        
        // Reset elapsed time for slide in
        elapsed = 0f;
        
        // Slide new box in from the left
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            boxTransform.anchoredPosition = Vector2.Lerp(leftOffScreen, centerPos, t);
            yield return null;
        }
        // Ensure it's exactly centered
        boxTransform.anchoredPosition = centerPos;
    }
    public void StartWorking()
    {
        workoverlayUI.SetActive(true);
        OrderHandler.Instance.BeginShift();
    }
    
    public void UpdateTimer(float timeRemaining)
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {timeRemaining:F1}s";
        }
    }
    public void UpdateOrderDisplay(List<Items> currentOrder)
    {
        for (int i = reqHolder.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(reqHolder.GetChild(i).gameObject);
        }
        foreach (Items item in currentOrder)
        {
            UnityEngine.Object.Instantiate(itemUIMap[item],reqHolder);
        }
    }
    public void UpdateCompletedOrders(int completed, int total)
    {
        if (completedOrders != null)
        {
            completedOrders.text = $"Orders: {completed}/{total}";
        }
    }
    public void CancelWork()
    {
        string header = "Stop working?";
        string body = "Are you sure you want to stop working? You will lose any earned money.";
        UIHandler.Instance.PopupManager.PopupYN(header,body, () =>
        {
            OrderHandler.Instance.CancelShift();
            CameraHandler.Instance.ToggleGamecam(true);
            ingameOverlayUI.SetActive(true);
            workoverlayUI.SetActive(false);
        }, () => {});
    }
    public void EnterWork()
    {
        string header = "Start working";
        string body = "Fulfill customer orders by dragging the requested items into the delivery box. Work fast for bonus earnings!";
        UIHandler.Instance.PopupManager.PopupYN(header,body, () =>
        {
            OrderHandler.Instance.BeginShift();
            CameraHandler.Instance.ToggleGamecam(false);
            workoverlayUI.SetActive(true);
            ingameOverlayUI.SetActive(false);
        }, null, "Start", "Nevermind");
    }
    public void EndShift(float totalEarned)
    {
        string body = $"Great work! You earned ${totalEarned:F2} for your hard work!";
        UIHandler.Instance.PopupManager.PopupInfo("Job well done!",body,"Yay!",() =>
        {
            CameraHandler.Instance.ToggleGamecam(true);
            ingameOverlayUI.SetActive(true);
            workoverlayUI.SetActive(false);
        });
    }
}