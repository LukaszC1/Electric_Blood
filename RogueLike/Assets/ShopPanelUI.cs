using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button resetButton;

    [SerializeField] private TextMeshProUGUI availableCoins;

    [SerializeField] private List<TextMeshProUGUI> characterStats;
    [SerializeField] private List<TextMeshProUGUI> costTexts;

    private void Awake()
    {
        //load the data from a file 
        PersistentUpgrades.Instance.Load();

        closeButton.onClick.AddListener(() =>
        {
            //close the panel
            gameObject.SetActive(false);
        });
    }

    public void OnClickPlus(int i)
    {
        var currentUpgradeLevel = characterStats[i];
        var splitString = currentUpgradeLevel.text.Split("/");

        int available = new();
        int current = new();

        if(splitString.Length > 0)
        {
            current = int.Parse(splitString[0].Trim());
            available = int.Parse(splitString[1].Trim());
        }
        
        if (current < available)
        { 
            current++;
            characterStats[i].text = current + "/" + available;
        }

        //update costs
    }

    public void OnClickMinus(int i)
    {
        var currentUpgradeLevel = characterStats[i];
        var splitString = currentUpgradeLevel.text.Split("/");

        int available = new();
        int current = new();

        if (splitString.Length > 0)
        {
            current = int.Parse(splitString[0].Trim());
            available = int.Parse(splitString[1].Trim());
        }

        if (current > 0)
        {
            current--;
            characterStats[i].text = current + "/" + available;
        }

        //update costs
    }
    private void ResetUpgrades()
    {
        //reset current upgrades and add the equivalent of the coins spent to total coins
    }
}
