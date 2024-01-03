using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script that handles the shop panel UI for persistent upgrades.
/// </summary>
public class ShopPanelUI : MonoBehaviour
{
    //Private fields
    [SerializeField] private Button closeButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private TextMeshProUGUI availableCoins;
    [SerializeField] private List<TextMeshProUGUI> characterStats;
    [SerializeField] private List<TextMeshProUGUI> costTexts;

    private List<LoadedStats> newCharacterStats = new();
    private int sumOfCoinsSpent = 0;
    private int totalCoinsSpent = 0;
    private int coins;

    /// <summary>
    /// Enum that represents the stats that can be upgraded.
    /// </summary>
    public enum CharacterStats : int
    {
        MaxHp,
        Armor, 
        HpRegen,
        DamageMultiplier,
        AreaMultiplier,
        ProjectileSpeed,
        MagnetSize,
        CooldownMultiplier,
        AmountBonus
    }

    /// <summary>
    /// The struct that holds the data for each stat for serialization.
    /// </summary>
    [Serializable]
    public struct LoadedStats
    {
        public CharacterStats stat;
        public float currentValue;
        public int currentLevel;
        public int maxLevel;
        public int initialPrice;
    }

    private void Awake()
    {
        //load the data from a file 
        PersistentUpgrades.Instance.Load();

        closeButton.onClick.AddListener(() =>
        {
            //close the panel
            gameObject.SetActive(false);
        });
        resetButton.onClick.AddListener(() =>
        {
            ResetUpgrades();
        });
    }

    private void Start()
    {
       newCharacterStats = PersistentUpgrades.Instance.saveData.newCharacterStats;
       totalCoinsSpent = PersistentUpgrades.Instance.saveData.totalCoinsSpent;
       coins = PersistentUpgrades.Instance.saveData.coins;

       InitializeUpgrades();
       UpdateAllCosts();
    }

    private void InitializeUpgrades()
    {
        newCharacterStats.ForEach(x =>
        {
            characterStats[(int)x.stat].text = x.currentLevel + "/" + x.maxLevel;
        });

        availableCoins.text = "COINS:" + PersistentUpgrades.Instance.saveData.coins.ToString();
    }

