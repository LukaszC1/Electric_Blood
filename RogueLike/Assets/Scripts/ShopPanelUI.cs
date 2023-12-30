using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button resetButton;

    [SerializeField] private TextMeshProUGUI availableCoins;

    [SerializeField] private List<TextMeshProUGUI> characterStats;
    [SerializeField] private List<TextMeshProUGUI> costTexts;

    private List<LoadedStats> newCharacterStats = new();
    private int sumOfCoinsSpent = 0;
    private int coins;

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
        //PersistentUpgrades.Instance.Load();

        closeButton.onClick.AddListener(() =>
        {
            //close the panel
            gameObject.SetActive(false);
        });
        buyButton.onClick.AddListener(() =>
        {           
            if(coins >= sumOfCoinsSpent)
            {
                PersistentUpgrades.Instance.saveData.coins -= sumOfCoinsSpent;             
                availableCoins.text = "COINS:" + PersistentUpgrades.Instance.saveData.coins.ToString();
                //PersistentUpgrades.Instance.Save();
                gameObject.SetActive(false);
            }
        });
        resetButton.onClick.AddListener(() =>
        {
            ResetUpgrades();
        });
    }

    private void Start()
    {
       newCharacterStats = PersistentUpgrades.Instance.saveData.newCharacterStats;
       sumOfCoinsSpent = PersistentUpgrades.Instance.saveData.sumOfCoinsSpent;
       coins = PersistentUpgrades.Instance.saveData.coins;

       InitializeUpgrades();
    }

    private void InitializeUpgrades()
    {
        newCharacterStats.ForEach(x =>
        {
            characterStats[(int)x.stat].text = x.currentLevel + "/" + x.maxLevel;
        });

        availableCoins.text = "COINS:" + PersistentUpgrades.Instance.saveData.coins.ToString();
    }

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

        if (current < available)
        {
            current++;
            characterStats[i].text = current + "/" + available;
        }
        else
            return;
        

        var calculatedCost = 0;
        switch (i)
        {
            case (int)CharacterStats.MaxHp:
                var maxHp = newCharacterStats[(int)CharacterStats.MaxHp];
                maxHp.currentLevel = current;
                maxHp.currentValue += 10f;
                newCharacterStats[(int)CharacterStats.MaxHp] = maxHp;
                calculatedCost = CalculateCost(maxHp.initialPrice, current - 1);
                break;

            case (int)CharacterStats.Armor:
                var armor = newCharacterStats[(int)CharacterStats.Armor];
                armor.currentLevel = current;
                armor.currentValue += 1f;
                newCharacterStats[(int)CharacterStats.Armor] = armor;
                calculatedCost = CalculateCost(armor.initialPrice, current - 1);
                break;

            case (int)CharacterStats.HpRegen:
                var hpRegen = newCharacterStats[(int)CharacterStats.HpRegen];
                hpRegen.currentLevel = current;
                hpRegen.currentValue += 0.1f;
                newCharacterStats[(int)CharacterStats.HpRegen] = hpRegen;
                calculatedCost = CalculateCost(hpRegen.initialPrice, current - 1);
                break;

            case (int)CharacterStats.CooldownMultiplier:
                var cooldownMultiplier = newCharacterStats[(int)CharacterStats.CooldownMultiplier];
                cooldownMultiplier.currentLevel = current;
                cooldownMultiplier.currentValue -= 0.02f;
                newCharacterStats[(int)CharacterStats.CooldownMultiplier] = cooldownMultiplier;
                calculatedCost = CalculateCost(cooldownMultiplier.initialPrice, current - 1);
                break;

            case (int)CharacterStats.AreaMultiplier:
                var areaMultiplier = newCharacterStats[(int)CharacterStats.AreaMultiplier];
                areaMultiplier.currentLevel = current;
                areaMultiplier.currentValue += 0.1f;
                newCharacterStats[(int)CharacterStats.AreaMultiplier] = areaMultiplier;
                calculatedCost = CalculateCost(areaMultiplier.initialPrice, current - 1);
                break;

            case (int)CharacterStats.AmountBonus:
                var amountBonus = newCharacterStats[(int)CharacterStats.AmountBonus];
                amountBonus.currentLevel = current;
                amountBonus.currentValue += 1f;
                newCharacterStats[(int)CharacterStats.AmountBonus] = amountBonus;
                calculatedCost = CalculateCost(amountBonus.initialPrice, current - 1);
                break;

            case (int)CharacterStats.MagnetSize:
                var magentSize = newCharacterStats[(int)CharacterStats.MagnetSize];
                magentSize.currentLevel = current;
                magentSize.currentValue += 1f;
                newCharacterStats[(int)CharacterStats.MagnetSize] = magentSize;
                calculatedCost = CalculateCost(magentSize.initialPrice, current - 1);
                break;

            case (int)CharacterStats.DamageMultiplier:
                var damageMultiplier = newCharacterStats[(int)CharacterStats.DamageMultiplier];
                damageMultiplier.currentLevel = current;
                damageMultiplier.currentValue += 0.1f;
                newCharacterStats[(int)CharacterStats.DamageMultiplier] = damageMultiplier;
                calculatedCost = CalculateCost(damageMultiplier.initialPrice, current - 1);
                break;

            case (int)CharacterStats.ProjectileSpeed:
                var projectileSpeed = newCharacterStats[(int)CharacterStats.ProjectileSpeed];
                projectileSpeed.currentLevel = current;
                projectileSpeed.currentValue += 0.2f;
                newCharacterStats[(int)CharacterStats.ProjectileSpeed] = projectileSpeed;
                calculatedCost = CalculateCost(projectileSpeed.initialPrice, current - 1);
                break;
        }
        var currentPrice = int.Parse(costTexts[i].text);
        currentPrice += calculatedCost;
        costTexts[i].text = currentPrice.ToString();
        sumOfCoinsSpent += calculatedCost;

        coins -= calculatedCost;
        availableCoins.text = "COINS:" + coins;
    }

    private int CalculateCost(int initialPrice, int bought, float value = 1f)
    {
        float cost;
        float totalBought = 0;

        foreach (var item in newCharacterStats)
        {
            totalBought += item.currentLevel;
        }
        
        cost = (initialPrice * (1 + bought)) + 20*Mathf.Pow(1.1f, totalBought-value); //calculate the cost of the next upgrade

        return Mathf.FloorToInt(cost);
    }


    private void ResetUpgrades()
    {
        if (sumOfCoinsSpent >= coins)
        {
            availableCoins.text = "COINS: " + PersistentUpgrades.Instance.saveData.coins;
        }
        else
        {
            coins += sumOfCoinsSpent;
            PersistentUpgrades.Instance.saveData.coins = coins;
            availableCoins.text = "COINS: " + coins;
        }

        sumOfCoinsSpent = 0;
        for (int i = 0; i < newCharacterStats.Count; ++i)
        {
            var item = newCharacterStats[i];
            item.currentLevel = 0;
            item.currentValue = 0;
            newCharacterStats[i] = item;
        }

        foreach (var text in costTexts)
        {
            text.text = "0";
        }

        newCharacterStats.ForEach(x =>
        {
            characterStats[(int)x.stat].text = x.currentLevel + "/" + x.maxLevel;
        });
    }
}
