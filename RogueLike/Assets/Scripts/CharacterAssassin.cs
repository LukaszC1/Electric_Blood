using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAssassin : Character
{
    public override void LevelUpBonus()
    {
        damageMultiplier += 0.01f;
    }
}
