using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class StageTime : NetworkBehaviour
{
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
        time.Value += Time.deltaTime;

    }
    public void UpdateTime(float previousValue, float nextValue)
    {
        timer.UpdateTime(time.Value);
    }
}
