using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Pick up object for coins.
/// </summary>
public class CoinPickup : NetworkBehaviour, iPickUpObject
{
    private float speed = 2.3f;
    private float speed2 = 3;
    Transform targetDestination;
    private float timer = 0.2f;

    /// <summary>
    /// Method which adds the coins to the players after pickup.
    /// </summary>
    /// <param name="character"></param>
    public void OnPickUp(Character character)
    {
        if (!IsOwner) return;

        AddCoinsServerRpc();
        DestroyObjectServerRpc();
    }

    private void Update()
    {
        if (targetDestination != null && Time.timeScale == 1)
        {
            timer -= Time.deltaTime;
            if (timer >= 0)
            {
                Vector3 direction = (targetDestination.position - transform.position).normalized;
                transform.position -= speed2 * Time.deltaTime * direction.normalized;
                speed2 *= 0.99f;
            }
            else
            {
                Vector3 direction = (targetDestination.position - transform.position).normalized;
                transform.position += speed * Time.deltaTime * direction.normalized;
                speed *= 1.001f;
            }
        }
    }

    public void SetTargetDestination(Transform destination)
    {
        targetDestination = destination;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc()
    {
        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddCoinsServerRpc()
    {
       AddCoinsClientRpc();
    }

    [ClientRpc]
    private void AddCoinsClientRpc()
    {
        PersistentUpgrades.Instance?.AddCoins(50);
    }
}
