using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class DataExporter : MonoBehaviour
{
    public static DataExporter Instance;

    void Awake()
    {
        Instance = this;
    }

    // Estructura para JSON (Datos del Jugador)
    [System.Serializable]
    public class PlayerData
    {
        public string username;  //todo esto es relleno, no creo que vaya a añadir algo que pregunte el username
        public int initialHearts; //esto es por si hago el evento de menos corazones al empezar
        public float walkSpeed;
        public float runSpeed;
    }

    // Estructura para XML (Resumen del Nivel)
    [System.Serializable]
    public class LevelSummary
    {
        public string levelName;
        public int tasksRequired;
        public float escapeSpeedMultiplier;
    }

    public void ExportAllData()
    {
        ExportToJson();
        ExportToXml();
    }

    private void ExportToJson()
    {
        PlayerData data = new PlayerData
        {
            username = "Jugador_Prueba", // Esto podría venir de la DB
            initialHearts = 3,
            walkSpeed = 5.0f,
            runSpeed = 9.0f
        };

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "player_data.json");
        File.WriteAllText(path, json);
        Debug.Log("JSON Exportado a: " + path);
    }

    private void ExportToXml()
    {
        LevelSummary summary = new LevelSummary
        {
            levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            tasksRequired = LevelManager.Instance.totalTasks,
            escapeSpeedMultiplier = LevelManager.Instance.speedMultiplier
        };

        XmlSerializer serializer = new XmlSerializer(typeof(LevelSummary));
        string path = Path.Combine(Application.persistentDataPath, "level_summary.xml");
        
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, summary);
        }
        Debug.Log("XML Exportado a: " + path);
    }
}
