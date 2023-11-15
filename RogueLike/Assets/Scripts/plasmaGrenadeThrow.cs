using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class plasmaGrenadeThrow : WeaponBase
{
    [SerializeField] GameObject grenadePrefab;

    public override void Attack()
    {
        StartCoroutine(SpawnMissile());
    }

    private IEnumerator SpawnMissile()
    {       

        for (int i = 0; i < weaponStats.amount; i++)
        {
            spawnObjectServerRpc();
            yield return new WaitForSeconds(0.35f);
        }
    }

    [ServerRpc]
    private void spawnObjectServerRpc()
    {
        if (enemies.Count != 0)
        {

            weaponSound.Play();

            GameObject grenade = Instantiate(grenadePrefab);
            Vector3 currentPosition = transform.position;
            grenade.transform.position = currentPosition;

            Vector3 randomEnemy = enemies[UnityEngine.Random.Range(0, enemies.Count - 1)].transform.position;
            if (randomEnemy != null)
            {
                Vector3 throwDirection = randomEnemy - currentPosition;

                PlasmaGrenade projectile = grenade.GetComponent<PlasmaGrenade>();

                projectile.setDirection(throwDirection.x, throwDirection.y);
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


}
