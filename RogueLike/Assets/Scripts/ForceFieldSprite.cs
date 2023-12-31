using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ForceFieldSprite is a script that updates the local scale of the force field sprite.
/// </summary>
public class ForceFieldSprite : MonoBehaviour
{
    /// <summary>
    /// Method that updates the local scale of the force field sprite.
    /// </summary>
    /// <param name="size"></param>
    public void UpdateTransform(Vector2 size)
    {
        transform.localScale = size;
    }
}
