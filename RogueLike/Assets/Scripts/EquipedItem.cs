using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipedItem : MonoBehaviour
{
    [SerializeField] public  Image icon;
    [SerializeField] TextMeshProUGUI levelText;
    private int level = 0;

    public bool isSet = false;
    Color temp;


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

    public void Clean()
    {
        icon.sprite = null;
    }

    public void LevelEquipedItem()
    {
        this.level++;
        levelText.text = level.ToString();
    }
}
