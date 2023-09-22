using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItems : MonoBehaviour
{
    public List<Item> items;
    Character character;


    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
       
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
        character.updateWeapons();
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
        itemToUpgrade.UnEquip(character);
        itemToUpgrade.stats.Sum(upgradeData.itemStats);
        itemToUpgrade.Equip(character);
        if(upgradeData.nextupgrade != null)
            character.AddUpgradeIntoList(upgradeData.nextupgrade);
        character.updateWeapons();

    }
}
