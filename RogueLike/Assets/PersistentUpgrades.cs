using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Tutorials.Core.Editor;
using UnityEngine;

/// <summary>
/// The class handling the persistent upgrades saving and loading.
/// </summary>
public class PersistentUpgrades : MonoBehaviour
{
    private SaveData saveData { get; set; } = new SaveData();
    public static PersistentUpgrades Instance { get; private set;}

    private void Awake()
    {
        Instance = this;
        //load the data from a file 
        Load();
        DontDestroyOnLoad(this);
    }

    private void OnApplicationQuit()
    {
        //serialize the data
        Save();
    }

    public void Save()
    {
        string saveDataJson = JsonUtility.ToJson(saveData);
        string filePath = Application.persistentDataPath + "/saveData.json";
        File.WriteAllText(filePath, saveDataJson);
        Debug.Log("Data saved at: " + filePath);
    }

    public void Load()
    {
        string filePath = Application.persistentDataPath + "/saveData.json";

        if (!File.Exists(filePath))
        {
            return;
        }

        string saveDataJson = File.ReadAllText(filePath);

        if(!saveDataJson.IsNullOrEmpty())
        saveData = JsonUtility.FromJson<SaveData>(saveDataJson);
        Debug.Log("Data loaded at: " + filePath);
    }
}

/// <summary>
/// The class containing the data to be saved.
/// </summary>
[Serializable]
public class SaveData
{
    public int coins;
    public int[] characterStats;

    /// <summary>
    /// Updates the data to be saved.
    /// </summary>
    /// <param name="coins"></param>
    /// <param name="characterStats"></param>
    public void UpdateData(int coins, int[] characterStats)
    {
        this.coins = coins;
        this.characterStats = characterStats;
    }
}