using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUi : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        text.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
