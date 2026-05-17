using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RandomPlayerSpawn : MonoBehaviour //no funciona bien, toca revisarlo mas tarde, tras morir o finalizar el nivel deja de mandar al jugador al spawn aleatoriamente
{
    public Transform[] spawnPoints;

    public float raycastHeight = 10f;
    public float groundOffset = 0.1f;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SpawnAfterLoad());
    }

    IEnumerator SpawnAfterLoad()
    {
        yield return null;

        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null) return;

        Transform selectedSpawn =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        Vector3 finalPosition = selectedSpawn.position;

        RaycastHit hit;

        Vector3 rayOrigin =
            selectedSpawn.position + Vector3.up * raycastHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 100f))
        {
            finalPosition.y = hit.point.y + groundOffset;
        }

        CharacterController cc =
            player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.transform.position = finalPosition;

        if (cc != null)
            cc.enabled = true;
    }
}