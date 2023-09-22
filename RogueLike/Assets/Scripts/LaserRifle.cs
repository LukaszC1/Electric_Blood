using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserRifle : WeaponBase
{
    [SerializeField] GameObject laserPrefab;


    private IEnumerator SpawnKnife()
    {
            //get enemy list somehow to fix the problem
            List<GameObject>  enemies = GetComponentInParent<WeaponManager>().enemiesManager.enemyList;
            
        
        if (enemies.Count == 0) { yield break; }

        for (int i = 0; i < weaponStats.amount; i++)
        {
            weaponSound.Play();

            GameObject thrownKnife = Instantiate(laserPrefab);
            Vector3 currentPosition = transform.position;
            thrownKnife.transform.position = currentPosition;

            Vector3 closestEnemy = GetClosestEnemy(enemies, currentPosition);
            Vector3 throwDirection = closestEnemy - currentPosition;

            ThrowingDaggerProjectile projectile = thrownKnife.GetComponent<ThrowingDaggerProjectile>();

            projectile.setDirection(throwDirection.x, throwDirection.y);
            projectile.damage = weaponStats.damage;
            projectile.speed = projectile.speed * character.projectileSpeedMultiplier;
            projectile.size = weaponStats.size;
            projectile.transform.localScale = new Vector2(projectile.transform.localScale.x * transform.localScale.x, projectile.transform.localScale.y * transform.localScale.y);
            projectile.pierce = weaponStats.pierce;
            yield return new WaitForSeconds(0.1f);
        }
    }

    Vector3 GetClosestEnemy(List<GameObject> enemies, Vector3 currentPosition)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (GameObject potentialTarget in enemies)
        {
            if (potentialTarget != null)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.transform;
                }
            }
        }
        return bestTarget.position;
    }

    public override void Attack()
    {
        StartCoroutine(SpawnKnife());
    }
}
