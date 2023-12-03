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
    [SerializeField] private GameObject _circleContainer;
    [SerializeField] private GameObject _circleTemplate;
    private List<ScriptableObject> _availableLevels;
    private List<GameObject> _displayedCirclesList;
    private int _currentLevelIndex = 0;

    private void Start()
    {
        _availableLevels = ElectricBloodMultiplayer.Instance.availableLevels;

        for (int i = 0; i < _availableLevels.Count; i++)
        {
            GameObject circleGameObject = Instantiate(_circleTemplate, _circleContainer.transform);
            circleGameObject.gameObject.SetActive(true);
            _displayedCirclesList.Add(gameObject);
        }
    }
    private void Awake()
    {
        _closeBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        _nextBtn.onClick.AddListener(() =>
        {
            _currentLevelIndex++;
            if (_currentLevelIndex >= _availableLevels.Count)
            {
                _currentLevelIndex = 0;
                UpdateDisplayedLevel();
            }
        });
        _previousBtn.onClick.AddListener(() =>
        {
            _currentLevelIndex--;
            if (_currentLevelIndex < 0)
            {
                _currentLevelIndex = _availableLevels.Count - 1;
                UpdateDisplayedLevel();
            }
        });
    }

    private void UpdateDisplayedLevel()
    {
        var circle = _displayedCirclesList[_currentLevelIndex].GetComponent<Image>();
        circle.color = Color.green;

        if(_currentLevelIndex == 0)
        {
            circle = _displayedCirclesList[_displayedCirclesList.Count-1].GetComponent<Image>();
            circle.color = Color.white;
        }
        else if (_currentLevelIndex == _availableLevels.Count - 1)
        {
            circle = _displayedCirclesList[0].GetComponent<Image>();
            circle.color = Color.white;
        }
        else
        {
            circle = _displayedCirclesList[_currentLevelIndex-1].GetComponent<Image>();
            circle.color = Color.white;
        }

        var selectedLevel = _availableLevels[_currentLevelIndex] as LevelData;
        _displayedImage.sprite = selectedLevel.levelSprite;
    }
}
