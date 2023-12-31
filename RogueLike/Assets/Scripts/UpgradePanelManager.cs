using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script attached to the UpgradePanel GameObject.
/// </summary>
public class UpgradePanelManager : MonoBehaviour
{
    /// <summary>
    /// UpgradePanel game object.
    /// </summary>
    [SerializeField] GameObject upgradePanel;

    /// <summary>
    /// List of upgrade buttons.
    /// </summary>
    [SerializeField] List<UpgradeButton> upgradeButtons;

    private ulong currentPlayer;

    /// <summary>
    /// Event invoked when the upgrade panel is closed to unpause the game.
    /// </summary>
    public static event EventHandler OnPauseAction;

    /// <summary>
    /// Methods closing the panel invoking the OnPauseAction event.
    /// </summary>
    public void ClosePanel()
    {
        HideButtons();

        upgradePanel.SetActive(false);
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void HideButtons()
    {
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        HideButtons();

    }

    /// <summary>
    /// Method which opens the panel.
    /// </summary>
    /// <param name="upgradeData"></param>
    /// <param name="currentPlayer"></param>
    public void OpenPanel(List<UpgradeData> upgradeData, ulong currentPlayer)
    {
        Clean();
        upgradePanel.SetActive(true);
        OnPauseAction?.Invoke(this, EventArgs.Empty);

        this.currentPlayer = currentPlayer;

        for (int i = 0; i < upgradeData.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(true);
            upgradeButtons[i].Set(upgradeData[i]);
        }
    }

    /// <summary>
    /// Upgrade buttons cleanup.
    /// </summary>
    public void Clean()
    {
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].Clean();
        }
    }

    /// <summary>
    /// Method which upgrades the character.
    /// </summary>
    /// <param name="pressedButtonID"></param>
    public void Upgrade(int pressedButtonID)
    {
        Transform player;
        GameManager.Instance.listOfPlayers.TryGetValue(currentPlayer, out player);
        player.GetComponent<Character>().Upgrade(pressedButtonID);
        ClosePanel();
    }
}
