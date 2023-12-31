using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Class handling the behaviour of the heal pick up object.
/// </summary>
public class HealPickUpObject : NetworkBehaviour, iPickUpObject
{
    [SerializeField] int healAmount;
    private float speed = 3;
    private float speed2 = 3;
    private float timer = 0.2f;
    Transform targetDestination;
    public void OnPickUp(Character character)
    {
        if (!IsOwner) return;
        character.Heal(healAmount);
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
