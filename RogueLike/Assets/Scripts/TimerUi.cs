using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class is responsible for updating the counter on the GameScene.
/// </summary>
public class TimerUi : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Method that updates the counter on the GameScene.
    /// </summary>
    /// <param name="time"></param>
    public void UpdateTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        text.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
