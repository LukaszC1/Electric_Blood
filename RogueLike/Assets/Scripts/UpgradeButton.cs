using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script attached to each UpgradeButton in the UpgradeMenu.
/// </summary>
public class UpgradeButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI upgradeName;
    [SerializeField] TextMeshProUGUI upgradeDescription;

    /// <summary>
    /// Setter for the upgrade button text and icon.
    /// </summary>
    /// <param name="upgradeData"></param>
    public void Set(UpgradeData upgradeData)
    {
        icon.sprite = upgradeData.icon;
        upgradeName.text = upgradeData.upgradeName;
        upgradeDescription.text = upgradeData.upgradeDescription;
    }

    /// <summary>
    /// Sprite cleanup method.
    /// </summary>
    public void Clean()
    {
        icon.sprite = null;
    }
}
