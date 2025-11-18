using UnityEngine;

public class DropZone : MonoBehaviour
{
    public void HandleDrop(Items itemType)
    {        
        bool correct = OrderHandler.Instance.CheckItem(itemType);
        if (correct)
        {
            // Visual/audio feedback for correct item
        }
        else
        {
            // Visual/audio feedback for wrong item
        }
}
}