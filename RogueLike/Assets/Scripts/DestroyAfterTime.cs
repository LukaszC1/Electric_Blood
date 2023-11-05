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
        if (!IsOwner) return;

        timer -= Time.deltaTime;
        if (timer < 0f)
        {

            if (IsHost)
            {
                gameObject.GetComponent<NetworkObject>().Despawn();
                Destroy(gameObject);
                Debug.Log("Despawned");               
            }
            else
            {
                destroyWhipAttackServerRpc();
            }      
        }
    }

    [ServerRpc]
    private void destroyWhipAttackServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
        Debug.Log("Despawned by server");
    }
}

