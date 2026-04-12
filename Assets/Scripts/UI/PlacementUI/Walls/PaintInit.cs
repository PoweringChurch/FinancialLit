using UnityEngine;
using UnityEngine.UI;
public class PaintInit : MonoBehaviour
{
    public GameObject paintButtonTemplate;
    public Transform content;
    public Image selectedDisplay;
    void Start()
    {
        foreach (var wallPaint in PlacementManager.Instance.Wall.wallPaints)
        {
            // set the paint button's preview color
            var newPaintButton = Instantiate(paintButtonTemplate, content);
            var paintPrevImg = newPaintButton.GetComponent<Image>();
            Color c = wallPaint.material.GetColor("_Color"); // i need base color instead of color; putting in color gives me a fully transparent color
            paintPrevImg.color = c;
            // set the paint icons color
            var paintIcon = newPaintButton.transform.Find("Icon");
            var iconImg = paintIcon.GetComponent<Image>();
            // calc luminance
            float lum = 0.3f * c.r + 0.6f * c.g + 0.115f * c.b;
            iconImg.color = lum > 0.5f ? Color.black : Color.white;
            
            var paintButton = newPaintButton.GetComponent<Button>();
            paintButton.onClick.AddListener(() => {PlacementManager.Instance.Wall.selectedPaint = wallPaint.key; selectedDisplay.color = c;} );
            
        }
    }
}
