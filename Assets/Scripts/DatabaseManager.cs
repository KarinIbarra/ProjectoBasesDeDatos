using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;
    private string dbPath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            dbPath = "URI=file:" + Path.Combine(Application.streamingAssetsPath, "myproject.db");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IDbConnection GetConnection()
    {
        return new SqliteConnection(dbPath);
    }

    // Ejemplo de consulta simple para obtener la configuración de un nivel
    public LevelConfig GetLevelConfig(string sceneName)
    {
        LevelConfig config = new LevelConfig();
        using (IDbConnection dbConnection = GetConnection())
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT total_tareas_requeridas, multiplicador_velocidad_escape FROM nivel WHERE escena_unity LIKE '%" + sceneName + "%'";
                dbCmd.CommandText = sqlQuery;
                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        config.totalTasks = reader.GetInt32(0);
                        config.speedMultiplier = reader.GetFloat(1);
                    }
                }
            }
        }
        return config;
    }
}

[System.Serializable]
public class LevelConfig
{
    public int totalTasks = 3;
    public float speedMultiplier = 1.5f;
}
