using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class WhipWeapon : WeaponBase
{
  
    [SerializeField] GameObject WhipObject;
    private Vector2 startPosition;
    private GameObject strike;

    private void ApplyDamage(Collider2D[] colliders)
    {
        for(int i=0; i < colliders.Length; i++)
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
        StartCoroutine(CoroutineAttack());
    }

    
    private IEnumerator CoroutineAttack()
    {       
            startPosition = transform.position;         

        if (playerMove.lastHorizontalVector > 0)
            {
                for (int i = 0; i < weaponStats.amount; i++)
                {
                    spawnObjectServerRpc(i);

                    Debug.Log("Attack");

                if (IsClient) yield return null;

                     strike = Instantiate(WhipObject);
                     strike.GetComponent<NetworkObject>().Spawn();
                     strike.transform.position = new Vector2(startPosition.x + 1.5f + i, startPosition.y);
                     Collider2D[] colliders = Physics2D.OverlapBoxAll(strike.transform.position, weaponStats.vectorSize, 0f);
                     if (i % 2 == 0)
                         strike.transform.localScale = new Vector2(strike.transform.localScale.x * transform.localScale.x, strike.transform.localScale.y * transform.localScale.y);
                     else
                         strike.transform.localScale = new Vector2(strike.transform.localScale.x * transform.localScale.x, -strike.transform.localScale.y * transform.localScale.y);
                     ApplyDamage(colliders);
                     weaponSound.Play();



                    yield return new WaitForSeconds(0.2f);
                }
            }
            else 
            {
                for (int i = 0; i < weaponStats.amount; i++)
                {
                    Debug.Log("Attack");

                    spawnObjectServerRpc(i);

                    if (IsClient) yield return null;

                    strike = Instantiate(WhipObject);
                    strike.transform.position = new Vector2(startPosition.x - 1.5f - i, startPosition.y);
                    strike.GetComponent<NetworkObject>().Spawn();
                    Collider2D[] colliders = Physics2D.OverlapBoxAll(strike.transform.position, weaponStats.vectorSize, 0f);
                    if (i % 2 == 0)
                        strike.transform.localScale = new Vector2(-strike.transform.localScale.x * transform.localScale.x, strike.transform.localScale.y * transform.localScale.y);
                    else
                        strike.transform.localScale = new Vector2(-strike.transform.localScale.x * transform.localScale.x, -strike.transform.localScale.y * transform.localScale.y);

                    ApplyDamage(colliders);
                    weaponSound.Play();

                yield return new WaitForSeconds(0.2f);
                }
            }     
        }

        [ServerRpc]
        private void spawnObjectServerRpc(int i)
        {
            startPosition = transform.position;
            strike = Instantiate(WhipObject);
            strike.transform.position = new Vector2(startPosition.x - 1.5f - i, startPosition.y);
            strike.GetComponent<NetworkObject>().Spawn();
            Collider2D[] colliders = Physics2D.OverlapBoxAll(strike.transform.position, weaponStats.vectorSize, 0f);
            if (i % 2 == 0)
                strike.transform.localScale = new Vector2(-strike.transform.localScale.x * transform.localScale.x, strike.transform.localScale.y * transform.localScale.y);
            else
                strike.transform.localScale = new Vector2(-strike.transform.localScale.x * transform.localScale.x, -strike.transform.localScale.y * transform.localScale.y);

            ApplyDamage(colliders);
            weaponSound.Play();
            
            Debug.Log("Spawned");
            NetworkLog.LogInfoServer("Spawned");
    }
}
