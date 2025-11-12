#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.AI;

[CustomEditor(typeof(FurnitureSetup))]
public class FurnitureSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FurnitureSetup setup = (FurnitureSetup)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Setup Furniture Components", GUILayout.Height(25)))
        {
            SetupComponents(setup);
        }
        
        EditorGUILayout.Space();

        if (GUILayout.Button("Update NavMeshObstacle", GUILayout.Height(25)))
        {
            UpdateNavMesh(setup);
        }
        
        EditorGUILayout.Space();

        if (GUILayout.Button("Create FurnitureData Asset", GUILayout.Height(25)))
        {
            CreateFurnitureDataAsset(setup);
        }
        
        EditorGUILayout.Space();

        if (GUILayout.Button("Remove Setup", GUILayout.Height(25)))
        {
            DestroyImmediate(setup);
        }
    }
    
    private void SetupComponents(FurnitureSetup setup)
    {
        GameObject obj = setup.gameObject;
        
        // Add Rigidbody
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;
        
        // Add BoxCollider
        BoxCollider box = obj.GetComponent<BoxCollider>();
        if (box == null)
        {
            box = obj.AddComponent<BoxCollider>();
        }
        box.size = setup.colliderSize;
        box.center = setup.colliderCenter;
        box.isTrigger = true;
        
        // Add outline
        Outline outline = obj.GetComponent<Outline>();
        if (outline == null)
        {
            Type outlineType = Type.GetType("Outline, Assembly-CSharp");
            if (outlineType != null)
            {
                outline = obj.AddComponent<Outline>();
            }
            else
            {
                Debug.LogError("Outline script not found");
            }
        }
        if (outline != null)
        {
            outline.enabled = false;
            outline.OutlineColor = setup.outlineColor;
        }
        
        // Add placement handler
        var handler = obj.AddComponent<PlacementHandler>();
        handler.meshComponents = obj.GetComponentsInChildren<MeshRenderer>();
        handler.itemName = obj.name;

        // Add functionality
        if (setup.hasFunctionality)
        {
            BaseFunctionality functionality = null;
            if (setup.baseFunctionality != null)
            {
                Type funcType = setup.baseFunctionality.GetType();
                if (funcType != null && obj.GetComponent(funcType) == null)
                {
                    functionality = (BaseFunctionality)obj.AddComponent(funcType);
                }
            }
            else
            {
                Debug.LogError("Functionality script not found, adding base functionality");
                if (obj.GetComponent<BaseFunctionality>() == null)
                {
                    functionality = obj.AddComponent<BaseFunctionality>();
                }
            }

            if (functionality != null)
            {
                functionality.price = setup.price;
            }
        }
        
        // Add nav mesh obstacle
        NavMeshObstacle obstacle = obj.AddComponent<NavMeshObstacle>();
        obstacle.carving = true;
        obstacle.shape = NavMeshObstacleShape.Box;
        obstacle.center = setup.colliderCenter;
        obstacle.size = setup.colliderSize;

        // Cleanup
        // Add to furniture layer
        obj.layer = 7;
        
        // Go through all children and remove box colliders
        Collider[] childColliders = obj.GetComponentsInChildren<Collider>();
        foreach (Collider childCollider in childColliders)
        {
            if (childCollider.gameObject != obj)
            {
                DestroyImmediate(childCollider);
            }
        }
        
        EditorUtility.SetDirty(obj);
        Debug.Log($"Furniture setup completed for {obj.name}");
    }
    private void UpdateNavMesh(FurnitureSetup setup)
    {
        GameObject obj = setup.gameObject;
        NavMeshObstacle navMeshObstacle = obj.GetComponent<NavMeshObstacle>();
        BoxCollider box = obj.GetComponent<BoxCollider>();
        if (!box)
        {
            EditorUtility.DisplayDialog("Missing BoxCollider",
                    "Add a BoxCollider component before updating the NavMeshObstacle",
                    "OK");
            return;
        }
        if (!navMeshObstacle)
        {
            EditorUtility.DisplayDialog("Missing NavMeshObstacle",
                    "Add a NavMeshObstacle component before updating the NavMeshObstacle",
                    "OK");
            return;
        }
        navMeshObstacle.center = box.center;
        navMeshObstacle.size = box.size;
    }
    private void CreateFurnitureDataAsset(FurnitureSetup setup)
    {
        GameObject obj = setup.gameObject;
        
        // Check if this is a prefab or instance
        GameObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(obj);
        if (prefabRoot == null)
        {
            // Not a prefab instance, check if it's a prefab asset
            if (!PrefabUtility.IsPartOfPrefabAsset(obj))
            {
                EditorUtility.DisplayDialog("Not a Prefab", 
                    "Create a prefab of this GameObject first before creating FurnitureData", 
                    "OK");
                return;
            }
            prefabRoot = obj;
        }
        else
        {
            prefabRoot = PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj);
        }
        
        // Create the FurnitureData asset
        FurnitureData data = CreateInstance<FurnitureData>();
        data.itemName = obj.name;
        data.prefab = prefabRoot;
        
        // Ensure the directory exists
        string folderPath = "Assets/Resources/Furniture";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            // Create Resources folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            // Create Furniture folder
            AssetDatabase.CreateFolder("Assets/Resources", "Furniture");
        }
        
        // Create the asset
        string assetPath = $"{folderPath}/{obj.name}.asset";
        
        // Check if asset already exists
        bool isNewAsset = true;
        if (AssetDatabase.LoadAssetAtPath<FurnitureData>(assetPath) != null)
        {
            bool overwrite = EditorUtility.DisplayDialog("Asset Exists", 
                $"FurnitureData for '{obj.name}' already exists. Overwrite?", 
                "Yes", "No");
            
            if (!overwrite)
            {
                return;
            }
            isNewAsset = false;
        }
        
        AssetDatabase.CreateAsset(data, assetPath);
        AssetDatabase.SaveAssets();
        
        // Add to database if it's a new asset
        if (isNewAsset)
        {
            AddToDatabase(data);
        }
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = data;
        
        Debug.Log($"Created FurnitureData asset at {assetPath}");
    }
    
    private void AddToDatabase(FurnitureData data)
    {
        // Find the FurnitureDatabase in the scene
        FurnitureDatabase database = FindFirstObjectByType<FurnitureDatabase>();
        
        if (database == null)
        {
            Debug.LogWarning("FurnitureDatabase not found in scene. Please add the FurnitureData manually.");
            return;
        }
        
        // Use SerializedObject to modify the array
        SerializedObject serializedDB = new SerializedObject(database);
        SerializedProperty furnitureArray = serializedDB.FindProperty("allFurniture");
        
        // Check if already in database
        for (int i = 0; i < furnitureArray.arraySize; i++)
        {
            if (furnitureArray.GetArrayElementAtIndex(i).objectReferenceValue == data)
            {
                Debug.Log($"{data.itemName} is already in the database.");
                return;
            }
        }
        
        // Add to array
        furnitureArray.arraySize++;
        furnitureArray.GetArrayElementAtIndex(furnitureArray.arraySize - 1).objectReferenceValue = data;
        
        serializedDB.ApplyModifiedProperties();
        EditorUtility.SetDirty(database);
        
        Debug.Log($"Added {data.itemName} to FurnitureDatabase!");
    }
}
#endif