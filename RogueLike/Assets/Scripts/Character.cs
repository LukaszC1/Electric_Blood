using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static ShopPanelUI;

/// <summary>
/// Abstract class which is the base class for all the characters in the game.
/// </summary>
public abstract class Character : NetworkBehaviour
{
    //Public fields
    [SerializeField] public NetworkVariable<int> maxHp = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public NetworkVariable<int> armor = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public NetworkVariable<float> hpRegen = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public NetworkVariable<float> damageMultiplier = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public NetworkVariable<float> areaMultiplier = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public NetworkVariable<float> projectileSpeedMultiplier = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public NetworkVariable<float> magnetSize = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public NetworkVariable<float> cooldownMultiplier = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public NetworkVariable<int> amountBonus = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public AudioSource xpSound;
    public NetworkVariable<ulong> playerID = new NetworkVariable<ulong>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentHp = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public ulong clientId;
    public float hpRegenTimer;
    public bool playerIsDead = false;

    //Private fields
    private bool indicatorNotLoaded = true;
    private bool nickNotLoaded = true;
    private float dissolveAmount = 1;
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
    [HideInInspector] public Magnet magnet;
    private PlayerMove playerMove;

    public void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
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

    private void Update()
    {
        if (!IsOwner) return;
        hpRegenTimer += Time.deltaTime * hpRegen.Value;

        if (hpRegenTimer > 1f)
        {
            Heal(1);
            hpRegenTimer -= 1f;
        }

    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (playerIsDead)
            isDyingUpdateServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeNicknameServerRpc(string nickname)
    {
        ChangeNicknameClientRpc(nickname);
        return;
    }

    [ClientRpc]
    private void ChangeNicknameClientRpc(string nickname)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = nickname;
        return;
    }

    private void Start()
    {
        if (!IsLocalPlayer)
        {
            if (indicatorNotLoaded)
            {
                var indicator = Instantiate(GameManager.Instance.indicatorPrefab, NetworkManager.Singleton.LocalClient.PlayerObject.transform);
                indicator.GetComponent<OffscreenIndicator>().target = this.transform;
                indicatorNotLoaded = false;
            }
        }

        if (!IsOwner) return;

        if (nickNotLoaded)
        {
            if(ElectricBloodMultiplayer.playMultiplayer)
                ChangeNicknameServerRpc(ElectricBloodMultiplayer.Instance.GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId).playerName.ToString());
            else
                GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);

