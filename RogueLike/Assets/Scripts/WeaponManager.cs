using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] Transform weaponObjectContainer;
    [SerializeField] WeaponData startingWeapon;
    [SerializeField] public EnemiesManager enemiesManager;

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
        AddWeapon(startingWeapon);
    }
    public void AddWeapon(WeaponData weaponData)
    {
        GameObject weaponGameObject = Instantiate(weaponData.weaponBasePrefab, weaponObjectContainer);
        weaponGameObject.GetComponent<WeaponBase>().SetData(weaponData);

        WeaponBase weaponBase = weaponGameObject.GetComponent<WeaponBase>();

        weaponBase.SetData(weaponData);
        weapons.Add(weaponBase);

        if(character != null)
        {
            character.AddUpgradeIntoList(weaponData.firstUpgrade);
        }
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
}
