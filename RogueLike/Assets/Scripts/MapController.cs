using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Class handling the map generation.
/// </summary>
public class MapController : NetworkBehaviour
{
    //Public fields

    /// <summary>
    /// List of all possible chunks.
    /// </summary>
    public List<GameObject> terrainChunk;

    public GameObject currentChunk;

    /// <summary>
    /// Radius of the collider circle.
    /// </summary>
    public float radius;

    public float cooldownTime;
    public LayerMask terrainMask;
    [Header("Optimization")]
    public List<GameObject> spawnedChunk;
    public GameObject latest;
    public float maxDistance; //> chunk size

    //Private fields
    Vector3 noTerrain;
    Vector3 right;
    Vector3 left;
    Vector3 up;
    Vector3 down;
    Vector3 upright;
    Vector3 upleft;
    Vector3 downrigth;
    Vector3 downleft;
    float dist;
    float cooldown;
  

    private void Start()
    {
        var playerData = ElectricBloodMultiplayer.Instance.GetPlayerDataFromClientId(0);
        var levelData = ElectricBloodMultiplayer.Instance.availableLevels[playerData.selectedLevel] as LevelData;
        terrainChunk = levelData.chunks;
        if (!IsOwner) return;
        ChunkSpawnerServerRpc(new Vector3(0, 0, 0));
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.listOfPlayers.Count == 0) return;
            CheckChunks();
        if (!IsServer) return;
        if(spawnedChunk != null)
            Optimizer();
        spawnedChunk.RemoveAll(x => x == null);
    }

    private void CheckChunks()
    {
        if (!currentChunk)
        {
            return;
        }

        up = currentChunk.transform.position + new Vector3(0, (float)22.4, 0);
        down = currentChunk.transform.position + new Vector3(0, -(float)22.4, 0);

        right = currentChunk.transform.position + new Vector3((float)22.4, 0, 0);
        left = currentChunk.transform.position + new Vector3(-(float)22.4, 0, 0);

        upright = currentChunk.transform.position + new Vector3((float)22.4, (float)22.4, 0);
        upleft = currentChunk.transform.position + new Vector3(-(float)22.4, (float)22.4, 0);

        downrigth = currentChunk.transform.position + new Vector3((float)22.4, -(float)22.4, 0);
        downleft = currentChunk.transform.position + new Vector3(-(float)22.4, -(float)22.4, 0);

        if (!Physics2D.OverlapCircle(up, radius, terrainMask))
        {
            ChunkSpawnerServerRpc(up);
        }
        if (!Physics2D.OverlapCircle(down, radius, terrainMask))
        {
            ChunkSpawnerServerRpc(down);
        }
        if (!Physics2D.OverlapCircle(right, radius, terrainMask))
        {
            ChunkSpawnerServerRpc(right);
        }
        if (!Physics2D.OverlapCircle(left, radius, terrainMask))
        {
            ChunkSpawnerServerRpc(left);
        }
        if (!Physics2D.OverlapCircle(upright, radius, terrainMask))
        {
            ChunkSpawnerServerRpc(upright);
        }
        if (!Physics2D.OverlapCircle(upleft, radius, terrainMask))
        {
            ChunkSpawnerServerRpc(upleft);
        }
        if (!Physics2D.OverlapCircle(downrigth, radius, terrainMask))
        {
            ChunkSpawnerServerRpc(downrigth);
        }
        if (!Physics2D.OverlapCircle(downleft, radius, terrainMask))
        {
            ChunkSpawnerServerRpc(downleft);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChunkSpawnerServerRpc(Vector3 positionToSpawn)
    {
        int rand = UnityEngine.Random.Range(0, terrainChunk.Count);
        latest = Instantiate(terrainChunk[rand], positionToSpawn, Quaternion.identity);
        spawnedChunk.Add(latest);
        latest.GetComponent<NetworkObject>().Spawn();
    }

    private void Optimizer()
    {
        cooldown -= Time.deltaTime;

        if (cooldown < 0f)
        {
            cooldown = cooldownTime;
        }
        else
        {
            return;
        }

        foreach (GameObject chunk in spawnedChunk)
        {
            bool disableChunk = true;
            foreach (var player in GameManager.Instance.listOfPlayers)
            {
                dist = Vector3.Distance(player.Value.position, chunk.transform.position);

                if (dist < maxDistance)
                {
                    disableChunk = false;
                }
            }
            if (disableChunk)
            {
                Destroy(chunk);
            }
        }
    }
}


