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
    [HideInInspector] public EnemiesManager enemiesManager;
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
        enemiesManager = FindObjectOfType<EnemiesManager>();
        equipedItemsManager.ReturnWeaponsIcons()[0].Set(startingWeapon.firstUpgrade);

        if(!IsOwner) return;
        AddWeapon(startingWeapon);
    }
    public void AddWeapon(WeaponData weaponData)
    {
        Debug.Log("Weapon equiped on: " + NetworkManager.LocalClientId.ToString());

        AddWeaponServerRpc(weaponData.Name);

        /*if(!IsHost && weaponData.Name.Equals("Force Field")) //to do implement it better?
        {
            WeaponData wpnForceField = allWeapons.Find(wd => wd.Name.Equals("Force Field"));
            GameObject weaponGameObject = Instantiate(wpnForceField.weaponBasePrefab, weaponObjectContainer);
            weaponGameObject.GetComponent<WeaponBase>().SetData(weaponData);
            WeaponBase weaponBase = weaponGameObject.GetComponent<WeaponBase>();
            weaponBase.SetData(wpnForceField);
            weapons.Add(weaponBase);
            if (character != null)
            {
                character.AddUpgradeIntoList(weaponData.firstUpgrade);
            }
        }*/
       
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

    [ServerRpc]
    private void AddWeaponServerRpc(string weaponName)
    {
        WeaponData weaponData = allWeapons.Find(wd => wd.Name.Equals(weaponName));

        GameObject weaponGameObject = Instantiate(weaponData.weaponBasePrefab, weaponObjectContainer);
        weaponGameObject.GetComponent<WeaponBase>().SetData(weaponData);

        WeaponBase weaponBase = weaponGameObject.GetComponent<WeaponBase>();

        weaponGameObject.GetComponent<NetworkObject>().Spawn();
        weaponGameObject.transform.SetParent(NetworkObject.transform, false);
        weaponBase.SetData(weaponData);
        weapons.Add(weaponBase);


        if (character != null)
        {
            character.AddUpgradeIntoList(weaponData.firstUpgrade);
        }
    }

}

