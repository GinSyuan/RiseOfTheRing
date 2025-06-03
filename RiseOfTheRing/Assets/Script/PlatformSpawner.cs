using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject[] platformPrefabs;
    public float verticalGap = 2.5f;
    public int initialPlatforms = 5;

    private float highestY;

    private void Start()
    {
        for (int i = 0; i < initialPlatforms; i++)
        {
            SpawnPlatform(i * verticalGap);
        }
    }

    private void Update()
    {
        if (Camera.main.transform.position.y + 10f > highestY)
        {
            SpawnPlatform(highestY + verticalGap);
        }
    }

    public void SpawnPlatform(float yPos)
    {
        int index = Random.Range(0, platformPrefabs.Length);
        Vector3 spawnPos = new Vector3(0f, yPos, 0f);
        Instantiate(platformPrefabs[index], spawnPos, Quaternion.identity);
        highestY = Mathf.Max(highestY, yPos);
    }
}
