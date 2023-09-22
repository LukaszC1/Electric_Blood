using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectroSphere : MonoBehaviour
{
    private float timerForce;
    public float timeToAttack;
    public float damage;
    public float size;



    private void Update()
    {
        timerForce -= Time.deltaTime;
        if (timerForce < 0f)
        {
            timerForce = timeToAttack;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, size);
            ApplyDamage(colliders);
        }
    }

    private void ApplyDamage(Collider2D[] colliders)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            iDamageable e = colliders[i].GetComponent<iDamageable>();
            if (e != null)
            {
                PostDamage((int)damage, colliders[i].transform.position);
                e.TakeDamage(damage);
            }
        }
    }
    public void PostDamage(int damage, Vector3 worldPosition)
    {
        MessageSystem.instance.PostMessage(damage, worldPosition);
    }
}
