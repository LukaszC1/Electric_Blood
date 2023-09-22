using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEnforcer : Character
{
    public override void LevelUpBonus()
    {
        projectileSpeedMultiplier += 0.02f;
    }
}
