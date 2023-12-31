using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UpgradeData is a ScriptableObject that contains data for upgrades.
/// </summary>
[CreateAssetMenu]
public class UpgradeData : ScriptableObject
{
    public UpgradeType upgradeType;
    public string upgradeName;
    public string upgradeDescription;
    public Sprite icon;

    public WeaponData weaponData;
    public WeaponStats weaponUpgradeStats;
    public UpgradeData nextupgrade;

    public Item item;
    public ItemStats itemStats;
}

/// <summary>
/// Enum for upgrade types.
/// </summary>
public enum UpgradeType
{
    WeaponUpgrade,
    ItemUpgrade,
    WeaponUnlock,
    ItemUnlock
}
