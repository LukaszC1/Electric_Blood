using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Experience bar class.
/// </summary>
public class ExperienceBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMPro.TextMeshProUGUI levelText;

    /// <summary>
    /// Updates the experience slider.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    public void UpdateExperienceSlider (float current, float target)
    {
        slider.maxValue = target;
        slider.value = current;
    }

    /// <summary>
    /// Sets the level text.
    /// </summary>
    /// <param name="level"></param>
    public void SetLevelText(int level)
    {
        levelText.text = "Level: " + level.ToString();
    }
}