    /// <summary>
    /// Method handling the OnClick event for the plus button with a given index.
    /// </summary>
    /// <param name="i"></param>
    public void OnClickPlus(int i)
    {
        var currentUpgradeLevel = characterStats[i];
        var splitString = currentUpgradeLevel.text.Split("/");

        int available = new();
        int current = new();

        if(splitString.Length > 0)
        {
            current = int.Parse(splitString[0].Trim());
            available = int.Parse(splitString[1].Trim());
        }

        if (current >= available) return;
        

        var calculatedCost = 0;
        switch (i)
        {
            case (int)CharacterStats.MaxHp:
                var maxHp = newCharacterStats[(int)CharacterStats.MaxHp];
                calculatedCost = CalculateCost(maxHp.initialPrice, current);
                if (calculatedCost > coins) return;
                current++;
                maxHp.currentLevel = current;
                maxHp.currentValue += 10f;
                newCharacterStats[(int)CharacterStats.MaxHp] = maxHp;
                break;

            case (int)CharacterStats.Armor:
                var armor = newCharacterStats[(int)CharacterStats.Armor];
                calculatedCost = CalculateCost(armor.initialPrice, current);
                if (calculatedCost > coins) return;
                current++;
                armor.currentLevel = current;
                armor.currentValue += 1f;
                newCharacterStats[(int)CharacterStats.Armor] = armor;
                break;

            case (int)CharacterStats.HpRegen:
                var hpRegen = newCharacterStats[(int)CharacterStats.HpRegen];
                calculatedCost = CalculateCost(hpRegen.initialPrice, current);
                if (calculatedCost > coins) return;
                current++;
                hpRegen.currentLevel = current;
                hpRegen.currentValue += 0.1f;
                newCharacterStats[(int)CharacterStats.HpRegen] = hpRegen;
                break;

            case (int)CharacterStats.CooldownMultiplier:
                var cooldownMultiplier = newCharacterStats[(int)CharacterStats.CooldownMultiplier];
                calculatedCost = CalculateCost(cooldownMultiplier.initialPrice, current);
                if (calculatedCost > coins) return;
                current++;
                cooldownMultiplier.currentLevel = current;
                cooldownMultiplier.currentValue -= 0.02f;
                newCharacterStats[(int)CharacterStats.CooldownMultiplier] = cooldownMultiplier;
                break;

            case (int)CharacterStats.AreaMultiplier:
                var areaMultiplier = newCharacterStats[(int)CharacterStats.AreaMultiplier];
                calculatedCost = CalculateCost(areaMultiplier.initialPrice, current);
                if (calculatedCost > coins) return;
                current++;
                areaMultiplier.currentLevel = current;
                areaMultiplier.currentValue += 0.1f;
                newCharacterStats[(int)CharacterStats.AreaMultiplier] = areaMultiplier;
                break;

            case (int)CharacterStats.AmountBonus:
                var amountBonus = newCharacterStats[(int)CharacterStats.AmountBonus];
                calculatedCost = CalculateCost(amountBonus.initialPrice, current);
                if (calculatedCost > coins) return;
                current++;
                amountBonus.currentLevel = current;
                amountBonus.currentValue += 1f;
                newCharacterStats[(int)CharacterStats.AmountBonus] = amountBonus;
                break;

            case (int)CharacterStats.MagnetSize:
                var magentSize = newCharacterStats[(int)CharacterStats.MagnetSize];
                calculatedCost = CalculateCost(magentSize.initialPrice, current);
                if (calculatedCost > coins) return;
                current++;
                magentSize.currentLevel = current;
                magentSize.currentValue += 1f;
                newCharacterStats[(int)CharacterStats.MagnetSize] = magentSize;
                break;

            case (int)CharacterStats.DamageMultiplier:
                var damageMultiplier = newCharacterStats[(int)CharacterStats.DamageMultiplier];
                calculatedCost = CalculateCost(damageMultiplier.initialPrice, current);
                if (calculatedCost > coins) return;
                current++;
                damageMultiplier.currentLevel = current;
                damageMultiplier.currentValue += 0.1f;
                newCharacterStats[(int)CharacterStats.DamageMultiplier] = damageMultiplier;
                break;

            case (int)CharacterStats.ProjectileSpeed:
                var projectileSpeed = newCharacterStats[(int)CharacterStats.ProjectileSpeed];
                calculatedCost = CalculateCost(projectileSpeed.initialPrice, current);
                if (calculatedCost > coins) return;
                current++;
                projectileSpeed.currentLevel = current;
                projectileSpeed.currentValue += 0.2f;
                newCharacterStats[(int)CharacterStats.ProjectileSpeed] = projectileSpeed;
                break;
        }
        characterStats[i].text = current + "/" + available;;
        sumOfCoinsSpent += calculatedCost;
        UpdateAllCosts();

        coins -= calculatedCost;
        availableCoins.text = "COINS:" + coins;
        PersistentUpgrades.Instance.saveData.coins -= calculatedCost;
        PersistentUpgrades.Instance.saveData.totalCoinsSpent += calculatedCost;
        PersistentUpgrades.Instance.Save();
    }

    private int CalculateCost(int initialPrice, int bought, float value = 1f)
    {
        float cost;
        float totalBought = 0;

        foreach (var item in newCharacterStats)
        {
            totalBought += item.currentLevel;
        }
        
        if(totalBought > 0)
        cost = (initialPrice * (1 + bought)) + 20*Mathf.Pow(1.1f, totalBought-value); //calculate the cost of the next upgrade
        else
        cost = initialPrice * (1 + bought); //calculate the cost of the next upgrade


        return Mathf.FloorToInt(cost);
    }


    private void ResetUpgrades()
    {
        coins += sumOfCoinsSpent + totalCoinsSpent;
        PersistentUpgrades.Instance.saveData.coins = coins;
        PersistentUpgrades.Instance.saveData.totalCoinsSpent = 0;
        availableCoins.text = "COINS:" + coins;

        sumOfCoinsSpent = 0;
        totalCoinsSpent = 0;

        for (int i = 0; i < newCharacterStats.Count; ++i)
        {
            var item = newCharacterStats[i];
            item.currentLevel = 0;
            item.currentValue = 0;
            newCharacterStats[i] = item;
        }

        UpdateAllCosts();
        PersistentUpgrades.Instance.Save();


        newCharacterStats.ForEach(x =>
        {
            characterStats[(int)x.stat].text = x.currentLevel + "/" + x.maxLevel;
        });
    }

    private void UpdateAllCosts()
    {
        for (int i = 0; i < newCharacterStats.Count; ++i)
        {
            var item = newCharacterStats[i];
            if (item.currentLevel == item.maxLevel)
                costTexts[i].text = "MAX";
            else
            {
                var cost = CalculateCost(item.initialPrice, item.currentLevel);
                costTexts[i].text = cost.ToString();
            }
        }
    }
}
