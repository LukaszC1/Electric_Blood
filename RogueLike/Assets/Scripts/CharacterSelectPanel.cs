using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPanel : NetworkBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private Image displayedCharacter;

    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterDescription;
    [SerializeField] private TextMeshProUGUI maxHp;
    [SerializeField] private TextMeshProUGUI armor;
    [SerializeField] private TextMeshProUGUI hpRegen;
    [SerializeField] private TextMeshProUGUI damageMultiplier;
    [SerializeField] private TextMeshProUGUI areaMultiplier;
    [SerializeField] private TextMeshProUGUI projectileSpeedMultiplier;
    [SerializeField] private TextMeshProUGUI magnetSize;
    [SerializeField] private TextMeshProUGUI cooldownMultiplier;
    [SerializeField] private TextMeshProUGUI amountBonus;

    private int _selectedOption = 0;
    private List<ScriptableObject> availableCharacters;

    private void Awake()
    { 

        nextButton.onClick.AddListener(() =>
        {
            _selectedOption++;
            if (_selectedOption >= availableCharacters.Count)
            {
                _selectedOption = 0;
            }

            UpdateDisplayedCharacter();
        });

        previousButton.onClick.AddListener(() =>
        {
            _selectedOption--;
            if (_selectedOption < 0)
            {
                _selectedOption = availableCharacters.Count - 1;
            }

            UpdateDisplayedCharacter();
        });

        selectButton.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });

        closeButton.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });
    }

    private void Start()
    {
        availableCharacters = ElectricBloodMultiplayer.Instance.availableCharacters;
        UpdateDisplayedCharacter();
    }

    public void UpdateDisplayedCharacter() //load the data
    {
        CharacterData characterData = (CharacterData)availableCharacters[_selectedOption];

        displayedCharacter.sprite = characterData.characterSprite;
        characterName.text = characterData.characterName;
        characterDescription.text = characterData.characterDescription;
        maxHp.text = "Max hp: " + characterData.characterPrefab.GetComponent<Character>().maxHp.Value.ToString();
        armor.text = "Armor: " + characterData.characterPrefab.GetComponent<Character>().armor.Value.ToString();
        hpRegen.text = "Hp regen: " + characterData.characterPrefab.GetComponent<Character>().hpRegen.Value.ToString();
        damageMultiplier.text = "Damage multiplier: " + characterData.characterPrefab.GetComponent<Character>().damageMultiplier.Value.ToString();
        areaMultiplier.text = "Area multiplier: " + characterData.characterPrefab.GetComponent<Character>().areaMultiplier.Value.ToString();
        projectileSpeedMultiplier.text = "Projectile speed multiplier: " + characterData.characterPrefab.GetComponent<Character>().projectileSpeedMultiplier.Value.ToString();
        magnetSize.text = "Magnet size: " + characterData.characterPrefab.GetComponent<Character>().magnetSize.Value.ToString();
        cooldownMultiplier.text = "Cooldown multiplier: " + characterData.characterPrefab.GetComponent<Character>().cooldownMultiplier.Value.ToString();
        amountBonus.text = "Amount bonus: " + characterData.characterPrefab.GetComponent<Character>().amountBonus.Value.ToString();
    }
}
