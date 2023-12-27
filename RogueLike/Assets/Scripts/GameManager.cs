using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    [HideInInspector] public int xpGemAmount;
    [HideInInspector] public float xpBank;
    public Dictionary<ulong, Transform> listOfPlayers = new();
    public List<Transform> listOfPlayerTransforms = new();
    private float timer = 1;
    [HideInInspector] NetworkVariable<int> killCount = new NetworkVariable<int>(0);
    [SerializeField] TMPro.TextMeshProUGUI killCounter;
    [SerializeField] GameObject xpBankGemPrefab;
    GameObject xpBankGem;
    [SerializeField] GameObject breakableObject;

    [HideInInspector] public NetworkVariable<int> level = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> experience = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public ExperienceBar experienceBar;
    [SerializeField] public AudioSource xpSound;

    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private bool isLocalPlayerReady;
    private bool isLocalGamePaused = false;
    private bool indicatorNotLoaded = true;

    [SerializeField] private WaitingForOtherPlayersUI waitingForOtherPlayersUI;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);

    private Dictionary<ulong, bool> playerPausedDictionary;

    [SerializeField] private GameObject characterPanel;
    [SerializeField] public GameObject singleplayerCamera;
    [SerializeField] public GameObject gameOverPanel;
    [SerializeField] private GameObject indicatorPrefab;
    private void Awake()
    {
        Instance = this;

        playerPausedDictionary = new Dictionary<ulong, bool>();
 
        experienceBar = FindObjectOfType<ExperienceBar>();
        experienceBar.UpdateExperienceSlider(experience.Value, TO_LEVEL_UP());
        experienceBar.SetLevelText(level.Value);

    }
    private void Start()
    {
        PlayerMove.OnPauseAction += OnPauseAction;
        UpgradePanelManager.OnPauseAction += OnPauseAction;
        experience.OnValueChanged += UpdateExpSlider;
        level.OnValueChanged += UpdateLevelText;
        killCount.OnValueChanged += UpdateKillCounter;
    }
    public void OnDestroy()
    {
        PlayerMove.OnPauseAction -= OnPauseAction;
        UpgradePanelManager.OnPauseAction -= OnPauseAction;
    }
    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (xpBank > 100 && !xpBankGem.activeSelf)
        {
            Transform player = listOfPlayerTransforms[UnityEngine.Random.Range(0, listOfPlayerTransforms.Count)];
            SetActiveClientRpc(xpBankGem);
            Vector3 position = GenerateRandomPosition(player.transform.position);
            while (CheckForCollision(position))
                position = GenerateRandomPosition(player.transform.position);
            xpBankGem.transform.position = position;
        }

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            if (UnityEngine.Random.value <= 0.075)
            {
                Transform player = listOfPlayerTransforms[UnityEngine.Random.Range(0, listOfPlayerTransforms.Count)];
                GameObject breakable = Instantiate(breakableObject);
                Vector3 position = GenerateRandomPosition(player.transform.position);
                while (CheckForCollision(position))
                    position = GenerateRandomPosition(player.transform.position);
                breakable.transform.position = position;
                breakable.GetComponent<NetworkObject>().Spawn();
            }
            timer = 1;
        }

        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    state.Value = State.GamePlaying;
                }
                break;
            case State.GamePlaying:

                gamePlayingTimer.Value += Time.deltaTime;
                break;
            case State.GameOver:
                break;
        }
    }

    public void FixedUpdate()
    {
        CheckLevelUp();
        RefreshListOfPlayers(); //THIS IS ONLY UNTIL LOBBY WORKS

        if (indicatorNotLoaded && NetworkManager.ConnectedClientsList.Count == listOfPlayerTransforms.Count)
        {
            //Spawn the indicators
            if (NetworkManager.ConnectedClientsList?.Count > 1)
            {
                foreach (var client in listOfPlayerTransforms)
                {
                    Instantiate(indicatorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                }
                indicatorNotLoaded = false;
            }
        }
    }

    private void OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGameWithMenuScreen();
    }

    private void TogglePauseGameWithMenuScreen()
    {
        isLocalGamePaused = !isLocalGamePaused;
        WaitingForOtherPlayersUIServerRpc();
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnpauseGameServerRpc();
            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    public void TogglePauseGame(bool toggle = true)
    {
        isLocalGamePaused = !isLocalGamePaused;

        if(toggle)
        WaitingForOtherPlayersUIServerRpc();

        if (isLocalGamePaused)
        {
            PauseGameServerRpc();
        }
        else
        {
            UnpauseGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;

        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;

        TestGamePausedState();
    }

    private void TestGamePausedState()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                isGamePaused.Value = true;
                return;
            }
        }
        isGamePaused.Value = false;
    }

    public bool IsGamePaused()
    {
        return isGamePaused.Value;
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }
    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            xpBankGem = Instantiate(xpBankGemPrefab);
            xpBankGem.GetComponent<NetworkObject>().Spawn();
            xpBankGem.SetActive(false);
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (ElectricBloodMultiplayer.playMultiplayer == false)
        {
            //enable a default camera before the player camera gets spawned
            singleplayerCamera.SetActive(true);
            characterPanel.SetActive(true);

            Time.timeScale = 0f;
        }
        else
        { 
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerData = ElectricBloodMultiplayer.Instance.GetPlayerDataFromClientId(clientId);
                var characterData = ElectricBloodMultiplayer.Instance.availableCharacters[playerData.characterIndex] as CharacterData;

                GameObject playerSpawned = Instantiate(characterData.characterPrefab);
                playerSpawned.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            }
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        TogglePauseGame(false); //pause game when client disconnects
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;

            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;

            OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public Vector3 GenerateRandomPosition(Vector3 playerPos)
    {
        Vector3 position = new Vector3();

        float f = UnityEngine.Random.value > 0.5f ? -1f : 1f;

        if (UnityEngine.Random.value > 0.5f)
        {
            position.x = UnityEngine.Random.Range(-17, 8);
            position.y = 8 * f;
        }
        else
        {
            position.y = UnityEngine.Random.Range(-17, 8);
            position.x = 17 * f;
        }
        position.x += playerPos.x;
        position.y += playerPos.y;
        position.z = 0;

        return position;
    }

    private bool CheckForCollision(Vector3 position)
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collision in collisions)
        {
            if (collision.transform.name == "buildings")
            {
                return true;
            }
        }
        Collider2D[] collisionsOtherPlayers = Physics2D.OverlapBoxAll(position, new Vector2(12f, 5f), 0f);
        foreach (Collider2D collision in collisionsOtherPlayers)
        {
            if (collision.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    public void IncrementKillCount()
    {
        if (IsServer)
        {
            killCount.Value += 1;
        }
    }

    public int TO_LEVEL_UP()
    {
        if (level.Value <= 20)
            return 5 + (level.Value - 1) * 10;
        else if (level.Value > 20 && level.Value <= 40)
            return 5 + (level.Value - 1) * 13;
        else
            return 5 + (level.Value - 1) * 16;
    }

    public void CheckLevelUp()
    {
        if (experience.Value >= TO_LEVEL_UP())
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        if (!IsOwner) return;
        experience.Value -= TO_LEVEL_UP();
        level.Value += 1;
        LevelUpClientRpc();
    }
    public void RefreshListOfPlayers()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        listOfPlayers.Clear();
        foreach (var player in players)
        {
            Character character = player.GetComponent<Character>();
            listOfPlayers.TryAdd(character.playerID.Value, character.transform);
        }
        listOfPlayerTransforms = new List<Transform>(listOfPlayers.Values);
    }

    public void UpdateKillCounter(int previousValue, int nextValue)
    {
        killCounter.text = ":" + killCount.Value.ToString();
    }

    public void UpdateExpSlider(float previousValue, float nextValue)
    {
        experienceBar.UpdateExperienceSlider(nextValue, TO_LEVEL_UP());
        xpSound.Play();
    }

    public void UpdateLevelText(int previousValue, int nextValue)
    {
        experienceBar.SetLevelText(nextValue);
    }

    public void AddExperience(float amount)
    {
        experience.Value += amount;
    }
    [ClientRpc]
    private void LevelUpClientRpc()
    {
        foreach (var xd in listOfPlayers)
        {
            xd.Value.GetComponent<Character>().LevelUp();
        }
    }
    [ClientRpc]
    private void SetActiveClientRpc(NetworkObjectReference objectReference)
    {
        objectReference.TryGet(out NetworkObject Object);
        Object.gameObject.SetActive(true);
    }

    [ClientRpc]
    private void WaitingForOtherPlayersUIClientRpc()
    {
        waitingForOtherPlayersUI.ChangeVisibility();
    }

    [ServerRpc(RequireOwnership = false)]
    private void WaitingForOtherPlayersUIServerRpc()
    {
        WaitingForOtherPlayersUIClientRpc();
    }
}
