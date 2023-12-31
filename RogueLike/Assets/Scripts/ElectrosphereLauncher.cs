using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// ElectrosphereLauncher is a weapon that spawns an ElectrosphereMissile on attack.
/// </summary>
public class ElectrosphereLauncher : WeaponBase
{
    [SerializeField] GameObject missilePrefab;

    public override void Attack()
    {
        StartCoroutine(SpawnMissile());
    }

    private IEnumerator SpawnMissile()
    {

        for (int i = 0; i < weaponStats.amount; i++)
        {
            spawnObjectServerRpc(i);

            yield return new WaitForSeconds(0.35f);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void spawnObjectServerRpc(int i)
    {
        List<GameObject> enemies = GetComponentInParent<WeaponManager>().enemiesManager.enemyList;

       if (enemies.Count != 0)
        {
            PlaySoundClientRpc();
            GameObject missile = Instantiate(missilePrefab);
            Vector3 currentPosition = transform.position;
            missile.transform.position = currentPosition;
            Vector3 randomEnemy = enemies[UnityEngine.Random.Range(0, enemies.Count - 1)].transform.position;

            if (randomEnemy != null)
            {
                Vector3 throwDirection = randomEnemy - currentPosition;
                ElectrosphereMissile projectile = missile.GetComponent<ElectrosphereMissile>();
                projectile.SetDirection(throwDirection.x, throwDirection.y);
                projectile.damage = weaponStats.damage;
                projectile.speed = projectile.speed * character.projectileSpeedMultiplier.Value;
                projectile.size = weaponStats.size;
                projectile.transform.localScale = new Vector2(projectile.transform.localScale.x * transform.localScale.x, projectile.transform.localScale.y * transform.localScale.y);
                projectile.pierce = weaponStats.pierce;
                projectile.character = character;
                projectile.timeToAttack = weaponStats.timeToAttack;

                projectile.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
    [ClientRpc]
    private void PlaySoundClientRpc()
    {
        weaponSound.Play();
    }
}
