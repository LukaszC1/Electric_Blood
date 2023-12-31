using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Script attached to projectiles.
/// </summary>
public class ThrowingDaggerProjectile : NetworkBehaviour
{
    //Public fields
    public float speed;
    public float damage;
    public float size;
    public int pierce = 1;

    //Private fields
    [SerializeField] float decayTime = 100;
    Vector3 direction;
    List<iDamageable> damagedEnemies;

    /// <summary>
    /// Sets the direction of the projectile.
    /// </summary>
    /// <param name="dirx"></param>
    /// <param name="diry"></param>
    public void setDirection(float dirx, float diry)
    {
        direction = new Vector3(dirx, diry);
        transform.right = direction;
    }

    private void Start()
    {
        damagedEnemies = new List<iDamageable>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner) return;

        transform.position += direction.normalized * speed * Time.deltaTime;

        decayTime -= Time.deltaTime;
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, size);

        foreach (Collider2D collision in collisions)
        {
            iDamageable enemy = collision.GetComponent<iDamageable>();

            if (damagedEnemies == null) { damagedEnemies=new List<iDamageable>(); }

            if (enemy != null && !damagedEnemies.Contains(enemy))
            {
                enemy.TakeDamage(damage);
                PostDamage((int)damage, collision.transform.position);
                damagedEnemies.Add(enemy);
                pierce--;
                break;
            }

        }

        if (decayTime <= 0 || pierce <= 0)
        {
            DestroyProjectileServerRpc();
        }
   
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyProjectileServerRpc()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Creates the damage popup object at a given position.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="worldPosition"></param>
    public void PostDamage(int damage, Vector3 worldPosition)
    {
        MessageSystem.instance.PostMessage(damage, worldPosition);
    }
}
