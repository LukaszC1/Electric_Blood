using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossItemPickUp : MonoBehaviour, iPickUpObject
{

    public void OnPickUp(Character character)
    {
        //add the random upgrade to the character

        var upgrade = character.GetUpgrades(1)[0];

        while(upgrade.upgradeType.ToString() != UpgradeType.WeaponUpgrade.ToString() || upgrade.upgradeType.ToString() != UpgradeType.WeaponUpgrade.ToString())
        {
            upgrade = character.GetUpgrades(1)[0];
        }

        character.AcquiredUpgradesAdd(upgrade);
        character.UpgradesRemove(upgrade);
        character.UpgradeWeaponPickUp(upgrade);

        Destroy(gameObject);
    }
    public void setTargetDestination(Transform destination)
    {
     
    }
}
