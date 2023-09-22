using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : WeaponBase
{
   
    private float timerForce, slowTimer = 0.1f;


    new private void Update()
    {
        timerForce -= Time.deltaTime;
        slowTimer -= Time.deltaTime;
        if (timerForce < 0f)
        {
            timerForce = weaponStats.timeToAttack;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, weaponStats.size);
            ApplyDamage(colliders);
        }
        if (slowTimer < 0f)
        {
            Slow();
        }
        firstAttack();
    }

    
    private void Slow()
    {
        slowTimer = 0.1f;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, weaponStats.size);
        for (int i = 0; i < colliders.Length; i++)
        {
            iDamageable e = colliders[i].GetComponent<iDamageable>();
            if (e != null)
            {
                e.ApplySlow(); 
            }
        }
    }

    private void firstAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, weaponStats.size);
        for (int i = 0; i < colliders.Length; i++)
        {
            iDamageable e = colliders[i].GetComponent<iDamageable>();
            if (e != null && e.TookDamage==false)
            {
                PostMessage((int)weaponStats.damage, colliders[i].transform.position);
                e.TakeDamage(weaponStats.damage);
                e.ApplySlow();
                e.TookDamage = true;
            }
        }
    }

    private void ApplyDamage(Collider2D[] colliders)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            iDamageable e = colliders[i].GetComponent<iDamageable>();
            if (e != null)
            {
                PostMessage((int)weaponStats.damage, colliders[i].transform.position);
                e.TakeDamage(weaponStats.damage);
            }  
        }
    }

    public override void Attack()
    {
        //empty for this weapon since it uses slow mechanic in update
    }

}
