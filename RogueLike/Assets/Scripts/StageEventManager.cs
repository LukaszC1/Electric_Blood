using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEventManager : MonoBehaviour
{
    [SerializeField] StageData stageData;
    [SerializeField] EnemiesManager enemiesManager;
    [SerializeField] PlayerWinManager playerWinManager;

    StageTime stageTime;
    int eventIndexer;


    private void Awake()
    {
        stageTime = GetComponent<StageTime>();
    }

    private void Update()
    {
        if (eventIndexer >= stageData.events.Count) { return; }

        if(stageTime.time > stageData.events[eventIndexer].time*60)
        {
            switch (stageData.events[eventIndexer].eventType)
            {
                case StageEventType.SpawnEnemyWave:
                    enemiesManager.AddWaveToSpawn(stageData.events[eventIndexer].enemyToSpawn, stageData.events[eventIndexer].amount, stageData.events[eventIndexer].length);
                    break;
                case StageEventType.SpawnEnemy:
                    enemiesManager.SpawnEnemy(stageData.events[eventIndexer].enemyToSpawn);
                    break;
                case StageEventType.WinStage:
                    WinStage();
                    break;
            }
            eventIndexer++;
        }
    }

    private void WinStage()
    {
        playerWinManager.Win();
    }
}
