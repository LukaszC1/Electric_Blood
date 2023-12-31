using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Abstract class which represents the weapon base definition.
/// </summary>
public abstract class WeaponBase : NetworkBehaviour
{
    //Public fields
    public WeaponData weaponData;
    public WeaponStats weaponStats;
    public PlayerMove playerMove;
    public Vector2 originalAoE;
    public float originalAoEF;
    public Vector2 originalScale;
    public Character character;
    public float originalDamage;
    public float originalCd;
    public int originalAmount;
    public AudioSource weaponSound;
    public GameObject sprite;
    public List<GameObject> enemies = new();

    float timer;
    [SerializeField] bool updateSprite = false;

    private void Awake()
    {
        playerMove = GetComponentInParent<PlayerMove>();
        weaponSound= GetComponent<AudioSource>();
        character = GetComponentInParent<Character>();
    }
    private void Start()
    {
        if (IsOwner)
        {
            enemies = FindObjectOfType<EnemiesManager>().enemyList;
            originalAoE = weaponStats.vectorSize;
            originalScale = transform.localScale;
            originalDamage = weaponStats.damage;
            originalCd = weaponStats.timeToAttack;
            originalAoEF = weaponStats.size;
            originalAmount = weaponStats.amount;

            if (weaponStats.vectorSize.x != 0 || weaponStats.vectorSize.y != 0)
                weaponStats.vectorSize = new Vector2(weaponStats.vectorSize.x * character.areaMultiplier.Value, weaponStats.vectorSize.y * character.areaMultiplier.Value);
            if (weaponStats.size != 0)
                weaponStats.size = weaponStats.size * character.areaMultiplier.Value;
            transform.localScale = new Vector2(transform.localScale.x * character.areaMultiplier.Value, transform.localScale.y * character.areaMultiplier.Value);
            weaponStats.damage = weaponStats.damage * character.damageMultiplier.Value;
            weaponStats.timeToAttack = weaponStats.timeToAttack * character.cooldownMultiplier.Value;
            weaponStats.amount += character.amountBonus.Value;
        }
    }
    private void Update()
    {
        if (!IsServer) return;
        timer -= Time.deltaTime;

        if (timer < 0f)
        {
            Attack();

           timer = weaponStats.timeToAttack;
        }
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;
        if (character == null) Destroy(gameObject);
    }

    /// <summary>
    /// Sets the weapon data.
    /// </summary>
    /// <param name="wd"></param>
    public virtual void SetData(WeaponData wd)
    {
        weaponData = wd;

        weaponStats = new WeaponStats(wd.stats.damage, wd.stats.timeToAttack, wd.stats.size, wd.stats.vectorSize, wd.stats.amount, wd.stats.pierce);
    }

    public abstract void Attack(); //each weapon has to inherit and implement this method

    /// <summary>
    /// Method which updates the weapon stats when the character levels up.
    /// </summary>
    public virtual void LevelUpUpdate()
    {
        if (character == null) 
            character = GetComponentInParent<Character>(); 
        if (weaponStats.vectorSize.x != 0 || weaponStats.vectorSize.y != 0)
            weaponStats.vectorSize = new Vector2(originalAoE.x * character.areaMultiplier.Value, originalAoE.y * character.areaMultiplier.Value);
        if (weaponStats.size != 0)
            weaponStats.size = originalAoEF * character.areaMultiplier.Value;
        transform.localScale = new Vector2(originalScale.x * character.areaMultiplier.Value, originalScale.y * character.areaMultiplier.Value);
        if(updateSprite)
            UpdateSpriteClientRpc(originalScale, character, character.areaMultiplier.Value);
        weaponStats.damage = originalDamage * character.damageMultiplier.Value;
        weaponStats.timeToAttack = originalCd * character.cooldownMultiplier.Value;
        weaponStats.amount = originalAmount + character.amountBonus.Value;

    }

    /// <summary>
    /// Method which posts a damage popup message.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="targetPos"></param>
    public virtual void PostMessage(int damage, Vector3 targetPos)
    {
        MessageSystem.instance.PostMessage(damage, targetPos);
    }

    /// <summary>
    /// Method which upgrades the weapon stats.
    /// </summary>
    /// <param name="upgradeData"></param>
    public void Upgrade(WeaponStats upgradeData)
    {
        float percentageIncrease = 1;
        if (originalAoEF == 0 && upgradeData.vectorSize.x > 0)
        percentageIncrease = (upgradeData.vectorSize.x + originalAoE.x)/originalAoE.x;
        else if (originalAoE.x == 0 && upgradeData.size > 0)
        percentageIncrease = ((upgradeData.size + originalAoEF) / originalAoEF);
        this.originalDamage += upgradeData.damage;
        this.originalCd -= upgradeData.timeToAttack;
        this.originalAoEF += upgradeData.size;
        this.originalAoE += upgradeData.vectorSize;
        this.originalScale = new Vector2(originalScale.x * percentageIncrease, originalScale.y * percentageIncrease);
        this.originalAmount += upgradeData.amount;
        weaponStats.pierce += upgradeData.pierce;
        LevelUpUpdate();
    }

    [ClientRpc]
    private void UpdateSpriteClientRpc(Vector2 originalScale, NetworkBehaviourReference characterReference, float areaMultiplier)
    {
        characterReference.TryGet(out Character character);
        ForceFieldSprite sprite = character.GetComponentInChildren<ForceFieldSprite>();
        if (sprite != null)
            sprite.UpdateTransform(new Vector2(originalScale.x * areaMultiplier, originalScale.y * areaMultiplier));
    }
}
