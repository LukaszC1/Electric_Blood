using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PassiveItems : NetworkBehaviour
{
    public List<Item> items;
    Character character;


    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Equip(Item itemToEquip)
    {
        if(items == null)
        {
            items = new List<Item>(); //initialize list if not present
        }
        Item newItemInstance = Item.CreateInstance<Item>();
        newItemInstance.Init(itemToEquip.Name);
        newItemInstance.stats.Sum(itemToEquip.stats);


        items.Add(newItemInstance);
        newItemInstance.Equip(character);
        if (itemToEquip.firstUpgrade != null)
            character.AddUpgradeIntoList(itemToEquip.firstUpgrade);
        character.updateWeaponsServerRpc();
    }

    public void UnEquip(Item itemToEquip)
    {
        if (items != null)
        {
            items = new List<Item>(); //initialize list if not present
        }
        items.Add(itemToEquip);
    }

    internal void UpgradeItem(UpgradeData upgradeData)
    {
        Item itemToUpgrade = items.Find(id => id.Name == upgradeData.item.Name);
        itemToUpgrade.stats.Sum(upgradeData.itemStats);
        character.UpgradeStats(upgradeData.itemStats);
        if(upgradeData.nextupgrade != null)
            character.AddUpgradeIntoList(upgradeData.nextupgrade);
        character.updateWeaponsServerRpc();

    }
}
