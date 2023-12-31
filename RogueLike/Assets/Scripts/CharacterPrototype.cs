using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character prototype class inherits from Character.
/// </summary>
public class CharacterPrototype : Character
{
    public override void LevelUpBonus()
    {
        areaMultiplier.Value += 0.01f;
    }
}
