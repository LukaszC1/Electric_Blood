using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Class containing all the stats of a weapon implementing INetworkSerializable interface.
/// </summary>
[Serializable]
public class WeaponStats : INetworkSerializable
{
    //weapon attributes
    public float damage;
    public float timeToAttack;
    public float size;
    public Vector2 vectorSize;
    public int amount;
    public int pierce;

    /// <summary>
    /// WeaponStats constructor.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="timeToAttack"></param>
    /// <param name="size"></param>
    /// <param name="vectorSize"></param>
    /// <param name="amount"></param>
    /// <param name="pierce"></param>
    public WeaponStats(float damage, float timeToAttack, float size, Vector2 vectorSize, int amount, int pierce)
    {
        this.damage = damage;   
        this.timeToAttack = timeToAttack;
        this.size = size;
        this.vectorSize = vectorSize;
        this.amount = amount;
        this.pierce = pierce;
    }

    /// <summary>
    /// WeaponStats default constructor.
    /// </summary>
    public WeaponStats()
    {
        this.damage = 0;
        this.timeToAttack = 0;
        this.size = 0;
        this.vectorSize = Vector2.zero;
        this.pierce = 0;
        this.amount = 0;
    }

    /// <summary>
    /// Implementation of the INetworkSerializable interface method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializer"></param>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref damage);
        serializer.SerializeValue(ref timeToAttack);
        serializer.SerializeValue(ref size);
        serializer.SerializeValue(ref amount);
        serializer.SerializeValue(ref pierce);
        serializer.SerializeValue(ref vectorSize);
    }

    /// <summary>
    /// Method which adds the upgrade stats to the current weapon stats.
    /// </summary>
    /// <param name="weaponUpgradeStats"></param>
    public void Sum(WeaponStats weaponUpgradeStats)
    {
        this.damage += weaponUpgradeStats.damage;
        this.timeToAttack += weaponUpgradeStats.timeToAttack;
        this.size += weaponUpgradeStats.size;
        this.vectorSize += weaponUpgradeStats.vectorSize;
        this.amount += weaponUpgradeStats.amount;
        this.pierce += weaponUpgradeStats.pierce;
    }
}

/// <summary>
/// Scriptable object containing the information about weapons.
/// </summary>
[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public string Name;
    public WeaponStats stats;
    public GameObject weaponBasePrefab;
    public UpgradeData firstUpgrade;
}
