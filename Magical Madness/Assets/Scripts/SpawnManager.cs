using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
    public SpawnPoint[] spawnpoints;

    private void Awake()
    {
        instance = this;
        spawnpoints = FindObjectsOfType<SpawnPoint>(); // Changed to FindObjectsOfType to find all SpawnPoint objects in the scene
    }

    public Transform GetSpawnPoint()
    {
        // yaha kuch logic lagana padega taaki koi 2 object same jagah respawn na hoye
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }
}
