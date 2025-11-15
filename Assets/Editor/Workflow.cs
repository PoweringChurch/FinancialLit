#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using UnityEngine.AI;

public class FurnitureWorkflowWindow : EditorWindow
{
    // Tabs
    private int selectedTab = 0;
    private string[] tabs = { "Batch Setup", "Single Setup", "Preview Baker" };
    
    // Batch Setup
    private string batchPrefabFolder = "Assets/Prefabs/Furniture";
    private bool batchSetupComponents = true;
    private bool batchCreateFurnitureData = true;
    private bool batchGeneratePreviews = true;
    private bool batchAutoAssign = true;
    
    // Single Setup
    private GameObject singleFurniture;
    
    // Preview Baker
    private string previewOutputFolder = "Assets/Resources/FurniturePreviews";
    private int previewSize = 512;
    private Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    private bool useTransparentBackground = false;
    private Vector3 cameraAngle = new Vector3(20, -30, 0);
    
    private Vector2 scrollPos;
    
    [MenuItem("Tools/Furniture Workflow Manager")]
    public static void ShowWindow()
    {
        FurnitureWorkflowWindow window = GetWindow<FurnitureWorkflowWindow>("Furniture Manager");
        window.minSize = new Vector2(450, 500);
    }
    
    void OnGUI()
    {
        // Header
        GUILayout.Space(10);
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 16;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("🛋️ Furniture Workflow Manager", headerStyle);
        GUILayout.Space(10);
        
        // Tabs
        selectedTab = GUILayout.Toolbar(selectedTab, tabs, GUILayout.Height(30));
        GUILayout.Space(10);
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        switch (selectedTab)
        {
            case 0:
                DrawBatchSetupTab();
                break;
            case 1:
                DrawSingleSetupTab();
                break;
            case 2:
                DrawPreviewBakerTab();
                break;
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    #region Batch Setup Tab
    void DrawBatchSetupTab()
    {
        GUILayout.Label("Batch Furniture Setup", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Process all furniture prefabs in one go! This will handle everything from component setup to preview generation.", MessageType.Info);
        GUILayout.Space(10);
        
        // Folder Selection
        EditorGUILayout.LabelField("Source", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        batchPrefabFolder = EditorGUILayout.TextField("Prefab Folder", batchPrefabFolder);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Prefab Folder", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                if (path.StartsWith(Application.dataPath))
                {
                    batchPrefabFolder = "Assets" + path.Substring(Application.dataPath.Length);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        // Options
        EditorGUILayout.LabelField("Processing Steps", EditorStyles.boldLabel);
        batchSetupComponents = EditorGUILayout.Toggle("Setup Components", batchSetupComponents);
        EditorGUI.indentLevel++;
        EditorGUILayout.HelpBox("Adds Rigidbody, Collider, Outline, PlacementHandler, Functionality, and NavMeshObstacle", MessageType.None);
        EditorGUI.indentLevel--;
        
        GUILayout.Space(5);
        
        batchCreateFurnitureData = EditorGUILayout.Toggle("Create FurnitureData Assets", batchCreateFurnitureData);
        EditorGUI.indentLevel++;
        EditorGUILayout.HelpBox("Creates ScriptableObject assets in Assets/Resources/Furniture", MessageType.None);
        EditorGUI.indentLevel--;
        
        GUILayout.Space(5);
        
        batchGeneratePreviews = EditorGUILayout.Toggle("Generate Preview Images", batchGeneratePreviews);
        if (batchGeneratePreviews)
        {
            EditorGUI.indentLevel++;
            previewOutputFolder = EditorGUILayout.TextField("Output Folder", previewOutputFolder);
            previewSize = EditorGUILayout.IntSlider("Preview Size", previewSize, 128, 1024);
            EditorGUI.indentLevel--;
        }
        
        GUILayout.Space(5);
        
        batchAutoAssign = EditorGUILayout.Toggle("Auto-Assign to Database", batchAutoAssign);
        EditorGUI.indentLevel++;
        EditorGUILayout.HelpBox("Automatically adds all created FurnitureData to the FurnitureDatabase", MessageType.None);
        EditorGUI.indentLevel--;
        
        GUILayout.Space(20);
        
        // Big Action Button
        GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
        if (GUILayout.Button("🚀 PROCESS ALL FURNITURE", GUILayout.Height(50)))
        {
            BatchProcessAllFurniture();
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(10);
        
        // Quick Stats
        if (AssetDatabase.IsValidFolder(batchPrefabFolder))
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { batchPrefabFolder });
            EditorGUILayout.HelpBox($"Found {guids.Length} prefabs in selected folder", MessageType.Info);
        }
    }
    
    void BatchProcessAllFurniture()
    {
        if (!AssetDatabase.IsValidFolder(batchPrefabFolder))
        {
            EditorUtility.DisplayDialog("Invalid Folder", $"The folder '{batchPrefabFolder}' does not exist.", "OK");
            return;
        }
        
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { batchPrefabFolder });
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("No Prefabs", $"No prefabs found in '{batchPrefabFolder}'", "OK");
            return;
        }
        
        bool confirm = EditorUtility.DisplayDialog("Confirm Batch Process",
            $"This will process {guids.Length} prefabs:\n\n" +
            $"• Setup Components: {(batchSetupComponents ? "Yes" : "No")}\n" +
            $"• Create FurnitureData: {(batchCreateFurnitureData ? "Yes" : "No")}\n" +
            $"• Generate Previews: {(batchGeneratePreviews ? "Yes" : "No")}\n" +
            $"• Auto-Assign: {(batchAutoAssign ? "Yes" : "No")}\n\n" +
            "Continue?",
            "Yes", "Cancel");
        
        if (!confirm) return;
        
        // Setup preview camera if needed
        Camera previewCamera = null;
        Light previewLight = null;
        if (batchGeneratePreviews)
        {
            CreateOutputFolder(previewOutputFolder);
            previewCamera = SetupPreviewCamera();
            previewLight = SetupPreviewLight();
        }
        
        int processedCount = 0;
        
        try
        {
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab == null) continue;
                
                EditorUtility.DisplayProgressBar("Processing Furniture",
                    $"Processing {prefab.name}... ({processedCount + 1}/{guids.Length})",
                    (float)processedCount / guids.Length);
                
                // Step 1: Setup Components
                if (batchSetupComponents)
                {
                    SetupPrefabComponents(prefab);
                }
                
                // Step 2: Create FurnitureData
                FurnitureData furnitureData = null;
                if (batchCreateFurnitureData)
                {
                    furnitureData = CreateFurnitureDataForPrefab(prefab);
                }
                
                // Step 3: Generate Preview
                if (batchGeneratePreviews && previewCamera != null)
                {
                    GeneratePreviewForPrefab(prefab, previewCamera);
                }
                
                // Step 4: Assign sprite to FurnitureData
                if (batchGeneratePreviews && batchCreateFurnitureData && furnitureData != null)
                {
                    AssignSpriteToData(furnitureData, prefab.name);
                }
                
                // Step 5: Add to database
                if (batchAutoAssign && batchCreateFurnitureData && furnitureData != null)
                {
                    AddToDatabase(furnitureData);
                }
                
                processedCount++;
            }
        }
        finally
        {
            // Cleanup
            if (previewCamera != null) DestroyImmediate(previewCamera.gameObject);
            if (previewLight != null) DestroyImmediate(previewLight.gameObject);
            
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        EditorUtility.DisplayDialog("Complete!",
            $"Successfully processed {processedCount} furniture prefabs!\n\n" +
            "All components setup, FurnitureData created, previews generated, and items added to database.",
            "Awesome!");
    }
    
    void SetupPrefabComponents(GameObject prefab)
    {
        string prefabPath = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
        
        try
        {
            // Add Rigidbody
            Rigidbody rb = prefabInstance.GetComponent<Rigidbody>();
            if (rb == null) rb = prefabInstance.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            
            // Add BoxCollider
            BoxCollider box = prefabInstance.GetComponent<BoxCollider>();
            if (box == null)
            {
                box = prefabInstance.AddComponent<BoxCollider>();
                // Auto-calculate bounds
                Bounds bounds = CalculateBounds(prefabInstance);
                box.center = bounds.center - prefabInstance.transform.position;
                box.size = bounds.size;
            }
            box.isTrigger = true;
            
            // Add PlacementHandler
            PlacementHandler handler = prefabInstance.GetComponent<PlacementHandler>();
            if (handler == null)
            {
                handler = prefabInstance.AddComponent<PlacementHandler>();
                handler.meshComponents = prefabInstance.GetComponentsInChildren<MeshRenderer>();
                handler.itemName = prefabInstance.name;
            }
            
            // Add NavMeshObstacle
            NavMeshObstacle obstacle = prefabInstance.GetComponent<NavMeshObstacle>();
            if (obstacle == null)
            {
                obstacle = prefabInstance.AddComponent<NavMeshObstacle>();
                obstacle.carving = true;
                obstacle.shape = NavMeshObstacleShape.Box;
                obstacle.center = box.center;
                obstacle.size = box.size;
            }
            
            // Set layer
            prefabInstance.layer = 7; // Furniture layer
            
            // Remove child colliders
            Collider[] childColliders = prefabInstance.GetComponentsInChildren<Collider>();
            foreach (Collider childCollider in childColliders)
            {
                if (childCollider.gameObject != prefabInstance)
                {
                    DestroyImmediate(childCollider);
                }
            }
            
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabInstance);
        }
    }
    
