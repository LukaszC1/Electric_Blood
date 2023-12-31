using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Class attached to HPBarBase.
/// </summary>
public class StatusBar : NetworkBehaviour
{
    /// <summary>
    /// The transform of the status bar.
    /// </summary>
    [SerializeField] Transform bar;

    /// <summary>
    /// Setter for the status bar.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="max"></param>
    public void SetState(int current, int max)
    {
        float state = (float)current;
        state /= max;
        if (state < 0f) { state = 0f; }
        bar.transform.localScale = new Vector3(state, 1f, 1f);
    }
}
