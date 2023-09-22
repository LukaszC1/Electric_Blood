using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{

    public List<GameObject> terrainChunk;
    public GameObject currentChunk;
    public GameObject player;
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
    PlayerMove movement;

    [Header("Optimization")]
    public List<GameObject> spawnedChunk;
    public GameObject latest;
    public float maxDistance; //> chunk size
    float dist;
    float cooldown;
    public float cooldownTime;



    // Start is called before the first frame update
    void Start()
    {
        movement = FindObjectOfType<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckChunks();
        optimizer();
    }


    void CheckChunks()
    {
        if (!currentChunk)
        {
            return;
        }

        if (movement.movementVector.x != 0 || movement.movementVector.y != 0)
        {


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
                ChunkSpawner(up);
            }
            if (!Physics2D.OverlapCircle(down, radius, terrainMask))
            {
                ChunkSpawner(down);
            }
            if (!Physics2D.OverlapCircle(right, radius, terrainMask))
            {
                ChunkSpawner(right);
            }
            if (!Physics2D.OverlapCircle(left, radius, terrainMask))
            {
                ChunkSpawner(left);
            }
            if (!Physics2D.OverlapCircle(upright, radius, terrainMask))
            {
                ChunkSpawner(upright);
            }
            if (!Physics2D.OverlapCircle(upleft, radius, terrainMask))
            {
                ChunkSpawner(upleft);
            }
            if (!Physics2D.OverlapCircle(downrigth, radius, terrainMask))
            {
                ChunkSpawner(downrigth);
            }
            if (!Physics2D.OverlapCircle(downleft, radius, terrainMask))
            {
                ChunkSpawner(downleft);
            }



        }


    }

    void ChunkSpawner(Vector3 positionToSpawn)
    {
        int rand = UnityEngine.Random.Range(0, terrainChunk.Count);
        latest = Instantiate(terrainChunk[rand], positionToSpawn, Quaternion.identity);
        spawnedChunk.Add(latest);
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

            dist = Vector3.Distance(player.transform.position, chunk.transform.position);  
            if (dist > maxDistance) 
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}


