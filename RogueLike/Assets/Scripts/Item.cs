using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
   

    internal void Sum(ItemStats stats)
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



[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string Name;
    public ItemStats stats;
    public UpgradeData firstUpgrade;

    public void Init(string Name)
    {
        this.Name = Name;
        stats = new ItemStats();
    }

    public void Equip(Character character)
    {
        character.maxHp += stats.maxHp;
        character.armor += stats.armor;
        character.hpRegen += stats.hpRegen;
        character.damageMultiplier += stats.dmgMultiplier;
        character.areaMultiplier += stats.aoeMultiplier;
        character.projectileSpeedMultiplier += stats.projectileSpeedMultiplier;
        character.magnetSize += stats.magnetSize;
        character.cooldownMultiplier -= stats.cdMultiplier;
        character.amountBonus += stats.amountBonus;
    }

    public void UnEquip(Character character)
    {
        character.maxHp -= stats.maxHp;
        character.armor -= stats.armor;
        character.hpRegen -= stats.hpRegen;
        character.damageMultiplier -= stats.dmgMultiplier;
        character.areaMultiplier -= stats.aoeMultiplier;
        character.projectileSpeedMultiplier -= stats.projectileSpeedMultiplier;
        character.magnetSize -= stats.magnetSize;
        character.cooldownMultiplier += stats.cdMultiplier;
        character.amountBonus -= stats.amountBonus;
    }


}
