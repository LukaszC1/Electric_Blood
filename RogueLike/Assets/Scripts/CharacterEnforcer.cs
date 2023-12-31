using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character enforcer class inherits from Character.
/// </summary>
public class CharacterEnforcer : Character
{
    public override void LevelUpBonus()
    {
        projectileSpeedMultiplier.Value += 0.02f;
    }
}
