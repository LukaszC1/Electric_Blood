using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossItemPickUp : NetworkBehaviour, iPickUpObject
{

    public void OnPickUp(Character character)
    {
        //add the random upgrade to the character

        if (character.GetUpgrades(1) != null)
        {
            var upgrade = character.GetUpgrades(1)[0];

            while (upgrade.upgradeType.ToString() != UpgradeType.WeaponUpgrade.ToString() || upgrade.upgradeType.ToString() != UpgradeType.WeaponUpgrade.ToString())
            {
                upgrade = character.GetUpgrades(1)[0];
            }

            character.AcquiredUpgradesAdd(upgrade);
            character.UpgradesRemove(upgrade);
            character.UpgradeWeaponPickUp(upgrade);
        }
        Destroy(gameObject);
    }
    public void SetTargetDestination(Transform destination)
    {
     
    }
}