            nickNotLoaded = false;
        }
      
        LoadPersistentUpgrades();

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

    private void LoadPersistentUpgrades()
    {
        if(PersistentUpgrades.Instance?.saveData != null) 
        {
            var saveData = PersistentUpgrades.Instance.saveData;

            saveData.newCharacterStats.ForEach(x =>
            {
                switch (x.stat)
                {
                    case CharacterStats.MaxHp:
                        maxHp.Value += Mathf.FloorToInt(x.currentValue);
                        break;
                    case CharacterStats.Armor:
                        armor.Value += Mathf.FloorToInt(x.currentValue);
                        break;
                    case CharacterStats.HpRegen:
                        hpRegen.Value += x.currentValue;
                        break;
                    case CharacterStats.DamageMultiplier:
                        damageMultiplier.Value += x.currentValue;
                        break;
                    case CharacterStats.AreaMultiplier:
                        areaMultiplier.Value += x.currentValue;
                        break;
                    case CharacterStats.ProjectileSpeed:
                        projectileSpeedMultiplier.Value += x.currentValue;
                        break;
                    case CharacterStats.MagnetSize:
                        magnetSize.Value += x.currentValue;
                        break;
                    case CharacterStats.CooldownMultiplier:
                        cooldownMultiplier.Value += x.currentValue;
                        break;
                    case CharacterStats.AmountBonus:
                        amountBonus.Value += Mathf.FloorToInt(x.currentValue);
                        break;
                }
            });
        }
    }

    /// <summary>
    /// Implemantation of taking damage by the player.
    /// </summary>
    /// <param name="damage"></param>
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

        if (currentHp.Value <= 0)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { playerID.Value }
                }
            };
            SwitchCameraClientRpc(clientRpcParams);
            if (!playerIsDead)
            {
                PlayerDeathClientRpc();
                playerIsDead = true;
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyGameObjectServerRpc()
    {
       this.GetComponent<NetworkObject>().Despawn();
    }

    [ClientRpc]
    public void TakeDamageClientRpc(int damage)
    {
        if (IsOwner)
        {
            TakeDamage(damage);
        }
    }

    [ClientRpc]
    private void SwitchCameraClientRpc(ClientRpcParams clientRpcParams = default)
    {
        foreach (var player in GameManager.Instance.listOfPlayerTransforms)
        {
            Character character = player.GetComponent<Character>();
            if (character.playerID != this.playerID)
            {
                character.camera.SetActive(true);
                break;
            }
        }
    }

    [ClientRpc]
    private void isDyingUpdateClientRpc()
    {
        dissolveAmount -= 0.05f;
        GetComponentInChildren<Renderer>().material.SetFloat("_Dissolve_Amount", dissolveAmount);
        if (dissolveAmount < 0)
        {
            CheckForGameOverServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void isDyingUpdateServerRpc()
    {
        isDyingUpdateClientRpc();
    }

    [ClientRpc]
    private void PlayerDeathClientRpc()
    {
        playerIsDead = true;
        playerMove.rgbd2d.simulated = false;
        playerMove.speed = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckForGameOverServerRpc()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length==1)
        {
            GameOverClientRpc();
            DestroyGameObjectServerRpc();
        }
        else
        {
            DestroyGameObjectServerRpc();
        }
    }

    [ClientRpc]
    private void GameOverClientRpc()
    {
        GetComponent<GameOver>().PlayerGameOver();
    }

    /// <summary>
    /// Function which applies armor to the damage taken.
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyArmor(ref int damage)
    {
        damage -= armor.Value;
        if (damage <= 0) { damage = 1; }
    }

    /// <summary>
    /// Method which heals the player.
    /// </summary>
    /// <param name="amount"></param>
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
            currentHp.Value = maxHp.Value;
        }
    }

    [ClientRpc]
    private void HealClientRpc(int amount)
    {
        if (IsOwner)
            Heal(amount);
    }

    /// <summary>
    /// Method which levels up the player.
    /// </summary>
    public void LevelUp()
    {

        if (!IsOwner) return;

        if (selectedUpgrades == null) { selectedUpgrades = new List<UpgradeData>(); }

        selectedUpgrades.Clear();
        selectedUpgrades.AddRange(GetUpgrades(4));

        if (selectedUpgrades.Count > 0)
            upgradePanelManager.OpenPanel(selectedUpgrades, NetworkManager.LocalClientId);

        magnet.LevelUpUpdate();

        LevelUpBonus();
    }

    [ServerRpc(RequireOwnership = false)]
    private void updateWeaponsServerRpc()
    {
        foreach (var weapon in weaponManager.weapons)
            weapon.LevelUpUpdate();
    }

    /// <summary>
    /// Method which returns a list of upgrades.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<UpgradeData> GetUpgrades(int count)
    {
        List<UpgradeData> upgradeList = new List<UpgradeData>();


        if (count > upgrades.Count)
            count = upgrades.Count;

        for (int i = 0; i < count; i++)
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

    /// <summary>
    /// Method which upgrades the player.
    /// </summary>
    /// <param name="selectedUpgrade"></param>
    public void Upgrade(int selectedUpgrade)
    {
        UpgradeData upgradeData = selectedUpgrades[selectedUpgrade];
        List<EquipedItem> iconList = new List<EquipedItem>();

        if (acquiredUpgrades == null) { acquiredUpgrades = new List<UpgradeData>(); }

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

    /// <summary>
    /// Method which upgrades the weapon from a pick up.
    /// </summary>
    /// <param name="upgradeData"></param>
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

    /// <summary>
    /// Method which upgrades the stats of the player.
    /// </summary>
    /// <param name="stats"></param>
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

    /// <summary>
    /// Method which adds an icon of the equipped item to the HUD.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="upgradeData"></param>
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

    /// <summary>
    /// Method which levels up the equipped item.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="upgradeData"></param>
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

    /// <summary>
    /// Method which checks if the player has max weapons.
    /// </summary>
    public void CheckForMaxWeapons()
    {
        if (weaponManager.weapons.Count >= 6)
        {
            upgrades.RemoveAll(x => x.upgradeType == UpgradeType.WeaponUnlock);
        }
    }

    /// <summary>
    /// Method which checks if the player has max items.
    /// </summary>
    public void CheckForMaxItems()
    {
        if (passiveItems.items.Count >= 6)
        {
            upgrades.RemoveAll(x => x.upgradeType == UpgradeType.ItemUnlock);
        }
    }

    private void AddUpgradesIntoList(List<UpgradeData> upgradesToAdd)
    {
        if (upgradesToAdd == null) { return; }

        this.upgrades.AddRange(upgradesToAdd);
    }

    /// <summary>
    /// Method which adds an upgrade into the list of upgrades.
    /// </summary>
    /// <param name="upgradeToAdd"></param>
    public void AddUpgradeIntoList(UpgradeData upgradeToAdd)
    {
        if (upgradeToAdd == null) { return; }

        upgrades.Add(upgradeToAdd);
    }

    /// <summary>
    /// Method which adds the acquired upgrades into the list of acquired upgrades.
    /// </summary>
    /// <param name="upgrade"></param>
    public void AcquiredUpgradesAdd(UpgradeData upgrade)
    {
        acquiredUpgrades.Add(upgrade);
    }

    /// <summary>
    /// Method which removes an upgrade from the list of upgrades.
    /// </summary>
    /// <param name="upgrade"></param>
    public void UpgradesRemove(UpgradeData upgrade)
    {
        upgrades.Remove(upgrade);
    }

    public abstract void LevelUpBonus();

    /// <summary>
    /// Method which initializes the player calss on NetworkSpawn.
    /// </summary>
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

    /// <summary>
    /// Method which vacuums the gems (pulls them towards the player).
    /// </summary>
    public void VacuumGems()
    {
        GameObject[] XPGems = GameObject.FindGameObjectsWithTag("XP");
        foreach (GameObject XPGem in XPGems)
        {
            if (XPGem.GetComponent<XPPickUpObject>() != null)
                XPGem.GetComponent<XPPickUpObject>().SetTargetDestination(this.transform);
            else
                XPGem.GetComponent<XPBankGem>().SetTargetDestination(this.transform);
        }
    }

}
