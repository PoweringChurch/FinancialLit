using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AssetLoader : MonoBehaviour
{
    public GameObject loadingPanel;
    public Slider progressBar;
    public Text loadingText;
    
    private List<Material> loadedMaterials = new List<Material>();
    void Start()
    {
        StartLoading();
    }
    public void StartLoading()
    {
        StartCoroutine(LoadAssets());
    }
    
    IEnumerator LoadAssets()
    {
        loadingPanel.SetActive(true);
        
        // List of material paths in Resources folder
        string[] materialPaths = new string[]
        {
            "Materials",
        };
        
        float totalAssets = materialPaths.Length;
        float loadedAssets = 0f;
        
        foreach (string path in materialPaths)
        {
            // Load async
            ResourceRequest request = Resources.LoadAsync<Material>(path);
            
            // Wait for it to finish
            while (!request.isDone)
            {
                // Update progress bar with current asset progress
                float currentProgress = (loadedAssets + request.progress) / totalAssets;
                progressBar.value = currentProgress;
                loadingText.text = $"Loading... {currentProgress * 100f:F0}%";
                yield return null;
            }
            
            // Store loaded material
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
        
        // Do whatever you need with loaded materials
        OnLoadingComplete();
    }
    
    void OnLoadingComplete()
    {
        Debug.Log($"Loaded {loadedMaterials.Count} materials!");
        // Apply materials, start game, etc.
    }
}