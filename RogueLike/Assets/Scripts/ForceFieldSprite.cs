using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldSprite : MonoBehaviour
{
    public void UpdateTransform(Vector2 size)
    {
        transform.localScale = size;
    }
}
