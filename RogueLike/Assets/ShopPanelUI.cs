using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI availableCoins;

    [SerializeField] private List<GameObject> plusMinusButtons;
    [SerializeField] private List<TextMeshProUGUI> characterStats;
    [SerializeField] private List<TextMeshProUGUI> costTexts;

    private void Awake()
    {
        //load the data from a file 

        for(int i = 0; i < plusMinusButtons.Count; i+=2)
        {
            plusMinusButtons[i]
            .GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    //plus button

                });
            plusMinusButtons[i+1]
           .GetComponent<Button>()
               .onClick.AddListener(() =>
               {
                   //minus button

               });
        }

        closeButton.onClick.AddListener(() =>
        {
            //serialize the data

            //close the panel
            gameObject.SetActive(false);
        });
    }

    private int ReturnIndex()
    {
        for (int i = 0; i < plusMinusButtons.Count; i++)
        {
            if (plusMinusButtons[i].gameObject == this.gameObject)
            {
                return i;
            }
        }
        return -1;
    }
}
