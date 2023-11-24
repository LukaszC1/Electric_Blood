using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class WeaponStats : INetworkSerializable
{
    public float damage;
    public float timeToAttack;
    public float size;
    public Vector2 vectorSize;
    public int amount;
    public int pierce;

    //weapon attributes

    public WeaponStats(float damage, float timeToAttack, float size, Vector2 vectorSize, int amount, int pierce)
    {
        this.damage = damage;   
        this.timeToAttack = timeToAttack;
        this.size = size;
        this.vectorSize = vectorSize;
        this.amount = amount;
        this.pierce = pierce;
    }
    public WeaponStats()
    {
        this.damage = 0;
        this.timeToAttack = 0;
        this.size = 0;
        this.vectorSize = Vector2.zero;
        this.pierce = 0;
        this.amount = 0;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref damage);
        serializer.SerializeValue(ref timeToAttack);
        serializer.SerializeValue(ref size);
        serializer.SerializeValue(ref amount);
        serializer.SerializeValue(ref pierce);
        serializer.SerializeValue(ref vectorSize);
    }

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
[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public string Name;
    public WeaponStats stats;
    public GameObject weaponBasePrefab;
    public UpgradeData firstUpgrade;
}
