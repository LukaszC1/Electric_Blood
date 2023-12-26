using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI availableCoins;
    private void Awake()
    {
        //load the data from a file 



        closeButton.onClick.AddListener(() =>
        {
            //serialize the data

            //close the panel
            gameObject.SetActive(false);
        });
    }
}
