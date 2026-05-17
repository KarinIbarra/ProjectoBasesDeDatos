using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Progreso")]
    public int tasksCompleted = 0;
    public int totalTasks = 3;

    [Header("Escape Phase")]
    public SlidingDoor exitDoorScript; // Script de la puerta deslizante
    public GameObject guideArrow;     // Objeto de la flecha guía
    public float speedMultiplier = 1.5f;

    private bool isEscapePhaseActive = false; // Control para activar solo una vez
    public bool IsEscapeActive()
    {
        return isEscapePhaseActive;
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadConfigFromDatabase();
    }

    void LoadConfigFromDatabase()
    {
        if (DatabaseManager.Instance != null)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            LevelConfig config = DatabaseManager.Instance.GetLevelConfig(sceneName);
            
            totalTasks = config.totalTasks;
            speedMultiplier = config.speedMultiplier;
            
            Debug.Log($"Configuración cargada de DB: Tareas={totalTasks}, Multiplicador={speedMultiplier}");
        }
        else
        {
            Debug.LogWarning("DatabaseManager no encontrado. Usando valores por defecto.");
        }
    }

    void Update()
    {
     
        // Al presionar Z,abrir la puerta del final
        if (Input.GetKeyDown(KeyCode.Z) && !isEscapePhaseActive)
        {
            Debug.Log("<color=yellow>DEBUG: Tecla Z presionada. Forzando fase de escape...</color>");
            StartEscapePhase();
        }
    }

    public void OnTaskCompleted()
    {
        tasksCompleted++;
        if (tasksCompleted >= totalTasks && !isEscapePhaseActive)
        {
            StartEscapePhase();
        }
    }

    public void StartEscapePhase()
    {
        if (isEscapePhaseActive) return;

        isEscapePhaseActive = true;

        // Abrir la puerta 
        if (exitDoorScript != null)
        {
            exitDoorScript.Open();
        }

        // Mostrar la flecha
        if (guideArrow != null)
        {
            guideArrow.SetActive(true);
        }

        // Potenciar a los enemigos
        // buscar todos los enemigos que heredan de la clase Enemy
        Enemy[] allEnemies = Object.FindObjectsOfType<Enemy>();
        foreach (Enemy e in allEnemies)
        {
            e.EnableEscapeMode(speedMultiplier);
        }

        //  Exportar datos 
        if (DataExporter.Instance != null)
        {
            DataExporter.Instance.ExportAllData();
        }

        Debug.Log("¡FASE DE ESCAPE ACTIVADA! Los enemigos son más rápidos y se han exportado los datos.");
    }
   
}