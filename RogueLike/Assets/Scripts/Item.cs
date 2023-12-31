using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing all the stats of an item.
/// </summary>
[Serializable]
public class ItemStats
{
    public int maxHp;
    public int armor;
    public float hpRegen;
    public float dmgMultiplier;
    public float aoeMultiplier;
    public float projectileSpeedMultiplier;
    public int magnetSize;
    public float cdMultiplier;
    public int amountBonus;
   
    /// <summary>
    /// Method which upgrades the stats of an item.
    /// </summary>
    /// <param name="stats"></param>
    public void Sum(ItemStats stats)
    {
        maxHp += stats.maxHp;
        armor += stats.armor;
        hpRegen += stats.hpRegen;
        dmgMultiplier += stats.dmgMultiplier;
        aoeMultiplier += stats.aoeMultiplier;
        projectileSpeedMultiplier += stats.projectileSpeedMultiplier;
        magnetSize += stats.magnetSize;
        cdMultiplier -= stats.cdMultiplier;
        amountBonus += stats.amountBonus;
    }
}

/// <summary>
/// Item ScriptableObject which contains all the information of an item.
/// </summary>
[CreateAssetMenu]
public class Item : ScriptableObject
{
    /// <summary>
    /// The name of the item.
    /// </summary>
    public string Name;

    /// <summary>
    /// The stats of the item.
    /// </summary>
    public ItemStats stats;

    /// <summary>
    /// The first upgrade.
    /// </summary>
    public UpgradeData firstUpgrade;

    /// <summary>
    /// Initializes the item.
    /// </summary>
    /// <param name="Name"></param>
    public void Init(string Name)
    {
        this.Name = Name;
        stats = new ItemStats();
    }

    /// <summary>
    /// Method which equips an item to a character.
    /// </summary>
    /// <param name="character"></param>
    public void Equip(Character character)
    {
        character.maxHp.Value += stats.maxHp;
        character.armor.Value += stats.armor;
        character.hpRegen.Value += stats.hpRegen;
        character.damageMultiplier.Value += stats.dmgMultiplier;
        character.areaMultiplier.Value += stats.aoeMultiplier;
        character.projectileSpeedMultiplier.Value += stats.projectileSpeedMultiplier;
        character.magnetSize.Value += stats.magnetSize;
        character.cooldownMultiplier.Value -= stats.cdMultiplier;
        character.amountBonus.Value += stats.amountBonus;
    }

    /// <summary>
    /// Method which unequips an item from a character.
    /// </summary>
    /// <param name="character"></param>
    public void UnEquip(Character character)
    {
        character.maxHp.Value -= stats.maxHp;
        character.armor.Value -= stats.armor;
        character.hpRegen.Value -= stats.hpRegen;
        character.damageMultiplier.Value -= stats.dmgMultiplier;
        character.areaMultiplier.Value -= stats.aoeMultiplier;
        character.projectileSpeedMultiplier.Value -= stats.projectileSpeedMultiplier;
        character.magnetSize.Value -= stats.magnetSize;
        character.cooldownMultiplier.Value += stats.cdMultiplier;
        character.amountBonus.Value -= stats.amountBonus;
    }


}
