using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Class that handles dropping an item on destroy.
/// </summary>
public class DropOnDestroy : NetworkBehaviour
{
    [SerializeField] GameObject droppedItem;
    [SerializeField] [Range(0,1)] float chanceToDrop=1f;
    public bool quitting = false;

    private void OnApplicationQuit()
    {
        quitting = true;       
    }

    /// <summary>
    /// Checks if the object should drop an item
    /// </summary>
    public void CheckDrop()
    {
        if (!IsOwner) return;
        if (quitting)
        {
            return;
        }

        if (Random.value < chanceToDrop)
        {
            SpawnDropServerRpc();  
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnDropServerRpc()
    {
        Transform t = Instantiate(droppedItem).transform;
        t.position = transform.position;
        t.GetComponent<NetworkObject>().Spawn();
    }


}
