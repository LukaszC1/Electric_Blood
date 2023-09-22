using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quickhack : WeaponBase
{
    [SerializeField] GameObject hackAnim;

    public override void Attack()
    {
        StartCoroutine(SpawnHack());
    }

    private IEnumerator SpawnHack()
    {
        //get enemy list somehow to fix the problem
        List<GameObject> enemies = GetComponentInParent<WeaponManager>().enemiesManager.enemyList;


        if (enemies.Count == 0) { yield break; }

        for (int i = 0; i < weaponStats.amount; i++)
        {
            GameObject hack = Instantiate(hackAnim);

            Vector3 randomEnemy = enemies[UnityEngine.Random.Range(0, enemies.Count - 1)].transform.position;
            if (randomEnemy == null) { yield break; }
            hack.transform.position = randomEnemy;
            hack.transform.localScale = new Vector2(hack.transform.localScale.x * transform.localScale.x, hack.transform.localScale.y * transform.localScale.y);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(hack.transform.position, weaponStats.size);
            for (int j = 0; j < colliders.Length; j++)
            {
                iDamageable e = colliders[j].GetComponent<iDamageable>();
                if (e != null)
                {
                    PostMessage((int)weaponStats.damage, colliders[j].transform.position);
                    e.TakeDamage(weaponStats.damage);
                }
                Enemy enemy = colliders[j].GetComponent<Enemy>();
                if (enemy != null)
                {
                    StartCoroutine(StunEnemy(enemy));
                }
            }


            yield return new WaitForSeconds(0.05f);
        }
    }
    private IEnumerator StunEnemy(Enemy enemy)
    {
        enemy.isStunned = true;
        yield return new WaitForSeconds(0.8f);
        enemy.isStunned = false;
    }    

}
