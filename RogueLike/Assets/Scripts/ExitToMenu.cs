using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script handling the exit to main menu.
/// </summary>
public class ExitToMenu : NetworkBehaviour
{
    [SerializeField] private Button mainMenuButton;
    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            Cleanup();
            NetworkManager.Singleton.Shutdown();

            PersistentUpgrades.Instance.Save(); 
            Destroy(PersistentUpgrades.Instance.gameObject);

            Loader.Load(Loader.Scene.MainMenu);
        });
    }

    private void Cleanup()
    {
        if (!IsServer) return;
        var enemiesManager = FindObjectOfType<EnemiesManager>();
        var enemies = enemiesManager.enemyList;

        if (enemies == null) return;

        foreach (var gameObject in enemies)
        {
            Destroy(gameObject);
        }

        FindObjectOfType<WeaponManager>().weapons.ForEach(w => Destroy(w.gameObject));
        FindObjectsOfType<DamagePopup>().ToList().ForEach(d => Destroy(d.gameObject));
    }
}
