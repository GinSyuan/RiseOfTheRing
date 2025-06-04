// LevelSpawner.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSpawner : MonoBehaviour
{
    [Header("List of Level Prefabs (in the exact order you want them to spawn)")]
    public GameObject[] levelPrefabs;        // Array of level prefabs to spawn in sequence

    [Header("Height of Each Level in World Units")]
    public float levelHeight = 10f;          // Vertical spacing between each spawned level

    [Header("Number of Initial Levels")]
    public int initialLevels = 6;            // Spawn the first N prefabs at game start

    [Header("Player Transform (used to trigger new level spawn)")]
    public Transform player;                 // Reference to the player’s Transform

    private float highestY = 0f;             // Tracks the highest Y position where a level has been spawned
    private int nextIndex = 0;               // Index of the next prefab to spawn

    private void Start()
    {
        // Spawn the initial set of levels using indices 0..initialLevels-1
        for (int i = 0; i < initialLevels; i++)
        {
            float spawnY = i * levelHeight;
            SpawnLevelAtY(spawnY);
        }

        // After spawning initialLevels, highestY is at (initialLevels - 1) * levelHeight
        highestY = (initialLevels - 1) * levelHeight;

        // Set nextIndex to 1 so that subsequent spawns skip the very first prefab (index 0)
        nextIndex = 1;
    }

    private void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is not set on LevelSpawner.");
            return;
        }

        // When the player’s Y position plus a buffer exceeds highestY, spawn the next level
        // Here we use half a levelHeight as a buffer so the next segment appears slightly before the player reaches the very top.
        float triggerHeight = player.position.y + (levelHeight * 0.5f);
        if (triggerHeight > highestY)
        {
            float spawnY = highestY + levelHeight;
            SpawnLevelAtY(spawnY);
        }
    }

    /// <summary>
    /// Instantiates the prefab at levelPrefabs[nextIndex], then moves it to this scene so it is unloaded correctly.
    /// </summary>
    private void SpawnLevelAtY(float yPos)
    {
        int index = nextIndex;

        // Update nextIndex so it cycles through 1..(length-1) (never reuse index 0 again)
        nextIndex++;
        if (nextIndex >= levelPrefabs.Length)
            nextIndex = 1;

        Vector3 spawnPosition = new Vector3(0f, yPos, 0f);
        Debug.Log($"Spawning level '{levelPrefabs[index].name}' at Y = {yPos:F2}");

        GameObject levelInstance = Instantiate(levelPrefabs[index], spawnPosition, Quaternion.identity);

        // Move the instantiated object to the same scene as this script, ensuring proper unloading
        Scene thisScene = gameObject.scene;
        SceneManager.MoveGameObjectToScene(levelInstance, thisScene);

        // Update highestY to the Y position of the level we just spawned
        highestY = yPos;
    }
}
