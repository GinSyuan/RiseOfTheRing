using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [Header("List of Level Prefabs (in the exact order you want them to spawn)")]
    public GameObject[] levelPrefabs;    

    [Header("Height of Each Level in World Units")]
    public float levelHeight = 10f;      

    [Header("Number of Initial Levels")]
    public int initialLevels = 6;         // Spawn all 6 prefabs at game start: indices 0..5.

    private float highestY = 0f;          // Keeps track of the highest Y position where a level has been spawned.
    private int nextIndex = 0;            // The index of the next prefab to spawn.

    private void Start()
    {
        // Spawn the first batch of levels: indices 0,1,2,3,4,5 in that exact sequence.
        for (int i = 0; i < initialLevels; i++)
        {
            float spawnY = i * levelHeight;
            SpawnLevelAtY(spawnY);
        }

        // After spawning 6 levels (0..5), the highestY sits at (5 * levelHeight).
        highestY = (initialLevels - 1) * levelHeight;

        // Set nextIndex = 1 so that subsequent spawns skip prefab 0.
        //    That means the next call to SpawnLevelAtY(...) will use levelPrefabs[1].
        nextIndex = 1;
    }

    private void Update()
    {
        // If camera has climbed close enough to the topmost spawned level, spawn another one.
        if (Camera.main.transform.position.y + 10f > highestY)
        {
            float spawnY = highestY + levelHeight;
            SpawnLevelAtY(spawnY);
        }
    }


    /// Instantiates the prefab at levelPrefabs[nextIndex] in Inspector order, 
    /// then advances nextIndex so it stays between 1 and (length-1).
    private void SpawnLevelAtY(float yPos)
    {
        // Choose the prefab at current nextIndex
        int index = nextIndex;

        // Advance nextIndex so that it loops from 1 through levelPrefabs.Length - 1.
        // In other words: if nextIndex = 5, nextIndex becomes 1 (skipping 0).
        nextIndex++;
        if (nextIndex >= levelPrefabs.Length)
        {
            // Wrap around to 1 (never use index 0 again after initial spawn).
            nextIndex = 1;
        }

        Vector3 spawnPosition = new Vector3(0f, yPos, 0f);
        Debug.Log($"Spawning level '{levelPrefabs[index].name}' at Y = {yPos}");

        // Instantiate the chosen level prefab so its bottom sits at world Y = yPos.
        Instantiate(levelPrefabs[index], spawnPosition, Quaternion.identity);

        // Update highestY to reflect that we've now spawned at yPos.
        highestY = yPos;
    }
}
