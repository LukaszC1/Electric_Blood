using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Class for destructable objects. Implements iDamageable.
/// </summary>
public class DestructableObject : NetworkBehaviour, iDamageable
{
    private bool tookDamage = false;
    public bool TookDamage { get => tookDamage; set => tookDamage = value; }

    private bool isGettingDestroyed = false;
    private float dissolveAmount = 1;

    /// <summary>
    /// List of possible drops on destroy of the object.
    /// </summary>
    [SerializeField] List<DropOnDestroy> dropOnDestroy = new List<DropOnDestroy>();

    public void ApplySlow()
    {
        return;
    }

    private void Update()
    {
        if (isGettingDestroyed)
        {
            dissolveAmount -= Time.deltaTime * 1.5f;
            GetComponent<Renderer>().material.SetFloat("_Dissolve_Amount", dissolveAmount);
            if (dissolveAmount < 0)
            {
                DestroyObjectServerRpc();
            }
        }
    }
    public void TakeDamage(float damage)
    {
        if (!isGettingDestroyed)
        {
            TakeDamageClientRpc();
        }
    }

    [ClientRpc]
    private void TakeDamageClientRpc()
    {
        isGettingDestroyed = true;
        GetComponent<BoxCollider2D>().enabled = false;
        foreach (DropOnDestroy dropOnDestroy in dropOnDestroy)
            dropOnDestroy.CheckDrop();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc()
    {
        Destroy(gameObject);
    }
}
