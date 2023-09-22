using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            weaponSound.Play();
            GameObject thrownKnife = Instantiate(knifePrefab);
            thrownKnife.transform.position = new Vector2(transform.position.x, transform.position.y + UnityEngine.Random.Range(-0.3f, 0.3f));
            ThrowingDaggerProjectile projectile = thrownKnife.GetComponent<ThrowingDaggerProjectile>();

            projectile.setDirection(playerMove.lastHorizontalVectorProjectiles, playerMove.lastVerticalVectorProjectiles);
            projectile.damage = weaponStats.damage;
            projectile.speed = projectile.speed * character.projectileSpeedMultiplier;
            projectile.size = weaponStats.size;
            projectile.transform.localScale = new Vector2(projectile.transform.localScale.x * transform.localScale.x, projectile.transform.localScale.y * transform.localScale.y);
            projectile.pierce = weaponStats.pierce;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
