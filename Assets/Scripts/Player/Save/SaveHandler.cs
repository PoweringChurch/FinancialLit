    using System.IO;
    using UnityEngine;

    public class SaveHandler : MonoBehaviour
    {
        public PlayerData currentPlayerData;
        public void ExportSaved()
        {
            // Update currentPlayerData with actual game state

            string json = JsonUtility.ToJson(currentPlayerData);
            string filePath = Path.Combine(Application.persistentDataPath, "playerSave.json");
            File.WriteAllText(filePath, json);
            Debug.Log("Game saved to: " + filePath);
        }
    }
