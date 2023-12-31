using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character assassin class inherits from Character.
/// </summary>
public class CharacterAssassin : Character
{
    public override void LevelUpBonus()
    {
        damageMultiplier.Value += 0.01f;
    }
}
