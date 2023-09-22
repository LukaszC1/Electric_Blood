using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public int maxHp = 100;
    public int armor = 0;
    public float hpRegen = 1f;
    public float damageMultiplier = 1f;
    public float areaMultiplier = 1f;
    public float projectileSpeedMultiplier = 1f;
    public float magnetSize = 1f;
    public float cooldownMultiplier = 1f;
    public int amountBonus = 0;

    public bool playerIsDead = false;

    public float hpRegenTimer;


    [HideInInspector] public int currentHp = 100;
    [SerializeField] StatusBar hpBar;
    [SerializeField] AudioSource xpSound;
    [HideInInspector]public int level = 1;
    float experience = 0;


    [SerializeField] ExperienceBar experienceBar;
    [SerializeField] UpgradePanelManager upgradePanelManager;
    [SerializeField] EquipedItemsManager equipedItemsManager;
    [SerializeField] List<UpgradeData> upgrades;

    List<UpgradeData> selectedUpgrades;
    [SerializeField] List<UpgradeData> acquiredUpgrades;
    [SerializeField] List<UpgradeData> upgradesAvailableOnStart;
    
    WeaponManager weaponManager;
    PassiveItems passiveItems;
    [HideInInspector]public Magnet magnet;

    public void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();
        magnet = GetComponent<Magnet>();
        passiveItems = GetComponent<PassiveItems>();
    }

    public void Update()
    {
        hpRegenTimer += Time.deltaTime * hpRegen;

        if(hpRegenTimer > 1f)
        {
            Heal(1);
            hpRegenTimer -= 1f;
        }
    
    }

    public void FixedUpdate()
    {
        CheckLevelUp();
    }

    int TO_LEVEL_UP()
    {
        if (level <= 20)
            return 5 + (level - 1) * 10;
        else if (level > 20 && level <= 40)
            return 5 + (level - 1) * 13;
        else
            return 5 + (level - 1) * 16;
    }

    public void Start()
    {
        currentHp = maxHp;
        experienceBar.UpdateExperienceSlider(experience, TO_LEVEL_UP());
        experienceBar.SetLevelText(level);
        hpBar.SetState(currentHp, maxHp);
        AddUpgradesIntoList(upgradesAvailableOnStart);
    }

    public void TakeDamage(int damage)
    {
        if (playerIsDead) return;
        ApplyArmor(ref damage);
        currentHp -= damage;

        if(currentHp <= 0)
        {
            //Destroy(gameObject);
           

            if (!playerIsDead)
            {
                playerIsDead = true;
                GetComponent<GameOver>().PlayerGameOver();
            }
       }

        hpBar.SetState(currentHp, maxHp);
    }

    public void ApplyArmor(ref int damage)
    {
        damage -= armor;
        if (damage <= 0) { damage = 1; }
    }


    public void Heal(int amount)
    {
        if (currentHp <= 0) { return; }

        currentHp += amount;
        if (currentHp > maxHp)
        {
            currentHp=maxHp;
        }
        hpBar.SetState(currentHp, maxHp);
    }

    public void AddExperience(float amount)
    {
        experience += amount;
        xpSound.Play();
        experienceBar.UpdateExperienceSlider(experience, TO_LEVEL_UP());
    }


    public void CheckLevelUp()
    {
        if (experience >= TO_LEVEL_UP())
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        if(selectedUpgrades == null) { selectedUpgrades = new List<UpgradeData>(); }
        selectedUpgrades.Clear();
        selectedUpgrades.AddRange(GetUpgrades(4));

        if(selectedUpgrades.Count > 0)
        upgradePanelManager.OpenPanel(selectedUpgrades);

        experience -= TO_LEVEL_UP();
        level += 1;
        experienceBar.SetLevelText(level);
        experienceBar.UpdateExperienceSlider(experience, TO_LEVEL_UP());

        magnet.LevelUpUpdate();

        LevelUpBonus();
        updateWeapons();
    }

    public void updateWeapons()
    {
        foreach (var weapon in weaponManager.weapons)
            weapon.LevelUpUpdate();
    }

    public List<UpgradeData> GetUpgrades(int count)
    {
        List<UpgradeData> upgradeList = new List<UpgradeData>();


        if(count > upgrades.Count)
            count = upgrades.Count;

        for(int i = 0; i < count; i++)
        {
            UpgradeData upgradeData = upgrades[Random.Range(0, upgrades.Count)];
            while (upgradeList.Contains(upgradeData))
            {
                upgradeData = upgrades[Random.Range(0, upgrades.Count)];
            }
            upgradeList.Add(upgradeData); //select random upgrades from the list
        }

        return upgradeList;
    }

    public void Upgrade (int selectedUpgrade)
    {
        UpgradeData upgradeData = selectedUpgrades[selectedUpgrade];
        List<EquipedItem> iconList = new List<EquipedItem>();

       if( acquiredUpgrades == null ) { acquiredUpgrades = new List<UpgradeData>(); }

        switch (upgradeData.upgradeType)
        {
            case UpgradeType.WeaponUpgrade:
                weaponManager.UpgradeWeapon(upgradeData);
                iconList = equipedItemsManager.ReturnWeaponsIcons();
                AddLevel(iconList, upgradeData);
                break;
            case UpgradeType.ItemUpgrade:
                passiveItems.UpgradeItem(upgradeData);
                iconList = equipedItemsManager.ReturnItemsIcons();
                AddLevel(iconList, upgradeData);
                break;
            case UpgradeType.WeaponUnlock:
                weaponManager.AddWeapon(upgradeData.weaponData);
                CheckForMaxWeapons();
                iconList = equipedItemsManager.ReturnWeaponsIcons();
                AddIcon(iconList, upgradeData);
                break;
            case UpgradeType.ItemUnlock:
                passiveItems.Equip(upgradeData.item);
                CheckForMaxItems();
                iconList = equipedItemsManager.ReturnItemsIcons();
                AddIcon(iconList, upgradeData);
                break;
        }

       acquiredUpgrades.Add(upgradeData);
       upgrades.Remove(upgradeData);
   

    }
    public void UpgradeWeaponPickUp(UpgradeData upgradeData)
    {
        List<EquipedItem> iconList = new List<EquipedItem>();

        if (upgradeData.upgradeType == UpgradeType.WeaponUpgrade)
        {   
            iconList = equipedItemsManager.ReturnWeaponsIcons();
        }
        else
            iconList = equipedItemsManager.ReturnItemsIcons();

        AddLevel(iconList, upgradeData);
        weaponManager.UpgradeWeapon(upgradeData);
    }

    public void AddIcon(List<EquipedItem> input, UpgradeData upgradeData)
    {
        foreach (var icon in input) //find first empty slot and add the icon
        {
            if (icon.isSet != true)
            {
                icon.Set(upgradeData);
                break;
            }
        }
    }

    public void AddLevel(List<EquipedItem> input, UpgradeData upgradeData)
    {
        foreach (var icon in input)
        {
            if (icon.isSet == true && icon.icon.sprite == upgradeData.icon)
            {
                icon.LevelEquipedItem();
                break;
            }
        }
    }


    public void CheckForMaxWeapons()
    {
        if(weaponManager.weapons.Count >= 6)
        {
            upgrades.RemoveAll(x => x.upgradeType == UpgradeType.WeaponUnlock);
        }
    }

    public void CheckForMaxItems()
    {
        if (passiveItems.items.Count >= 6)
        {
            upgrades.RemoveAll(x => x.upgradeType == UpgradeType.ItemUnlock);
        }
    }

    internal void AddUpgradesIntoList(List<UpgradeData> upgradesToAdd)
    {
        if(upgradesToAdd == null) { return; }

       this.upgrades.AddRange(upgradesToAdd);
    }
    internal void AddUpgradeIntoList(UpgradeData upgradeToAdd)
    {
        if (upgradeToAdd == null) { return; }

        upgrades.Add(upgradeToAdd);
    }

    internal void AcquiredUpgradesAdd(UpgradeData upgrade)
    {
        acquiredUpgrades.Add(upgrade);
    }

    internal void UpgradesRemove(UpgradeData upgrade)
    {
        upgrades.Remove(upgrade);
    }

    public abstract void LevelUpBonus();
}
