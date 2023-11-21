using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public string characterDescription;
    public Sprite characterSprite;
    public GameObject characterPrefab;
}
