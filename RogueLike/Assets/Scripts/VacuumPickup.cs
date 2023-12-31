using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Script attached to the pickup object that is used to vacuum gems from the map.
/// </summary>
public class VacuumPickup : NetworkBehaviour, iPickUpObject
{
    //Private fields
    private float speed = 2.3f;
    private float speed2 = 3;
    Transform targetDestination;
    private float timer = 0.2f;

    /// <summary>
    /// On pickup method implementation, vacuum the gems from the map.
    /// </summary>
    /// <param name="character"></param>
    public void OnPickUp(Character character)
    {
        if (!IsOwner) return;
        character.VacuumGems();
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
}
