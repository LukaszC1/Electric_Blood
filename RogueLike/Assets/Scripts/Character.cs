using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Character : NetworkBehaviour
{
    public NetworkVariable<int> maxHp = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> armor = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> hpRegen = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> damageMultiplier = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> areaMultiplier = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> projectileSpeedMultiplier = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> magnetSize = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> cooldownMultiplier = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> amountBonus = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public bool playerIsDead = false;

    public float hpRegenTimer;
    [SerializeField] AudioSource xpSound;


    [HideInInspector] public NetworkVariable<int> currentHp = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] StatusBar hpBar;

    private UpgradePanelManager upgradePanelManager;
    [SerializeField] EquipedItemsManager equipedItemsManager;
    [SerializeField] List<UpgradeData> upgrades;

    List<UpgradeData> selectedUpgrades;
    [SerializeField] List<UpgradeData> acquiredUpgrades;
    [SerializeField] List<UpgradeData> upgradesAvailableOnStart;
    
    WeaponManager weaponManager;
    PassiveItems passiveItems;
    [SerializeField] GameObject camera;
    [HideInInspector]public Magnet magnet;

    public void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();
        magnet = GetComponent<Magnet>();
        passiveItems = GetComponent<PassiveItems>();
        upgradePanelManager = FindObjectOfType<UpgradePanelManager>();
        equipedItemsManager = FindObjectOfType<EquipedItemsManager>(); // this will need to be reworked for multiple clients :3
    }

    public void Update()
    {    
        if (!IsOwner) return;
        hpRegenTimer += Time.deltaTime * hpRegen.Value;

        if(hpRegenTimer > 1f)
        {
            Heal(1);
            hpRegenTimer -= 1f;
        }
    
    }



    public void Start()
    {
        currentHp.Value = maxHp.Value;
        hpBar.SetState(currentHp.Value, maxHp.Value);
        AddUpgradesIntoList(upgradesAvailableOnStart);
    }

    public void TakeDamage(int damage)
    {
        if (playerIsDead) return;
        ApplyArmor(ref damage);
        currentHp.Value -= damage;

        if(currentHp.Value <= 0)
        {
            //Destroy(gameObject);
           

            if (!playerIsDead)
            {
                playerIsDead = true;
                GetComponent<GameOver>().PlayerGameOver();
            }
       }

        hpBar.SetState(currentHp.Value, maxHp.Value);
    }

    public void ApplyArmor(ref int damage)
    {
        damage -= armor.Value;
        if (damage <= 0) { damage = 1; }
    }


    public void Heal(int amount)
    {
        if (currentHp.Value <= 0) { return; }
        if (!IsOwner) return;

        currentHp.Value += amount;
        if (currentHp.Value > maxHp.Value)
        {
            currentHp.Value=maxHp.Value;
        }
        hpBar.SetState(currentHp.Value, maxHp.Value);
    }

    public void AddExperience(float amount)
    {
        GameManager.Instance.experience += amount;
        xpSound.Play();
        GameManager.Instance.experienceBar.UpdateExperienceSlider(GameManager.Instance.experience, GameManager.Instance.TO_LEVEL_UP());
    }

    public void LevelUp()
    {

        if (!IsOwner) return;

        if(selectedUpgrades == null) { selectedUpgrades = new List<UpgradeData>(); }
    
        selectedUpgrades.Clear();
        selectedUpgrades.AddRange(GetUpgrades(4));

        if (selectedUpgrades.Count > 0)
        upgradePanelManager.OpenPanel(selectedUpgrades, NetworkManager.LocalClientId);

        magnet.LevelUpUpdate();

        LevelUpBonus();
        updateWeaponsServerRpc();
    }

    [ServerRpc]
    public void updateWeaponsServerRpc()
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

    public void UpgradeStats(ItemStats stats)
    {
        maxHp.Value += stats.maxHp;
        armor.Value += stats.armor;
        hpRegen.Value += stats.hpRegen;
        damageMultiplier.Value += stats.dmgMultiplier;
        areaMultiplier.Value += stats.aoeMultiplier;
        projectileSpeedMultiplier.Value += stats.projectileSpeedMultiplier;
        magnetSize.Value += stats.magnetSize;
        cooldownMultiplier.Value -= stats.cdMultiplier;
        amountBonus.Value += stats.amountBonus;
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

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            GameManager.Instance.listOfPlayers.Add(NetworkManager.LocalClientId, transform);
        }
        if (IsOwner && camera.activeSelf == false)
        {
            camera.SetActive(true);
        }
    }
}
