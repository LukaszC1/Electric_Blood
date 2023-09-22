using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrosphereMissile : MonoBehaviour
{
    Vector3 direction;
    public float speed;
    public float damage;
    public float size;
    public int pierce = 1;
    [SerializeField] float decayTime = 100;
    [SerializeField] GameObject electroSpherePrefab;
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
                GameObject electroSphere = Instantiate(electroSpherePrefab);
                Vector3 currentPosition = transform.position;
                electroSphere.transform.position = currentPosition;

                ElectroSphere field = electroSphere.GetComponent<ElectroSphere>();

                field.damage = damage;
                field.size = size * 5;
                field.transform.localScale = new Vector2(field.transform.localScale.x * transform.localScale.x, field.transform.localScale.y * transform.localScale.y);
                field.timeToAttack = timeToAttack * 0.25f;
                break;
            }

        }

        if (decayTime <= 0 || pierce <= 0)
        {
            Destroy(gameObject);
        }
    
    }
}
