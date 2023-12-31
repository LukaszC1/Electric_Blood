using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Script attached to the player prefab that handles the passive items.
/// </summary>
public class PassiveItems : NetworkBehaviour
{
    /// <summary>
    /// List of items that the player has.
    /// </summary>
    public List<Item> items;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    /// <summary>
    /// Method which adds a new item.
    /// </summary>
    /// <param name="itemToEquip"></param>
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
    }

    /// <summary>
    /// Method which removes an item.
    /// </summary>
    /// <param name="itemToEquip"></param>
    public void UnEquip(Item itemToEquip)
    {
        if (items != null)
        {
            items = new List<Item>(); //initialize list if not present
        }
        items.Add(itemToEquip);
    }

    /// <summary>
    /// Method which upgrades an item.
    /// </summary>
    /// <param name="upgradeData"></param>
    public void UpgradeItem(UpgradeData upgradeData)
    {
        Item itemToUpgrade = items.Find(id => id.Name == upgradeData.item.Name);
        itemToUpgrade.stats.Sum(upgradeData.itemStats);
        character.UpgradeStats(upgradeData.itemStats);
        if(upgradeData.nextupgrade != null)
            character.AddUpgradeIntoList(upgradeData.nextupgrade);
    }
}
