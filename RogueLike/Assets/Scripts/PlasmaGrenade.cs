using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaGrenade : MonoBehaviour
{
    Vector3 direction;
    public float speed;
    public float damage;
    public float size;
    public int pierce = 1;
    [SerializeField] float decayTime = 100;
    [SerializeField] GameObject plasmaExlposion;
    public Character character;
    public float timeToAttack;

    List<iDamageable> damagedEnemies;

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
        transform.position += direction.normalized * speed * Time.deltaTime;
        transform.Rotate(0.0f, 0.0f, 3, Space.Self);
        //Debug.Log(transform.position);

    
         decayTime -= Time.deltaTime;
         Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, size);

         foreach (Collider2D collision in collisions)
         {
             iDamageable enemy = collision.GetComponent<iDamageable>();

             if (damagedEnemies == null) { damagedEnemies = new List<iDamageable>(); }

             if (enemy != null && !damagedEnemies.Contains(enemy))
             {
                 damagedEnemies.Add(enemy);
                 pierce--;
                 GameObject explosion = Instantiate(plasmaExlposion);
                 Vector3 currentPosition = transform.position;
                 explosion.transform.position = currentPosition;
                 explosion.transform.localScale = new Vector2(explosion.transform.localScale.x * transform.localScale.x, explosion.transform.localScale.y * transform.localScale.y);

                 Collider2D[] colliders = Physics2D.OverlapCircleAll(explosion.transform.position, size*5);
                 for (int i = 0; i < colliders.Length; i++)
                 {
                     iDamageable e = colliders[i].GetComponent<iDamageable>();
                     if (e != null)
                     {
                         PostDamage((int)damage, colliders[i].transform.position);
                         e.TakeDamage(damage);
                     }
                 }

                 break;
             }

         }

         if (decayTime <= 0 || pierce <= 0)
         {
             Destroy(gameObject);
         }
     
    }
    public void PostDamage(int damage, Vector3 worldPosition)
    {
        MessageSystem.instance.PostMessage(damage, worldPosition);
    }
}
