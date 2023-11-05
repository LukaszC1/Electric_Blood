using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] Transform weaponObjectContainer;
    [SerializeField] WeaponData startingWeapon;
    [SerializeField] public EnemiesManager enemiesManager;
    [SerializeField] public List<WeaponData> allWeapons;

    [HideInInspector]public List<WeaponBase> weapons;
    Character character;
    EquipedItemsManager equipedItemsManager;

    private void Awake()
    {  
        weapons = new List<WeaponBase>();
        character = GetComponent<Character>();
        equipedItemsManager = FindObjectOfType<EquipedItemsManager>();
    }
    private void Start()
    {
        equipedItemsManager.ReturnWeaponsIcons()[0].Set(startingWeapon.firstUpgrade);

        if(!IsOwner) return;
        AddWeapon(startingWeapon);
    }
    public void AddWeapon(WeaponData weaponData)
    {
        //if(!IsOwner) return;
        AddWeaponClientRpc(weaponData.Name);      
    }

    internal void UpgradeWeapon(UpgradeData upgradeData)
    {
        WeaponBase weaponToUpgrade = weapons.Find(wd => wd.weaponData == upgradeData.weaponData);
        weaponToUpgrade.Upgrade(upgradeData);
        if (upgradeData.nextupgrade != null)
        {
            character.AddUpgradeIntoList(upgradeData.nextupgrade);
        }
    }

    [ClientRpc]
    private void AddWeaponClientRpc(string weaponName)
    {
        WeaponData weaponData = allWeapons.Find(wd => wd.Name.Equals(weaponName));

        GameObject weaponGameObject = Instantiate(weaponData.weaponBasePrefab, weaponObjectContainer);
        weaponGameObject.GetComponent<WeaponBase>().SetData(weaponData);

        //if (weaponGameObject.GetComponent<NetworkObject>() != null)
        //weaponGameObject.GetComponent<NetworkObject>().Spawn();

        WeaponBase weaponBase = weaponGameObject.GetComponent<WeaponBase>();

        weaponBase.SetData(weaponData);
        weapons.Add(weaponBase);


        if (character != null)
        {
            character.AddUpgradeIntoList(weaponData.firstUpgrade);
        }
    }

}
