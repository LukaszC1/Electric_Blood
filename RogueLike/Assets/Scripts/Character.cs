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
    public ulong clientId;
    public float hpRegenTimer;

    [SerializeField] public AudioSource xpSound;

    public NetworkVariable<ulong> playerID = new NetworkVariable<ulong>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> currentHp = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
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
   
    private void NetworkVariable_OnStatsChanged(float previousValue, float newValue)
    {
        updateWeaponsServerRpc();
    }

    private void NetworkVariable_OnStatsChanged(int previousValue, int newValue)
    {
        updateWeaponsServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateHpBarServerRpc(int previousValue, int newValue)
    {
        UpdateHpBarClientRpc();
    }

    [ClientRpc]
    private void UpdateHpBarClientRpc()
    {
        hpBar.SetState(currentHp.Value, maxHp.Value);
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
        if(!IsOwner) return;

        currentHp.Value = maxHp.Value;

        maxHp.OnValueChanged += NetworkVariable_OnStatsChanged;
        armor.OnValueChanged += NetworkVariable_OnStatsChanged;
        hpRegen.OnValueChanged += NetworkVariable_OnStatsChanged;
        damageMultiplier.OnValueChanged += NetworkVariable_OnStatsChanged;
        areaMultiplier.OnValueChanged += NetworkVariable_OnStatsChanged;
        projectileSpeedMultiplier.OnValueChanged += NetworkVariable_OnStatsChanged;
        magnetSize.OnValueChanged += NetworkVariable_OnStatsChanged;
        cooldownMultiplier.OnValueChanged += NetworkVariable_OnStatsChanged;
        amountBonus.OnValueChanged += NetworkVariable_OnStatsChanged;
        currentHp.OnValueChanged += UpdateHpBarServerRpc;

        AddUpgradesIntoList(upgradesAvailableOnStart);
        clientId = NetworkManager.Singleton.LocalClientId;
    }

    public void TakeDamage(int damage)
    {
        if (!IsOwner)  //only for testing
        {
            TakeDamageClientRpc(damage);
            return;
        }
        if (playerIsDead) return;

        ApplyArmor(ref damage);
        currentHp.Value -= damage;

        if(currentHp.Value <= 0)
        {
            //Destroy(gameObject);
           

            if (!playerIsDead)
            {
                playerIsDead = true;
                //GetComponent<GameOver>().PlayerGameOver();
            }
       }

    }
    [ClientRpc]
    public void TakeDamageClientRpc(int damage)
    {
        if (IsOwner)
        {
          TakeDamage(damage);
        }
    }

    public void ApplyArmor(ref int damage)
    {
        damage -= armor.Value;
        if (damage <= 0) { damage = 1; }
    }


    public void Heal(int amount)
    {
        if (currentHp.Value <= 0) { return; }
        if (!IsOwner)
        {
            HealClientRpc(amount);
            return;
        }

        currentHp.Value += amount;
        if (currentHp.Value > maxHp.Value)
        {
            currentHp.Value=maxHp.Value;
        }
    }

    [ClientRpc]
    private void HealClientRpc(int amount)
    {
        if (IsOwner)
            Heal(amount);
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
    }

    [ServerRpc(RequireOwnership = false)]
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
            playerID.Value = NetworkManager.Singleton.LocalClientId;
        }
        if (IsOwner && camera.activeSelf == false)
        {
            camera.SetActive(true);
        }
    }

    public void VacuumGems()
    {
        GameObject[] XPGems = GameObject.FindGameObjectsWithTag("XP");
        foreach (GameObject XPGem in XPGems)
        {
            if(XPGem.GetComponent<XPPickUpObject>() != null)
                XPGem.GetComponent<XPPickUpObject>().SetTargetDestination(this.transform);
            else
                XPGem.GetComponent<XPBankGem>().SetTargetDestination(this.transform);
        }
    }

}
