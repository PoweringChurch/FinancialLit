using UnityEngine;
using UnityEngine.UI;
public class PaintInit : MonoBehaviour
{
    public GameObject paintButtonTemplate;
    public GameObject floorButtonTemplate;
    public Transform paintContent;
    public Transform floorContent;
    public Image selectedPaintDisplay;
    public Image selectedMaterialDisplay;
    void Start()
    {
        foreach (var wallPaint in PlacementManager.Instance.Wall.wallPaints)
        {
            // set the paint button's preview color
            var newPaintButton = Instantiate(paintButtonTemplate, paintContent);
            var paintPrevImg = newPaintButton.GetComponent<Image>();
            Color c = wallPaint.material.GetColor("_Color"); 
            paintPrevImg.color = c;
            // set the paint icons color
            var paintIcon = newPaintButton.transform.Find("Icon");
            var iconImg = paintIcon.GetComponent<Image>();
            // calc luminance
            float lum = 0.3f * c.r + 0.6f * c.g + 0.115f * c.b;
            iconImg.color = lum > 0.5f ? Color.black : Color.white;
            
            var paintButton = newPaintButton.GetComponent<Button>();
            paintButton.onClick.AddListener(() => {PlacementManager.Instance.Wall.selectedPaint = wallPaint.key; selectedPaintDisplay.color = c;} );
        }

        foreach (var floorMaterial in PlacementManager.Instance.Floor.floorMaterials)
        {
            // set the paint button's preview color
            var newFloorButton = Instantiate(floorButtonTemplate, floorContent);
            newFloorButton.transform.Find("Preview").GetComponent<Image>().sprite = floorMaterial.previewImage;
            var floorButton = newFloorButton.GetComponent<Button>();
            floorButton.onClick.AddListener(() => {PlacementManager.Instance.Floor.selectedMaterial = floorMaterial.key; selectedMaterialDisplay.sprite = floorMaterial.previewImage;} );
        }
    }
}
