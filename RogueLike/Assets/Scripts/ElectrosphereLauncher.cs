using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrosphereLauncher : WeaponBase
{
    [SerializeField] GameObject missilePrefab;

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

            GameObject missile = Instantiate(missilePrefab);
            Vector3 currentPosition = transform.position;
            missile.transform.position = currentPosition;

            Vector3 randomEnemy = enemies[UnityEngine.Random.Range(0, enemies.Count - 1)].transform.position;
            if (randomEnemy == null) { yield break; }
            Vector3 throwDirection = randomEnemy - currentPosition;

            ElectrosphereMissile projectile = missile.GetComponent<ElectrosphereMissile>();

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
