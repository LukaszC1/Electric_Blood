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
    public event EventHandler OnLocalPlayerReadyChanged;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private bool isLocalPlayerReady;
    private bool isLocalGamePaused = false;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);

    private Dictionary<ulong, bool> playerPausedDictionary;
    [SerializeField] private GameObject playerPrefab;


    private void Awake()
    {
        Instance = this;

        playerPausedDictionary = new Dictionary<ulong, bool>();

        xpBankGem = Instantiate(xpBankGemPrefab);
        xpBankGem.SetActive(false);
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
    }
    public override void OnDestroy()
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

                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    if (UnityEngine.Random.value <= 0.1)
                    {
                        listOfPlayers.TryGetValue((ulong)UnityEngine.Random.Range(0, listOfPlayers.Count), out Transform player);
                        GameObject breakable = Instantiate(breakableObject);
                        Vector3 position = GenerateRandomPosition(player.transform.position);
                        while (CheckForCollision(position))
                            position = GenerateRandomPosition(player.transform.position);
                        breakable.transform.position = position;
                    }
                    timer = 1;
                }

                if (xpBank > 100 && !xpBankGem.activeSelf)
                {
                    listOfPlayers.TryGetValue((ulong)UnityEngine.Random.Range(0, listOfPlayers.Count), out Transform player);
                    xpBankGem.SetActive(true);
                    Vector3 position = GenerateRandomPosition(player.transform.position);
                    while (CheckForCollision(position))
                        position = GenerateRandomPosition(player.transform.position);
                    xpBankGem.transform.position = position;
                }
                
                break;
            case State.GameOver:
                break;
        }
    }

    public void FixedUpdate()
    {
        CheckLevelUp();
        RefreshListOfPlayers(); //THIS IS ONLY UNTIL LOBBY WORKS
    }

    private void OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    public void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
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
                // This player is paused
                isGamePaused.Value = true;
                return;
            }
        }

        // All players are unpaused
        isGamePaused.Value = false;
    }

    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state.Value == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }

    public bool IsWaitingToStart()
    {
        return state.Value == State.WaitingToStart;
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

        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject playerSpawned = Instantiate(playerPrefab);
            playerSpawned.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        TogglePauseGame(); //pause game when client disconnects
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
            killCounter.text = ":" + killCount.Value.ToString();
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
        var players = FindObjectsOfType<Character>();
        listOfPlayers.Clear();
        foreach (var player in players)
            listOfPlayers.TryAdd(player.playerID.Value, player.transform);
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
}
