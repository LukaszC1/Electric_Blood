using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using static ShopPanelUI;

/// <summary>
/// The class handling the persistent upgrades saving and loading.
/// </summary>
public class PersistentUpgrades : MonoBehaviour
{
    /// <summary>
    /// Field containing the data to be saved.
    /// </summary>
    public SaveData saveData = new SaveData();
    public static PersistentUpgrades Instance { get; private set;}

    private void Awake()
    {
        Instance = this;
        //load the data from a file 
        Load();
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Method which saves the data to a file.
    /// </summary>
    public void Save()
    {
        string saveDataJson = JsonUtility.ToJson(saveData);
        string filePath = Application.persistentDataPath + "/saveData.json";
        File.WriteAllText(filePath, saveDataJson);
        Debug.Log("Data saved at: " + filePath);
    }

    /// <summary>
    /// Method which loads the data from a file.
    /// </summary>
    public void Load()
    {
        string filePath = Application.persistentDataPath + "/saveData.json";

        if (!File.Exists(filePath))
        {
            Save();
            return;
        }

        string saveDataJson = File.ReadAllText(filePath);

        if(saveDataJson != null && saveDataJson != string.Empty)
        saveData = JsonUtility.FromJson<SaveData>(saveDataJson);
        Debug.Log("Data loaded at: " + filePath);
    }

    /// <summary>
    /// Method which increases the number of coins.
    /// </summary>
    /// <param name="v"></param>
    public void AddCoins(int v)
    {
       saveData.coins += v;
    }
}

/// <summary>
/// The class containing the data to be saved.
/// </summary>
[Serializable]
public class SaveData
{
    /// <summary>
    /// Field containing the number of coins collected.
    /// </summary>
    public int coins
    {
        get
        {
            return _coins;
        }

        set
        {
            OnValueChanged(_coins, value);
            _coins = value;       
        }
    }
    [FormerlySerializedAs("coins")]
    [SerializeField]
    private int _coins = 0;

    /// <summary>
    /// Field containing the number of total coins spent.
    /// </summary>
    public int totalCoinsSpent = 0;
    private void OnValueChanged(int oldValue, int newValue)
    {
        GameManager.Instance?.UpdateCoins(newValue);
        Debug.Log("Coins changed from " + oldValue + " to " + newValue);
    }

    /// <summary>
    /// List containing the upggraded stats of the character.
    /// </summary>
    public List<LoadedStats> newCharacterStats = new List<LoadedStats>{
        new LoadedStats { stat = CharacterStats.MaxHp, currentValue = 0, currentLevel = 0, maxLevel = 5, initialPrice = 100 },
        new LoadedStats { stat = CharacterStats.Armor, currentValue = 0, currentLevel = 0, maxLevel = 1, initialPrice = 1000 },
        new LoadedStats { stat = CharacterStats.HpRegen, currentValue = 0, currentLevel = 0, maxLevel = 5, initialPrice = 100 },
        new LoadedStats { stat = CharacterStats.DamageMultiplier, currentValue = 0, currentLevel = 0, maxLevel = 5, initialPrice = 100 },
        new LoadedStats { stat = CharacterStats.AreaMultiplier, currentValue = 0, currentLevel = 0, maxLevel = 5, initialPrice = 100 },
        new LoadedStats { stat = CharacterStats.ProjectileSpeed, currentValue = 0, currentLevel = 0, maxLevel = 5, initialPrice = 100 },
        new LoadedStats { stat = CharacterStats.MagnetSize, currentValue = 0, currentLevel = 0, maxLevel = 2, initialPrice = 100 },
        new LoadedStats { stat = CharacterStats.CooldownMultiplier, currentValue = 0, currentLevel = 0, maxLevel = 5, initialPrice = 300 },
        new LoadedStats { stat = CharacterStats.AmountBonus, currentValue = 0, currentLevel = 0, maxLevel = 1, initialPrice = 2000 }
    };
}