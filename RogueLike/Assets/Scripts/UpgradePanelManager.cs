using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{

    [SerializeField] GameObject upgradePanel;
    [SerializeField] List<UpgradeButton> upgradeButtons;
    private ulong currentPlayer;

    public static event EventHandler OnPauseAction;
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

    public void Clean()
    {
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].Clean();
        }
    }


    public void Upgrade(int pressedButtonID)
    {
        Transform player;
        GameManager.Instance.listOfPlayers.TryGetValue(currentPlayer, out player);
        player.GetComponent<Character>().Upgrade(pressedButtonID);
        ClosePanel();
    }
}
