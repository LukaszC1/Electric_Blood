using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPrototype : Character
{
    public override void LevelUpBonus()
    {
        areaMultiplier += 0.01f;
    }
}
