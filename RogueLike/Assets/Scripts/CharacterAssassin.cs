using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAssassin : Character
{
    public override void LevelUpBonus()
    {
        damageMultiplier.Value += 0.01f;
    }
}
