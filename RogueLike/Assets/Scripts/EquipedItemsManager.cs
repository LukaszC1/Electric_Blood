using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EquipedItemsManager is a class that manages the equiped items and weapons of the player.
/// </summary>
public class EquipedItemsManager : MonoBehaviour
{
    [SerializeField] List<EquipedItem> equipedItems;
    [SerializeField] List<EquipedItem> equipedWeapons;

    public List<EquipedItem> ReturnItemsIcons()
    {
        return equipedItems;
    }

    public List<EquipedItem> ReturnWeaponsIcons()
    {
        return equipedWeapons;
    }
}
