using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour //weapons base class
{
    public WeaponData weaponData;
    public WeaponStats weaponStats;
    float timer;
    public PlayerMove playerMove;
    public Vector2 originalAoE;
    public float originalAoEF;
    public Vector2 originalScale;
    public Character character;
    public float originalDamage;
    public float originalCd;
    public int originalAmount;
    public AudioSource weaponSound;

    public void Awake()
    {
        playerMove = GetComponentInParent<PlayerMove>();
        weaponSound= GetComponent<AudioSource>();
        character = GetComponentInParent<Character>();

    }
    public void Start()
    {
        originalAoE = weaponStats.vectorSize;
        originalScale = transform.localScale;
        originalDamage = weaponStats.damage;
        originalCd = weaponStats.timeToAttack;
        originalAoEF = weaponStats.size;
        originalAmount = weaponStats.amount;

        if (weaponStats.vectorSize.x != 0 || weaponStats.vectorSize.y != 0)
            weaponStats.vectorSize = new Vector2(weaponStats.vectorSize.x * character.areaMultiplier, weaponStats.vectorSize.y * character.areaMultiplier);
        if (weaponStats.size != 0)
            weaponStats.size = weaponStats.size * character.areaMultiplier;
        transform.localScale = new Vector2(transform.localScale.x * character.areaMultiplier, transform.localScale.y * character.areaMultiplier);
        weaponStats.damage = weaponStats.damage * character.damageMultiplier;
        weaponStats.timeToAttack = weaponStats.timeToAttack * character.cooldownMultiplier;
        weaponStats.amount += character.amountBonus;
    }
    public void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0f)
        {
           Attack();
            timer = weaponStats.timeToAttack;
        }
    }

    public virtual void SetData(WeaponData wd)
    {
        weaponData = wd;

        weaponStats = new WeaponStats(wd.stats.damage, wd.stats.timeToAttack, wd.stats.size, wd.stats.vectorSize, wd.stats.amount, wd.stats.pierce);
    }

    public abstract void Attack(); //each weapon has to inherit and implement this method

    public virtual void LevelUpUpdate()
    {
        if (weaponStats.vectorSize.x != 0 || weaponStats.vectorSize.y != 0)
            weaponStats.vectorSize = new Vector2(originalAoE.x * character.areaMultiplier, originalAoE.y * character.areaMultiplier);
        if (weaponStats.size != 0)
            weaponStats.size = originalAoEF * character.areaMultiplier;
        transform.localScale = new Vector2(originalScale.x * character.areaMultiplier, originalScale.y * character.areaMultiplier);
        weaponStats.damage = originalDamage * character.damageMultiplier;
        weaponStats.timeToAttack = originalCd * character.cooldownMultiplier;
        weaponStats.amount = originalAmount + character.amountBonus;

    }


    public virtual void PostMessage(int damage, Vector3 targetPos)
    {
        MessageSystem.instance.PostMessage(damage, targetPos);
    }


    internal void Upgrade(UpgradeData upgradeData)
    {
        float percentageIncrease = 1;
        if (originalAoEF == 0 && upgradeData.weaponUpgradeStats.vectorSize.x > 0)
        percentageIncrease = (upgradeData.weaponUpgradeStats.vectorSize.x + originalAoE.x)/originalAoE.x;
        else if (originalAoE.x == 0 && upgradeData.weaponUpgradeStats.size > 0)
        percentageIncrease = ((upgradeData.weaponUpgradeStats.size + originalAoEF) / originalAoEF);
        this.originalDamage += upgradeData.weaponUpgradeStats.damage;
        this.originalCd -= upgradeData.weaponUpgradeStats.timeToAttack;
        this.originalAoEF += upgradeData.weaponUpgradeStats.size;
        this.originalAoE += upgradeData.weaponUpgradeStats.vectorSize;
        this.originalScale = new Vector2(originalScale.x * percentageIncrease, originalScale.y * percentageIncrease);
        this.originalAmount += upgradeData.weaponUpgradeStats.amount;
        weaponStats.pierce += upgradeData.weaponUpgradeStats.pierce;
        LevelUpUpdate();
    }
}
