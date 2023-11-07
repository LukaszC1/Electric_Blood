using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ThrowingDagger : WeaponBase
{
    [SerializeField] GameObject knifePrefab;


    public override void Attack()
    {
        StartCoroutine(CoroutineAttack());
    }

    private IEnumerator CoroutineAttack()
    {
        for (int i = 0; i < weaponStats.amount; i++)
        {
            attackServerRpc(transform.position);
            yield return new WaitForSeconds(0.1f);
        }
    }

    [ServerRpc]
    private void attackServerRpc(Vector2 startPosition)
    {
        weaponSound.Play();
        GameObject thrownKnife = Instantiate(knifePrefab);
        thrownKnife.transform.position = new Vector2(startPosition.x + UnityEngine.Random.Range(-0.3f, 0.3f), startPosition.y + UnityEngine.Random.Range(-0.3f, 0.3f));
        ThrowingDaggerProjectile projectile = thrownKnife.GetComponent<ThrowingDaggerProjectile>();

        projectile.setDirection(playerMove.lastHorizontalVectorProjectiles.Value, playerMove.lastVerticalVectorProjectiles.Value);
        thrownKnife.GetComponent<NetworkObject>().Spawn();
        projectile.damage = weaponStats.damage;
        projectile.speed = projectile.speed * character.projectileSpeedMultiplier;
        projectile.size = weaponStats.size;
        projectile.transform.localScale = new Vector2(projectile.transform.localScale.x * transform.localScale.x, projectile.transform.localScale.y * transform.localScale.y);
        projectile.pierce = weaponStats.pierce;
    }
}
