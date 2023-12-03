using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionPanel : MonoBehaviour
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _nextBtn;
    [SerializeField] private Button _previousBtn;
    [SerializeField] private Image _displayedImage;
    [SerializeField] private Transform _circleContainer;
    [SerializeField] private Transform _circleTemplate;
    private List<ScriptableObject> _availableLevels;
    private List<Transform> _displayedCirclesList;
    private int _currentLevelIndex = 0;

    private void Start()
    {
        _displayedCirclesList = new();
        _availableLevels = ElectricBloodMultiplayer.Instance.availableLevels;

        for (int i = 0; i < _availableLevels.Count; i++)
        {
            Transform circle = Instantiate(_circleTemplate, _circleContainer);
            circle.gameObject.SetActive(true);
            _displayedCirclesList.Add(circle);
        }
        UpdateDisplayedLevel();
    }
    private void Awake()
    {
        _closeBtn.onClick.AddListener(() =>
        {
            ElectricBloodMultiplayer.Instance.ChangeSelectedLevelIndex(_currentLevelIndex);
            gameObject.SetActive(false);
        });
        _nextBtn.onClick.AddListener(() =>
        {
            _currentLevelIndex++;
            if (_currentLevelIndex >= _availableLevels.Count)
            {
                _currentLevelIndex = 0;               
            }
            UpdateDisplayedLevel();
        });
        _previousBtn.onClick.AddListener(() =>
        {
            _currentLevelIndex--;
            if (_currentLevelIndex < 0)
            {
                _currentLevelIndex = _availableLevels.Count - 1;           
            }
            UpdateDisplayedLevel();
        });
    }

    private void UpdateDisplayedLevel()
    {
        var circle = _displayedCirclesList[_currentLevelIndex].GetComponent<Image>();
        circle.color = Color.magenta;

       for(int i = 0; i < _displayedCirclesList.Count; i++)
        {
            if (i == _currentLevelIndex)
                continue;
            else
            _displayedCirclesList[i].GetComponent<Image>().color = Color.white;
        }

        var selectedLevel = _availableLevels[_currentLevelIndex] as LevelData;
        _displayedImage.sprite = selectedLevel.levelSprite;
    }
}
