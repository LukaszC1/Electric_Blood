using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum StageEventType
{
    SpawnEnemyWave,
    SpawnEnemy,
    WinStage
}

[Serializable]
public class StageEvent
{
    public StageEventType eventType;
    public float time;
    public GameObject enemyToSpawn;
    public int amount;
    public float length;
}



[CreateAssetMenu]
public class StageData : ScriptableObject
{
    public List<StageEvent> events;
}
