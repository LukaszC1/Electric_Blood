using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Enum holding event types for stages.
/// </summary>
public enum StageEventType
{
    SpawnEnemyWave,
    SpawnEnemy,
    WinStage
}

/// <summary>
/// Stage event class. Holds data for stage events.
/// </summary>
[Serializable]
public class StageEvent
{
    public StageEventType eventType;
    public float time;
    public GameObject enemyToSpawn;
    public int amount;
    public float length;
}

/// <summary>
/// Scriptable object holding a list of stage events.
/// </summary>
[CreateAssetMenu]
public class StageData : ScriptableObject
{
    public List<StageEvent> events;
}
