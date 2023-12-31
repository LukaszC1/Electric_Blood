using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Class handling the events of a stage.
/// </summary>
public class StageEventManager : NetworkBehaviour
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
        if (!IsServer) return;
        if (eventIndexer >= stageData.events.Count) { return; }

        if(stageTime.time.Value > stageData.events[eventIndexer].time*60)
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
                    GameManager.Instance.TogglePauseGame(false);
                    WinStageClientRpc();
                    break;
            }
            eventIndexer++;
        }
    }

    [ClientRpc]
    private void WinStageClientRpc()
    {
        playerWinManager.Win();
    }
}
