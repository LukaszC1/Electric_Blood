using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    [SerializeField] GameObject upgradePanel;
    PauseManager pauseManager;

    [SerializeField] List<UpgradeButton> upgradeButtons;
    public void Awake()
    {
        pauseManager = GetComponent<PauseManager>();
    }
    public void ClosePanel()
    {
        HideButtons();

        upgradePanel.SetActive(false);
        pauseManager.UnPauseGame();
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
    public void OpenPanel(List<UpgradeData> upgradeData)
    {
        Clean();
        upgradePanel.SetActive(true);
        pauseManager.PauseGame();


            for (int i = 0; i < upgradeData.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(true);
            upgradeButtons[i].Set(upgradeData[i]);
        }
    }

    public void Clean()
    {
        for(int i = 0; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].Clean();
        }
    }


    public void Upgrade(int pressedButtonID)
    {
        GameManager.Instance.playerTransform.GetComponent<Character>().Upgrade(pressedButtonID);
        ClosePanel();
    }
}
