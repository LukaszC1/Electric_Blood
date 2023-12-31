using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Script attached to WhipWeapon prefab.
/// </summary>
public class WhipWeapon : WeaponBase
{
  /// <summary>
  /// Whip prefab.
  /// </summary>
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

    /// <summary>
    /// Implemetnation of abstract method from WeaponBase.
    /// </summary>
    public override void Attack()
    {
        StartCoroutine(CoroutineAttack());
    }

    /// <summary>
    /// Method handling periodic attacking of a weapon using coroutine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoroutineAttack()
    {       
            startPosition = transform.position;         

        if (playerMove.lastHorizontalVector.Value > 0)
            {
                for (int i = 0; i < weaponStats.amount; i++)
                {
                     spawnObjectRightServerRpc(i, startPosition);

                    yield return new WaitForSeconds(0.2f);
                }
            }
            else 
            {
                for (int i = 0; i < weaponStats.amount; i++)
                {

                    spawnObjectLeftServerRpc(i, startPosition);

                    yield return new WaitForSeconds(0.2f);
                }
            }     
    }

    [ServerRpc(RequireOwnership = false)]
    private void spawnObjectRightServerRpc(int i, Vector2 startPosition)
        {
            strike = Instantiate(WhipObject);      
            strike.transform.position = new Vector2(startPosition.x + 1.5f + i, startPosition.y);

            Collider2D[] colliders = Physics2D.OverlapBoxAll(strike.transform.position, weaponStats.vectorSize, 0f);
            if (i % 2 == 0)
                strike.transform.localScale = new Vector2(strike.transform.localScale.x * transform.localScale.x, strike.transform.localScale.y * transform.localScale.y);
            else
                strike.transform.localScale = new Vector2(strike.transform.localScale.x * transform.localScale.x, -strike.transform.localScale.y * transform.localScale.y);
            ApplyDamage(colliders);
            PlaySoundClientRpc();

            strike.GetComponent<NetworkObject>().Spawn();      
        }

    [ServerRpc(RequireOwnership = false)]
    private void spawnObjectLeftServerRpc(int i, Vector2 startPosition)
        {
            strike = Instantiate(WhipObject);
            strike.transform.position = new Vector2(startPosition.x - 1.5f - i, startPosition.y);

            Collider2D[] colliders = Physics2D.OverlapBoxAll(strike.transform.position, weaponStats.vectorSize, 0f);
            if (i % 2 == 0)
                strike.transform.localScale = new Vector2(-strike.transform.localScale.x * transform.localScale.x, strike.transform.localScale.y * transform.localScale.y);
            else
                strike.transform.localScale = new Vector2(-strike.transform.localScale.x * transform.localScale.x, -strike.transform.localScale.y * transform.localScale.y);

            ApplyDamage(colliders);
            PlaySoundClientRpc();

            strike.GetComponent<NetworkObject>().Spawn();
    }

    [ClientRpc]
    private void PlaySoundClientRpc()
    {
        weaponSound.Play();
    }
}
