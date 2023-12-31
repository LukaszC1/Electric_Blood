using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Class handling the displayed time of the stage.
/// </summary>
public class StageTime : NetworkBehaviour
{
    /// <summary>
    /// NetworkVariable holding the time of the stage.
    /// </summary>
    public NetworkVariable<float> time = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    TimerUi timer;

    private void Awake()
    {
        timer = FindObjectOfType<TimerUi>();
    }

    private void Start()
    {
        time.OnValueChanged += UpdateTime;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if(time != null)
        time.Value += Time.deltaTime;

    }

    /// <summary>
    /// Method which updates the time of the stage.
    /// </summary>
    /// <param name="previousValue"></param>
    /// <param name="nextValue"></param>
    public void UpdateTime(float previousValue, float nextValue)
    {
        timer.UpdateTime(time.Value);
    }
}
