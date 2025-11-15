using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class FurniturePreviewBaker : EditorWindow
{
    private string prefabFolderPath = "Assets/Prefabs/Furniture";
    private string outputFolderPath = "Assets/Resources/FurniturePreviews";
    private int previewSize = 512;
    private Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    private bool useTransparentBackground = false;
    private Vector3 cameraAngle = new Vector3(20, -30, 0); // Euler angles
    
    [MenuItem("Tools/Furniture Preview Baker")]
    public static void ShowWindow()
    {
        GetWindow<FurniturePreviewBaker>("Preview Baker");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Furniture Preview Baker", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        prefabFolderPath = EditorGUILayout.TextField("Prefab Folder", prefabFolderPath);
        outputFolderPath = EditorGUILayout.TextField("Output Folder", outputFolderPath);
        
        EditorGUILayout.Space();
        previewSize = EditorGUILayout.IntSlider("Preview Size", previewSize, 128, 1024);
        
        useTransparentBackground = EditorGUILayout.Toggle("Transparent Background", useTransparentBackground);
        if (!useTransparentBackground)
        {
            backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
        }
        
        cameraAngle = EditorGUILayout.Vector3Field("Camera Angle", cameraAngle);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Generate All Previews", GUILayout.Height(40)))
        {
            GenerateAllPreviews();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This will create preview images for all prefabs in the specified folder.", MessageType.Info);
    }
    
    void GenerateAllPreviews()
    {
        // Create output folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(outputFolderPath))
        {
            string parentFolder = Path.GetDirectoryName(outputFolderPath);
            string newFolderName = Path.GetFileName(outputFolderPath);
            AssetDatabase.CreateFolder(parentFolder, newFolderName);
        }
        
        // Find all prefabs in the folder
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolderPath });
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("No Prefabs Found", $"No prefabs found in {prefabFolderPath}", "OK");
            return;
        }
        
        // Setup preview scene
        Camera previewCamera = SetupPreviewCamera();
        Light previewLight = SetupPreviewLight();
        
        int count = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                EditorUtility.DisplayProgressBar("Generating Previews", 
                    $"Processing {prefab.name}...", (float)count / guids.Length);
                
                GeneratePreviewForPrefab(prefab, previewCamera);
                count++;
            }
        }
        
        // Cleanup
        if (previewCamera != null)
            DestroyImmediate(previewCamera.gameObject);
        if (previewLight != null)
            DestroyImmediate(previewLight.gameObject);
        
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        
        // Auto-assign sprites to FurnitureData
        AssignSpritesToFurnitureData();
        
        EditorUtility.DisplayDialog("Complete", $"Generated {count} preview images in {outputFolderPath}", "OK");
    }
    
    void AssignSpritesToFurnitureData()
    {
        // Find the FurnitureDatabase GameObject in any scene
        // First check if there's one in the current scene
        FurnitureDatabase database = GameObject.FindFirstObjectByType<FurnitureDatabase>();
        
        // If not in current scene, search all scenes in build settings
        if (database == null)
        {
            string[] scenePaths = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
            
            foreach (string scenePath in scenePaths)
            {
                var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
                    scenePath, 
                    UnityEditor.SceneManagement.OpenSceneMode.Additive
                );
                
                database = GameObject.FindFirstObjectByType<FurnitureDatabase>();
                
                if (database != null)
                    break;
                    
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
            }
        }
        
        if (database == null)
        {
            Debug.LogWarning("FurnitureDatabase not found in any scene. Make sure it exists in a scene in Build Settings.");
            return;
        }
        
        SerializedObject serializedDB = new SerializedObject(database);
        SerializedProperty furnitureArray = serializedDB.FindProperty("allFurniture");
        
        int assignedCount = 0;
        
        // Loop through all furniture items
        for (int i = 0; i < furnitureArray.arraySize; i++)
        {
            SerializedProperty furnitureItem = furnitureArray.GetArrayElementAtIndex(i);
            FurnitureData data = furnitureItem.objectReferenceValue as FurnitureData;
            
            if (data == null || data.prefab == null)
                continue;
            
            // Find matching sprite by prefab name
            string spritePath = $"{outputFolderPath}/{data.prefab.name}_Preview.png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            
            if (sprite != null)
            {
                SerializedObject serializedData = new SerializedObject(data);
                SerializedProperty iconProperty = serializedData.FindProperty("icon");
                
                if (iconProperty.objectReferenceValue != sprite)
                {
                    iconProperty.objectReferenceValue = sprite;
                    serializedData.ApplyModifiedProperties();
                    EditorUtility.SetDirty(data);
                    assignedCount++;
                }
            }
        }
        
        serializedDB.ApplyModifiedProperties();
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Auto-assigned {assignedCount} preview sprites to FurnitureData!");
    }
    
    Camera SetupPreviewCamera()
    {
        GameObject camObj = new GameObject("PreviewCamera");
        Camera cam = camObj.AddComponent<Camera>();
        
        cam.clearFlags = useTransparentBackground ? CameraClearFlags.SolidColor : CameraClearFlags.SolidColor;
        cam.backgroundColor = useTransparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
        cam.orthographic = false;
        cam.fieldOfView = 30f;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 100f;
        cam.cullingMask = 1 << 7; // Only render layer 7 (Furniture)
        
        return cam;
    }
    
    Light SetupPreviewLight()
    {
        GameObject lightObj = new GameObject("PreviewLight");
        Light light = lightObj.AddComponent<Light>();
        
        light.type = LightType.Directional;
        light.color = Color.white;
        light.intensity = 1f;
        lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
        
        return light;
    }
    
    void GeneratePreviewForPrefab(GameObject prefab, Camera cam)
    {
        // Instantiate prefab
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
        
        // Set all objects to Furniture layer (layer 7)
        SetLayerRecursively(instance, 7);
        
        // Frame the object
        Bounds bounds = CalculateBounds(instance);
        PositionCamera(cam, bounds);
        
        // Create RenderTexture
        RenderTexture rt = new RenderTexture(previewSize, previewSize, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 4;
        cam.targetTexture = rt;
        
        // Render
        cam.Render();
        
        // Save to PNG
        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(previewSize, previewSize, TextureFormat.RGBA32, false);
        screenshot.ReadPixels(new Rect(0, 0, previewSize, previewSize), 0, 0);
        screenshot.Apply();
        
        byte[] bytes = screenshot.EncodeToPNG();
        string filename = $"{outputFolderPath}/{prefab.name}_Preview.png";
        File.WriteAllBytes(filename, bytes);
        
        // Cleanup
        RenderTexture.active = null;
        cam.targetTexture = null;
        rt.Release();
        DestroyImmediate(screenshot);
        DestroyImmediate(instance);
        
        // Import and configure as sprite
        AssetDatabase.ImportAsset(filename);
        TextureImporter importer = AssetImporter.GetAtPath(filename) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = useTransparentBackground;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
    }
    
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    
    Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one);
        
        Bounds bounds = renderers[0].bounds;
        foreach (var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        
        return bounds;
    }
    
    void PositionCamera(Camera cam, Bounds bounds)
    {
        // Calculate distance to fit object
        float maxExtent = bounds.extents.magnitude;
        float distance = maxExtent / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        
        // Apply camera angle
        Quaternion rotation = Quaternion.Euler(cameraAngle);
        Vector3 direction = rotation * Vector3.forward;
        
        cam.transform.position = bounds.center - direction * distance * 1.5f;
        cam.transform.LookAt(bounds.center);
    }
}