    FurnitureData CreateFurnitureDataForPrefab(GameObject prefab)
    {
        string folderPath = "Assets/Resources/Furniture";
        CreateOutputFolder(folderPath);
        
        string assetPath = $"{folderPath}/{prefab.name}.asset";
        
        // Check if exists
        FurnitureData existingData = AssetDatabase.LoadAssetAtPath<FurnitureData>(assetPath);
        if (existingData != null)
        {
            return existingData;
        }
        
        // Create new
        FurnitureData data = CreateInstance<FurnitureData>();
        data.itemName = prefab.name;
        data.prefab = prefab;
        
        AssetDatabase.CreateAsset(data, assetPath);
        return data;
    }
    
    void AssignSpriteToData(FurnitureData data, string prefabName)
    {
        string spritePath = $"{previewOutputFolder}/{prefabName}_Preview.png";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        
        if (sprite != null && data != null)
        {
            SerializedObject serializedData = new SerializedObject(data);
            SerializedProperty iconProperty = serializedData.FindProperty("icon");
            iconProperty.objectReferenceValue = sprite;
            serializedData.ApplyModifiedProperties();
            EditorUtility.SetDirty(data);
        }
    }
    #endregion
    
    #region Single Setup Tab
    void DrawSingleSetupTab()
    {
        GUILayout.Label("Single Furniture Setup", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Setup individual furniture items with manual control over each step.", MessageType.Info);
        GUILayout.Space(10);
        
        singleFurniture = (GameObject)EditorGUILayout.ObjectField("Furniture GameObject", singleFurniture, typeof(GameObject), true);
        
        GUILayout.Space(10);
        
        EditorGUI.BeginDisabledGroup(singleFurniture == null);
        
        if (GUILayout.Button("Setup Components", GUILayout.Height(30)))
        {
            SetupSingleComponents();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Create FurnitureData Asset", GUILayout.Height(30)))
        {
            CreateSingleFurnitureData();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Generate Preview Image", GUILayout.Height(30)))
        {
            GenerateSinglePreview();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Add to Database", GUILayout.Height(30)))
        {
            AddSingleToDatabase();
        }
        
        EditorGUI.EndDisabledGroup();
        
        GUILayout.Space(20);
        
        if (singleFurniture != null)
        {
            EditorGUILayout.HelpBox($"Selected: {singleFurniture.name}", MessageType.None);
        }
    }
    
    void SetupSingleComponents()
    {
        if (singleFurniture == null) return;
        
        GameObject obj = singleFurniture;
        
        // Check if it's a prefab
        bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(obj) || PrefabUtility.IsPartOfPrefabInstance(obj);
        
        if (isPrefab)
        {
            string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
            if (string.IsNullOrEmpty(prefabPath))
                prefabPath = AssetDatabase.GetAssetPath(obj);
            
            GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
            
            try
            {
                SetupComponentsOnObject(prefabInstance);
                PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                EditorUtility.DisplayDialog("Success", $"Components setup for {obj.name}", "OK");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabInstance);
            }
        }
        else
        {
            SetupComponentsOnObject(obj);
            EditorUtility.DisplayDialog("Success", $"Components setup for {obj.name}", "OK");
        }
    }
    
    void SetupComponentsOnObject(GameObject obj)
    {
        // Rigidbody
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) rb = obj.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        
        // BoxCollider
        BoxCollider box = obj.GetComponent<BoxCollider>();
        if (box == null)
        {
            box = obj.AddComponent<BoxCollider>();
            Bounds bounds = CalculateBounds(obj);
            box.center = bounds.center - obj.transform.position;
            box.size = bounds.size;
        }
        box.isTrigger = true;
        
        // PlacementHandler
        PlacementHandler handler = obj.GetComponent<PlacementHandler>();
        if (handler == null)
        {
            handler = obj.AddComponent<PlacementHandler>();
            handler.meshComponents = obj.GetComponentsInChildren<MeshRenderer>();
            handler.itemName = obj.name;
        }
        
        // NavMeshObstacle
        NavMeshObstacle obstacle = obj.GetComponent<NavMeshObstacle>();
        if (obstacle == null)
        {
            obstacle = obj.AddComponent<NavMeshObstacle>();
            obstacle.carving = true;
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.center = box.center;
            obstacle.size = box.size;
        }
        
        obj.layer = 7;
        
        Collider[] childColliders = obj.GetComponentsInChildren<Collider>();
        foreach (Collider childCollider in childColliders)
        {
            if (childCollider.gameObject != obj)
            {
                DestroyImmediate(childCollider);
            }
        }
        
        EditorUtility.SetDirty(obj);
    }
    
    void CreateSingleFurnitureData()
    {
        if (singleFurniture == null) return;
        
        GameObject prefab = singleFurniture;
        if (PrefabUtility.IsPartOfPrefabInstance(singleFurniture))
        {
            prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(singleFurniture);
        }
        
        FurnitureData data = CreateFurnitureDataForPrefab(prefab);
        AssetDatabase.SaveAssets();
        
        Selection.activeObject = data;
        EditorGUIUtility.PingObject(data);
        
        EditorUtility.DisplayDialog("Success", $"Created FurnitureData for {prefab.name}", "OK");
    }
    
    void GenerateSinglePreview()
    {
        if (singleFurniture == null) return;
        
        GameObject prefab = singleFurniture;
        if (PrefabUtility.IsPartOfPrefabInstance(singleFurniture))
        {
            prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(singleFurniture);
        }
        
        CreateOutputFolder(previewOutputFolder);
        Camera cam = SetupPreviewCamera();
        Light light = SetupPreviewLight();
        
        try
        {
            GeneratePreviewForPrefab(prefab, cam);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", $"Generated preview for {prefab.name}", "OK");
        }
        finally
        {
            if (cam != null) DestroyImmediate(cam.gameObject);
            if (light != null) DestroyImmediate(light.gameObject);
        }
    }
    
    void AddSingleToDatabase()
    {
        if (singleFurniture == null) return;
        
        GameObject prefab = singleFurniture;
        if (PrefabUtility.IsPartOfPrefabInstance(singleFurniture))
        {
            prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(singleFurniture);
        }
        
        string assetPath = $"Assets/Resources/Furniture/{prefab.name}.asset";
        FurnitureData data = AssetDatabase.LoadAssetAtPath<FurnitureData>(assetPath);
        
        if (data == null)
        {
            EditorUtility.DisplayDialog("Not Found", $"FurnitureData for {prefab.name} doesn't exist. Create it first.", "OK");
            return;
        }
        
        AddToDatabase(data);
        EditorUtility.DisplayDialog("Success", $"Added {prefab.name} to database", "OK");
    }
    #endregion
    
    #region Preview Baker Tab
    void DrawPreviewBakerTab()
    {
        GUILayout.Label("Preview Image Settings", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Configure how preview images are generated for furniture items.", MessageType.Info);
        GUILayout.Space(10);
        
        previewOutputFolder = EditorGUILayout.TextField("Output Folder", previewOutputFolder);
        previewSize = EditorGUILayout.IntSlider("Image Size", previewSize, 128, 1024);
        
        GUILayout.Space(10);
        
        useTransparentBackground = EditorGUILayout.Toggle("Transparent Background", useTransparentBackground);
        if (!useTransparentBackground)
        {
            backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
        }
        
        GUILayout.Space(10);
        
        cameraAngle = EditorGUILayout.Vector3Field("Camera Angle (Euler)", cameraAngle);
        
        GUILayout.Space(20);
        
        EditorGUILayout.HelpBox("These settings apply to preview generation in both Batch and Single modes.", MessageType.Info);
    }
    #endregion
    
    #region Helper Methods
    void CreateOutputFolder(string folderPath)
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            string[] folders = folderPath.Split('/');
            string currentPath = folders[0];
            
            for (int i = 1; i < folders.Length; i++)
            {
                string newPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = newPath;
            }
        }
    }
    
    Camera SetupPreviewCamera()
    {
        GameObject camObj = new GameObject("PreviewCamera");
        Camera cam = camObj.AddComponent<Camera>();
        
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = useTransparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
        cam.orthographic = false;
        cam.fieldOfView = 30f;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 100f;
        cam.cullingMask = 1 << 7;
        
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
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
        
        SetLayerRecursively(instance, 7);
        
        Bounds bounds = CalculateBounds(instance);
        PositionCamera(cam, bounds);
        
        RenderTexture rt = new RenderTexture(previewSize, previewSize, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 4;
        cam.targetTexture = rt;
        
        cam.Render();
        
        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(previewSize, previewSize, TextureFormat.RGBA32, false);
        screenshot.ReadPixels(new Rect(0, 0, previewSize, previewSize), 0, 0);
        screenshot.Apply();
        
        byte[] bytes = screenshot.EncodeToPNG();
        string filename = $"{previewOutputFolder}/{prefab.name}_Preview.png";
        File.WriteAllBytes(filename, bytes);
        
        RenderTexture.active = null;
        cam.targetTexture = null;
        rt.Release();
        DestroyImmediate(screenshot);
        DestroyImmediate(instance);
        
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
        float maxExtent = bounds.extents.magnitude;
        float distance = maxExtent / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        
        Quaternion rotation = Quaternion.Euler(cameraAngle);
        Vector3 direction = rotation * Vector3.forward;
        
        cam.transform.position = bounds.center - direction * distance * 1.5f;
        cam.transform.LookAt(bounds.center);
    }
    
    void AddToDatabase(FurnitureData data)
    {
        FurnitureDatabase database = FindFirstObjectByType<FurnitureDatabase>();
        
        if (database == null)
        {
            Debug.LogWarning("FurnitureDatabase not found in scene.");
            return;
        }
        
        SerializedObject serializedDB = new SerializedObject(database);
        SerializedProperty furnitureArray = serializedDB.FindProperty("allFurniture");
        
        // Check if already in database
        for (int i = 0; i < furnitureArray.arraySize; i++)
        {
            if (furnitureArray.GetArrayElementAtIndex(i).objectReferenceValue == data)
            {
                return; // Already exists
            }
        }
        
        // Add to array
        furnitureArray.arraySize++;
        furnitureArray.GetArrayElementAtIndex(furnitureArray.arraySize - 1).objectReferenceValue = data;
        
        serializedDB.ApplyModifiedProperties();
        EditorUtility.SetDirty(database);
    }
    #endregion
}
#endif