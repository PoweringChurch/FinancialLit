using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToggleObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private bool showOnHover = true; 
    
    private void Start()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!showOnHover);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetObject != null)
        {
            targetObject.SetActive(showOnHover);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!showOnHover);
        }
    }
}