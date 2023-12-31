using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Script to the Throwing Dagger weapon.
/// </summary>
public class ThrowingDagger : WeaponBase
{
    /// <summary>
    /// The prefab of the Throwing Dagger.
    /// </summary>
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

    [ServerRpc(RequireOwnership = false)]
    private void attackServerRpc(Vector2 startPosition)
    {
        PlaySoundClientRpc();
        GameObject thrownKnife = Instantiate(knifePrefab);
        thrownKnife.transform.position = new Vector2(startPosition.x + UnityEngine.Random.Range(-0.3f, 0.3f), startPosition.y + UnityEngine.Random.Range(-0.3f, 0.3f));
        ThrowingDaggerProjectile projectile = thrownKnife.GetComponent<ThrowingDaggerProjectile>();

        projectile.setDirection(playerMove.lastHorizontalVectorProjectiles.Value, playerMove.lastVerticalVectorProjectiles.Value);
        thrownKnife.GetComponent<NetworkObject>().Spawn();
        projectile.damage = weaponStats.damage;
        projectile.speed = projectile.speed * character.projectileSpeedMultiplier.Value;
        projectile.size = weaponStats.size;
        projectile.transform.localScale = new Vector2(projectile.transform.localScale.x * transform.localScale.x, projectile.transform.localScale.y * transform.localScale.y);
        projectile.pierce = weaponStats.pierce;
    }

    [ClientRpc]
    private void PlaySoundClientRpc()
    {
        weaponSound.Play();
    }
}
