using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public string levelName;
    public Sprite levelSprite;

    public List<GameObject> chunks;
}
