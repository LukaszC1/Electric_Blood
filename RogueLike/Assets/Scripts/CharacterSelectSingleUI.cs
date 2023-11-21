using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int _characterIndex;
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _selected;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            ElectricBloodMultiplayer.Instance.ChangePlayerCharacter(_characterIndex);
        });
    }

    private void Start()
    {
        ElectricBloodMultiplayer.Instance.OnPlayerDataNetworkListChanged += ElectricBlood_OnPlayerDataNetworkListChanged;
        //todo some effect after we select this character

        UpdateIsSelected();
    }

    private void ElectricBlood_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (ElectricBloodMultiplayer.Instance.GetPlayerData().characterIndex == _characterIndex)
        {
            _selected.SetActive(true);
        }
        else
        {
            _selected.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        ElectricBloodMultiplayer.Instance.OnPlayerDataNetworkListChanged -= ElectricBlood_OnPlayerDataNetworkListChanged;
    }
}
