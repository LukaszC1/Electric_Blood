using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character data ScriptableObject containing all of the information about a character. (name, sprite etc.)
/// </summary>
[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public string characterDescription;
    public Sprite characterSprite;
    public GameObject characterPrefab;
}
