using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Rendering.Universal;
using UnityEngine;

/// <summary>
/// Weapon manager class. This class is responsible for managing the weapons of the player.
/// </summary>
public class WeaponManager : NetworkBehaviour
{
    /// <summary>
    /// List of weapons.
    /// </summary>
    [HideInInspector] public List<WeaponBase> weapons;

    /// <summary>
    /// Enemies manager handling the enemies spawning.
    /// </summary>
    [HideInInspector] public EnemiesManager enemiesManager;

    /// <summary>
    /// List of all possible weapons.
    /// </summary>
    [SerializeField] public List<WeaponData> allWeapons;

    [SerializeField] WeaponData startingWeapon;
    Character character;
    EquipedItemsManager equipedItemsManager;
    [SerializeField] Transform weaponObjectContainer;

    private void Awake()
    {
        weapons = new List<WeaponBase>();
        character = GetComponent<Character>();
        equipedItemsManager = FindObjectOfType<EquipedItemsManager>();
    }
    private void Start()
    {
        enemiesManager = FindObjectOfType<EnemiesManager>();
        equipedItemsManager.ReturnWeaponsIcons()[0].Set(startingWeapon.firstUpgrade);

        if (!IsOwner) return;
        AddWeapon(startingWeapon);
    }

    /// <summary>
    /// Method which adds a weapon.
    /// </summary>
    /// <param name="weaponData"></param>
    public void AddWeapon(WeaponData weaponData)
    {
        AddWeaponServerRpc(weaponData.Name);
    }

    /// <summary>
    /// Method which applies upgrades to weapons.
    /// </summary>
    /// <param name="upgradeData"></param>
    public void UpgradeWeapon(UpgradeData upgradeData)
    {
        WeaponBase weaponToUpgrade = weapons.Find(wd => wd.weaponData == upgradeData.weaponData);
        UpgradeWeaponServerRpc(upgradeData.weaponUpgradeStats, weaponToUpgrade);
        if (upgradeData.nextupgrade != null)
        {
            character.AddUpgradeIntoList(upgradeData.nextupgrade);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddWeaponServerRpc(string weaponName)
    {
        WeaponData weaponData = allWeapons.Find(wd => wd.Name.Equals(weaponName));

        GameObject weaponGameObject = Instantiate(weaponData.weaponBasePrefab, weaponObjectContainer);
        weaponGameObject.GetComponent<WeaponBase>().SetData(weaponData);

        WeaponBase weaponBase = weaponGameObject.GetComponent<WeaponBase>();

        weaponGameObject.GetComponent<NetworkObject>().Spawn();
        weaponGameObject.transform.SetParent(NetworkObject.transform, false);
        weaponGameObject.transform.position = character.transform.position;


        if (character != null)
        {
            AddFirstUpgradeClientRpc(weaponName, weaponBase);
        }
    }
    [ClientRpc]
    private void AddFirstUpgradeClientRpc(string weaponName, NetworkBehaviourReference weaponGameObject)
    {
        WeaponData weaponData = allWeapons.Find(wd => wd.Name.Equals(weaponName));
        character.AddUpgradeIntoList(weaponData.firstUpgrade);
        weaponGameObject.TryGet<WeaponBase>(out WeaponBase weaponBase);
        weaponBase.SetData(weaponData);
        weapons.Add(weaponBase);
    }
    [ServerRpc(RequireOwnership = false)]
    private void UpgradeWeaponServerRpc(WeaponStats weaponStats, NetworkBehaviourReference weaponBaseReference)
    {
        weaponBaseReference.TryGet<WeaponBase>(out WeaponBase weaponToUpgrade);
        weaponToUpgrade.Upgrade(weaponStats);
    }
}

