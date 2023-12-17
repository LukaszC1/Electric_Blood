using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Unity.Netcode;

public class Enemy : NetworkBehaviour, iDamageable
{
    Transform targetDestination;
    GameObject targetGameObject;
    Character targetCharacter;
    [SerializeField] public float speed;
    public bool isBoss;
    private float reverseSpeed;
    private float originalSpeed;
    private float previousSpeed;

    private float timer;
    private float timer2 = 0.15f;
    Rigidbody2D rgbd2d;

    [SerializeField] float hp = 4;
    [SerializeField] int damage = 1;
    [SerializeField] Material whiteMat;
    Material originalMat;
    private SpriteRenderer sprite;
    private bool isSlowed = false;
    [HideInInspector]public bool isStunned = false;
    private bool tookDamage= false;
    private bool takingDamage= false;
    float maxDistance = 30f;
    float distance;
    DropOnDestroy dropOnDestroy;
    private float dissolveAmount = 1;
    private bool isDying=false;

    public bool TookDamage { get => tookDamage; set => tookDamage = value; }

    private void Awake()
    {
        rgbd2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        dropOnDestroy = GetComponent<DropOnDestroy>();
        originalMat = GetComponent<Renderer>().material;
        reverseSpeed = -1 * speed;
        originalSpeed = speed;
    }

    private void Start()
    {
        if (!IsOwner) return;
        float playerScaling = (GameManager.Instance.listOfPlayers.Count + 1f) / 2f;
        if (isBoss)
        {
            hp *= GameManager.Instance.level.Value * playerScaling;
        }
        else
        {
            hp *= playerScaling;
        }
    }

    public void SetTarget(GameObject target)
    {
        targetGameObject = target;
        targetDestination = target.transform; 
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (targetDestination == null)
            SetTarget(FindObjectOfType<Character>().gameObject);
        Vector3 direction = (targetDestination.position - transform.position).normalized; 
        rgbd2d.velocity = direction * speed;
        distance = Vector3.Distance(transform.position, targetDestination.position);

        if (distance > maxDistance)
        {
            if (isBoss)
            {
                transform.position = GameManager.Instance.GenerateRandomPosition(targetGameObject.transform.position);
            }
            else
            {
                dropOnDestroy.quitting = true;
                Destroy(gameObject);
                GetComponent<DropOnDestroy>().CheckDrop();
            }
        }

        if (direction.x > 0)
        {
            sprite.flipX = false;
        }
        else
        {
            sprite.flipX = true;
        }


        if (isDying)
        {
            isDyingUpdateClientRpc();
        }
        else if (takingDamage)
        {
            takingDamageClientRpc();
        }
        else if (isStunned)
            speed = 0;
        else if (isSlowed)
            speed = originalSpeed / 2;
        else
            speed = originalSpeed;

    }

    private void Update()
    {
        if (!IsOwner) return;
        timer -= Time.deltaTime;
        if (timer < 0 && isSlowed)
        {
            speed = previousSpeed;
            isSlowed = false;
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Attack(collision.gameObject);
        }
    }

    private void Attack(GameObject target)
    {
        if (!isDying)
        {
           target.GetComponent<Character>().TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (!takingDamage)
            takingDamage = true;
        else
            timer2 += 0.1f;

        if (hp<= 0)
        {
            isDyingClientRpc();
            isDying = true;;
            GameManager.Instance.IncrementKillCount();
        }
    }

    public void ApplySlow()
    {
        if (!isSlowed)
        {
            timer = 0.1f;
            isSlowed = true;
            previousSpeed = speed;
            speed = speed * 0.5f;
            
        }
        else
            timer += 0.1f;
    }
    [ClientRpc]
    private void isDyingUpdateClientRpc()
    {
        dissolveAmount -= 0.05f;
        GetComponent<Renderer>().material.SetFloat("_Dissolve_Amount", dissolveAmount);
        if (dissolveAmount < 0)
        {
            Destroy(gameObject);
        }
    }
    [ClientRpc]
    private void isDyingClientRpc()
    {
        GetComponent<DropOnDestroy>().CheckDrop();
        GetComponent<Renderer>().material = originalMat;
        rgbd2d.simulated = false;
        speed = 0;
    }
    [ClientRpc]
    private void takingDamageClientRpc()
    {
        timer2 -= Time.deltaTime;
        GetComponent<Renderer>().material = whiteMat;
        speed = reverseSpeed;
        if (timer2 <= 0)
        {
            GetComponent<Renderer>().material = originalMat;
            timer2 = 0.15f;
            takingDamage = false;
        }
    }
}