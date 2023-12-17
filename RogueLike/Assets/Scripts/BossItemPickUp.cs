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

        if (!IsOwner) return;

        foreach (var player in GameManager.Instance.listOfPlayers)
        {
            Character playerChar = player.Value.GetComponent<Character>();
            if (playerChar.GetUpgrades(1) != null)
            {
                var upgrade = playerChar.GetUpgrades(1)[0];

                while (upgrade.upgradeType.ToString() != UpgradeType.WeaponUpgrade.ToString() || upgrade.upgradeType.ToString() != UpgradeType.WeaponUpgrade.ToString())
                {
                    upgrade = playerChar.GetUpgrades(1)[0];
                }

                playerChar.AcquiredUpgradesAdd(upgrade);
                playerChar.UpgradesRemove(upgrade);
                playerChar.UpgradeWeaponPickUp(upgrade);
            }
        }
        DestroyObjectServerRpc();
    }
    public void SetTargetDestination(Transform destination)
    {
     
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc()
    {
        Destroy(gameObject);
    }
}
