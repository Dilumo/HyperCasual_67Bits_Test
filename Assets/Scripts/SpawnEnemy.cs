using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 0, spawnInterval);
    }

    public void Spawn()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }

}
