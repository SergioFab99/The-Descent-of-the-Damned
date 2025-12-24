using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DemonSummonAbility : SummonAbilityBase
{
    private void Awake()
    {
        summonCount = 1;
    }

    public override void Activate()
    {
        Summon();
    }
}
