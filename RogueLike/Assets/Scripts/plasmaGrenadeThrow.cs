using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plasmaGrenadeThrow : WeaponBase
{
    [SerializeField] GameObject grenadePrefab;

    public override void Attack()
    {
        StartCoroutine(SpawnMissile());
    }

    private IEnumerator SpawnMissile()
    {
        //get enemy list somehow to fix the problem
        List<GameObject> enemies = GetComponentInParent<WeaponManager>().enemiesManager.enemyList;


        if (enemies.Count == 0) { yield break; }

        for (int i = 0; i < weaponStats.amount; i++)
        {
            weaponSound.Play();

            GameObject grenade = Instantiate(grenadePrefab);
            Vector3 currentPosition = transform.position;
            grenade.transform.position = currentPosition;

            Vector3 randomEnemy = enemies[UnityEngine.Random.Range(0, enemies.Count - 1)].transform.position;
            if (randomEnemy == null) { yield break; }
            
            Vector3 throwDirection = randomEnemy - currentPosition;

            PlasmaGrenade projectile = grenade.GetComponent<PlasmaGrenade>();

            if (randomEnemy != null)
                projectile.setDirection(throwDirection.x, throwDirection.y);
            projectile.damage = weaponStats.damage;
            projectile.speed = projectile.speed * character.projectileSpeedMultiplier;
            projectile.size = weaponStats.size;
            projectile.transform.localScale = new Vector2(projectile.transform.localScale.x * transform.localScale.x, projectile.transform.localScale.y * transform.localScale.y);
            projectile.pierce = weaponStats.pierce;
            projectile.character = character;
            projectile.timeToAttack = weaponStats.timeToAttack;
            yield return new WaitForSeconds(0.35f);
        }
    }


}
