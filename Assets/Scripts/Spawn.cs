using UnityEngine;
using System.Collections.Generic;

public class RandomEnemySpawner : MonoBehaviour
{
    [Header("Todos los enemigos de la escena")]
    public List<GameObject> enemies = new List<GameObject>();

    [Header("Puntos de spawn")]
    public List<Transform> spawnPoints = new List<Transform>();

    void Start()
    {
        // Verificaciones básicas
        if (enemies.Count < 2)
        {
            Debug.LogWarning("Necesitas al menos 2 enemigos.");
            return;
        }

        if (spawnPoints.Count < 2)
        {
            Debug.LogWarning("Necesitas al menos 2 puntos de spawn.");
            return;
        }

        // Crear copias temporales para no repetir
        List<GameObject> availableEnemies = new List<GameObject>(enemies);
        List<Transform> availableSpawns = new List<Transform>(spawnPoints);

        // Elegir 2 enemigos aleatorios
        GameObject enemy1 = GetRandomEnemy(availableEnemies);
        GameObject enemy2 = GetRandomEnemy(availableEnemies);

        // Elegir 2 spawns aleatorios
        Transform spawn1 = GetRandomSpawn(availableSpawns);
        Transform spawn2 = GetRandomSpawn(availableSpawns);

        // Posicionar enemigos
        SpawnEnemy(enemy1, spawn1);
        SpawnEnemy(enemy2, spawn2);

        Debug.Log("Enemigos seleccionados: " + enemy1.name + " y " + enemy2.name);
    }

    GameObject GetRandomEnemy(List<GameObject> list)
    {
        int index = Random.Range(0, list.Count);
        GameObject selected = list[index];
        list.RemoveAt(index); // evita repetir
        return selected;
    }

    Transform GetRandomSpawn(List<Transform> list)
    {
        int index = Random.Range(0, list.Count);
        Transform selected = list[index];
        list.RemoveAt(index); // evita repetir
        return selected;
    }

    void SpawnEnemy(GameObject enemy, Transform spawnPoint)
    {
        UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null)
        {
            // 1. Desactivamos el agente temporalmente para asegurar el movimiento
            agent.enabled = false;

            // 2. Posicionamos el transform
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = spawnPoint.rotation;

            // 3. Reactivamos y usamos Warp para posicionar al agente en el NavMesh
            agent.enabled = true;
            agent.Warp(spawnPoint.position);
        }
        else
        {
            // Si por alguna razón no tiene agente, movemos el transform normalmente
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = spawnPoint.rotation;
        }

        enemy.SetActive(true);
    }
}
