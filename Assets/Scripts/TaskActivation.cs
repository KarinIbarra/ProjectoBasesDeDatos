    using UnityEngine;
    using System.Collections.Generic;

    public class RandomTaskActivator : MonoBehaviour
    {
        [Header("Configuración")]
        public int tasksToActivate = 3;

        void Start()
        {
            // 1. Buscamos todas las tareas en la escena (incluyendo las desactivadas)
            TaskInteraction[] allTasks = Resources.FindObjectsOfTypeAll<TaskInteraction>();
            List<TaskInteraction> hiddenTasks = new List<TaskInteraction>();

            foreach (TaskInteraction task in allTasks)
            {
                // Solo añadimos las que son parte de la escena y están ocultas
                if (task.gameObject.scene.name != null && !task.gameObject.activeSelf)
                {
                    hiddenTasks.Add(task);
                }
            }

            // 2. Verificamos si hay suficientes
            if (hiddenTasks.Count < tasksToActivate)
            {
                Debug.LogWarning($"Solo se encontraron {hiddenTasks.Count} tareas ocultas. Se activarán todas.");
                tasksToActivate = hiddenTasks.Count;
            }

            // 3. Activación aleatoria sin repetir
            for (int i = 0; i < tasksToActivate; i++)
            {
                int randomIndex = Random.Range(0, hiddenTasks.Count);
                TaskInteraction selectedTask = hiddenTasks[randomIndex];

                selectedTask.gameObject.SetActive(true);

                // La eliminamos de la lista para no volver a elegirla
                hiddenTasks.RemoveAt(randomIndex);
            }

            Debug.Log($"Se han activado {tasksToActivate} tareas aleatoriamente.");
        }
    }
