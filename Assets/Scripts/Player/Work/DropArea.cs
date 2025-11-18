using UnityEngine;

public class DropZone : MonoBehaviour
{
    public void HandleDrop(Items itemType)
    {        
        bool correct = OrderHandler.Instance.CheckItem(itemType);
        if (correct)
        {
            // Visual/audio feedback for correct item
            Debug.Log("✓ Correct item!");
        }
        else
        {
            // Visual/audio feedback for wrong item
            Debug.Log("✗ Wrong item!");
        }
}
}