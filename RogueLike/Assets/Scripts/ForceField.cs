using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// ForceField weapon class. This weapon is a passive weapon that slows enemies and deals damage to them.
/// </summary>
public class ForceField : WeaponBase
{
    private float timerForce, slowTimer = 0.1f;

    new private void Update()
    {
        if (!IsOwner) return;

        timerForce -= Time.deltaTime;
        slowTimer -= Time.deltaTime;

        forceFieldAttackServerRpc();
    }

    new private void Start()
    {
        sprite.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
        if (!IsHost)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            if (character == null)
                character = GetComponentInParent<Character>();
            Instantiate(sprite, character.transform);
        }
        if (IsOwner)
        {
            originalAoE = weaponStats.vectorSize;
            originalScale = transform.localScale;
            originalDamage = weaponStats.damage;
            originalCd = weaponStats.timeToAttack;
            originalAoEF = weaponStats.size;
            originalAmount = weaponStats.amount;

            if (weaponStats.vectorSize.x != 0 || weaponStats.vectorSize.y != 0)
                weaponStats.vectorSize = new Vector2(weaponStats.vectorSize.x * character.areaMultiplier.Value, weaponStats.vectorSize.y * character.areaMultiplier.Value);
            if (weaponStats.size != 0)
                weaponStats.size = weaponStats.size * character.areaMultiplier.Value;
            transform.localScale = new Vector2(transform.localScale.x * character.areaMultiplier.Value, transform.localScale.y * character.areaMultiplier.Value);
            weaponStats.damage = weaponStats.damage * character.damageMultiplier.Value;
            weaponStats.timeToAttack = weaponStats.timeToAttack * character.cooldownMultiplier.Value;
            weaponStats.amount += character.amountBonus.Value;
        }
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

    [ServerRpc(RequireOwnership = false)]
    private void forceFieldAttackServerRpc()
    {
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
}
