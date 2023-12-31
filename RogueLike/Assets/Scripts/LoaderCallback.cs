using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Callback for the Loader class for the first update.
/// </summary>
public class LoaderCallback : MonoBehaviour
{
    private bool isFirstUpdate = true;
    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;

            Loader.LoaderCallback();
        }
    }
}
