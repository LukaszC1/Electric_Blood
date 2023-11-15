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
