    using System.IO;
    using UnityEngine;

    public class SaverExporter : MonoBehaviour
    {
        public PlayerData currentPlayerData;
        public void SaveGame()
        {
            // Update currentPlayerData with actual game state
            currentPlayerData.gold = 100; // Example
            currentPlayerData.health = 75.5f; // Example
            currentPlayerData.playerName = "Hero"; // Example

            string json = JsonUtility.ToJson(currentPlayerData);
            string filePath = Path.Combine(Application.persistentDataPath, "playerSave.json");
            File.WriteAllText(filePath, json);
            Debug.Log("Game saved to: " + filePath);
        }
    }
