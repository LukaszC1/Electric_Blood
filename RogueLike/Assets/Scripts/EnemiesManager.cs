using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemiesSpawnWave
{
    public GameObject enemyPrefab;
    public int amount;
    public float length;


    public EnemiesSpawnWave(GameObject enemyPrefab, int amount, float length)
    {
        this.enemyPrefab = enemyPrefab;
        this.amount = amount;
        this.length = length;
    }

}


public class EnemiesManager : MonoBehaviour
{
    [SerializeField] Vector2 spawnArea;
    [SerializeField] GameObject player;
    public List<GameObject> enemyList;

    public List<EnemiesSpawnWave> enemiesSpawnWaveList;


    private void Update()
    { 
        enemyList.RemoveAll(x => x == null);

    }
    private void LateUpdate()
    {
        if (Time.timeScale == 1 && enemiesSpawnWaveList.Count > 0)
            ProcessSpawn();
    }

    private void ProcessSpawn()
    {
        if (enemiesSpawnWaveList == null) { return; }

        for (int i = 0; i < enemiesSpawnWaveList.Count; i++)
        {
            enemiesSpawnWaveList[i].length -= Time.deltaTime;
            while (enemiesSpawnWaveList[i].amount > enemyList.Count)
            {
                SpawnEnemy(enemiesSpawnWaveList[UnityEngine.Random.Range(0, enemiesSpawnWaveList.Count)].enemyPrefab);
            }
            if (enemiesSpawnWaveList[i].length <= 0)
                enemiesSpawnWaveList.RemoveAt(i);
        }

    }

    public void AddWaveToSpawn(GameObject enemyToSpawn, int amount, float length)
    {
        EnemiesSpawnWave wave = new EnemiesSpawnWave(enemyToSpawn, amount, length);

        if (enemiesSpawnWaveList == null) { enemiesSpawnWaveList = new List<EnemiesSpawnWave>(); }

        enemiesSpawnWaveList.Add(wave);
    }


    public void SpawnEnemy(GameObject enemyToSpawn)
    {
        Vector3 position = GenerateRandomPosition();
        while (CheckForCollision(position))
            position = GenerateRandomPosition();
        GameObject newEnemy;
        newEnemy = Instantiate(enemyToSpawn);


        newEnemy.transform.position = position;
        newEnemy.GetComponent<Enemy>().SetTarget(player);
        newEnemy.transform.parent = transform;
        enemyList.Add(newEnemy);
    }

    private Vector3 GenerateRandomPosition()
    {
        Vector3 position = new Vector3();

        float f = UnityEngine.Random.value > 0.5f ? -1f : 1f;

        if (UnityEngine.Random.value > 0.5f)
        {
            position.x = UnityEngine.Random.Range(-spawnArea.x, spawnArea.x);
            position.y = spawnArea.y * f;
        }
        else
        {
            position.y = UnityEngine.Random.Range(-spawnArea.y, spawnArea.y);
            position.x = spawnArea.x * f;
        }
        position.x += player.transform.position.x;
        position.y += player.transform.position.y;
        position.z = 0;

        return position;
    }
    private bool CheckForCollision(Vector3 position)
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collision in collisions)
        {
            if (collision.transform.name == "buildings")
            {
                return true;
            }
        }
        return false;
    }
}
