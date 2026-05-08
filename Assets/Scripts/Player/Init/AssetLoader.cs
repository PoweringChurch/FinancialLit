using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class AssetLoader : MonoBehaviour
{
    public GameObject loadingPanel;
    public Slider progressBar;
    public Text loadingText;
    // set contrast
    public Canvas canvas;
    private Image[] images;
    private TextMeshProUGUI[] tmpros;

    private List<Material> loadedMaterials = new List<Material>();
    void Start()
    {
        StartLoading();
    }
    public void StartLoading()
    {
        SetContrast();
        StartCoroutine(LoadAssets());
    }
    private List<Transform> toDeactivate = new();
    void SetActiveAllDescendants(Transform parent)
    {
        parent.gameObject.SetActive(true);
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf == false)
            {
                child.gameObject.SetActive(true);
                toDeactivate.Add(child);
            }
            SetActiveAllDescendants(child);
        }
    }
    void DeactivateChildren()
    {
        foreach (Transform childTransform in toDeactivate)
            childTransform.gameObject.SetActive(false);
    }
    public void SetContrast()
    {
        SetActiveAllDescendants(canvas.transform);
        images = canvas.GetComponentsInChildren<Image>();
        tmpros = canvas.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var img in images)
        {
            Color c = img.color;
            float lum = 0.3f * c.r + 0.6f * c.g + 0.115f * c.b;
            img.color = lum > 0.5f ? Color.black : Color.white;
        }
        foreach (var tmp in tmpros)
        {
            tmp.color = Color.grey;
        }
        DeactivateChildren();
        print("contrast helper init");
    }
    IEnumerator LoadAssets()
    {
        loadingPanel.SetActive(true);
        
        string[] materialPaths = new string[]
        {
            "Materials",
        };
        
        float totalAssets = materialPaths.Length;
        float loadedAssets = 0f;
        
        foreach (string path in materialPaths)
        {
            // load async
            ResourceRequest request = Resources.LoadAsync<Material>(path);
            
            // wait for it to finish
            while (!request.isDone)
            {
                // update progress bar with current asset progress
                float currentProgress = (loadedAssets + request.progress) / totalAssets;
                progressBar.value = currentProgress;
                loadingText.text = $"Loading... {currentProgress * 100f:F0}%";
                yield return null;
            }
            
            // store loaded material
            Material mat = request.asset as Material;
            if (mat != null)
            {
                loadedMaterials.Add(mat);
            }
            
            loadedAssets++;
            progressBar.value = loadedAssets / totalAssets;
        }
        
        loadingText.text = "Loading Complete!";
        yield return new WaitForSeconds(0.5f);
        
        loadingPanel.SetActive(false);
        
        OnLoadingComplete();
    }
    
    void OnLoadingComplete()
    {
        Debug.Log($"Loaded {loadedMaterials.Count} materials!");
    }
}