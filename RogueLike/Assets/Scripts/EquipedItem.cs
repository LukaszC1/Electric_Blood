using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// EqupedItem is a class that represents the equiped item in the UI.
/// </summary>
public class EquipedItem : MonoBehaviour
{
    [SerializeField] public Image icon;
    [SerializeField] TextMeshProUGUI levelText;
    private int level = 0;
    public bool isSet = false;
    Color temp;

    /// <summary>
    /// Setter for the equiped item.
    /// </summary>
    /// <param name="upgradeData"></param>
    public void Set(UpgradeData upgradeData)
    {
        gameObject.SetActive(true);
        isSet = true;
        icon.sprite = upgradeData.icon;
        temp = icon.color;
        temp.a = 1.0f;
        icon.color = temp;
        level++;
        levelText.text = level.ToString();
    }

    /// <summary>
    /// Clean the equiped item sprite.
    /// </summary>
    public void Clean()
    {
        icon.sprite = null;
    }

    /// <summary>
    /// Level up the equiped item on the ui.
    /// </summary>
    public void LevelEquipedItem()
    {
        this.level++;
        levelText.text = level.ToString();
    }
}
