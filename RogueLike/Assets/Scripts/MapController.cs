using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MapController : NetworkBehaviour
{

    public List<GameObject> terrainChunk;
    public GameObject currentChunk;
    public float radius;
    Vector3 noTerrain;
    Vector3 right;
    Vector3 left;
    Vector3 up;
    Vector3 down;
    Vector3 upright;
    Vector3 upleft;
    Vector3 downrigth;
    Vector3 downleft;
    public LayerMask terrainMask;
    [Header("Optimization")]
    public List<GameObject> spawnedChunk;
    public GameObject latest;
    public float maxDistance; //> chunk size
    float dist;
    float cooldown;
    public float cooldownTime;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.listOfPlayers.Count == 0) return; //this is mainly here for it to not mess up before lobby is implemented
        CheckChunks();
        if (!IsServer) return;
        optimizer();
        spawnedChunk.RemoveAll(x => x == null);
    }


    void CheckChunks()
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
    void ChunkSpawnerServerRpc(Vector3 positionToSpawn)
    {
        int rand = UnityEngine.Random.Range(0, terrainChunk.Count);
        latest = Instantiate(terrainChunk[rand], positionToSpawn, Quaternion.identity);
        spawnedChunk.Add(latest);
        latest.GetComponent<NetworkObject>().Spawn();
    }

    void optimizer()
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


