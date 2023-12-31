using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LevelData is a ScriptableObject that contains the data for a level.
/// </summary>
[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    /// <summary>
    /// The name of the level.
    /// </summary>
    public string levelName;

    /// <summary>
    /// The sprite to display for the level.
    /// </summary>
    public Sprite levelSprite;

    /// <summary>
    /// The chunks that make up the level.
    /// </summary>
    public List<GameObject> chunks;
}
