using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroyAfterTime : NetworkBehaviour
{
    [SerializeField] float timeToDestroy = 0.8f;
    float timer;

    private void OnEnable()
    {
        timer = timeToDestroy;
    }

    private void LateUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {

            if (gameObject.GetComponent<NetworkObject>().IsSpawned)
            {
                gameObject.GetComponent<NetworkObject>().Despawn();
                Debug.Log("Despawned");
                NetworkLog.LogInfoServer("Spawned");
            }
            Destroy(gameObject);
        }
    }
}
