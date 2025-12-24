using UnityEngine;

public class CorpseSummonAbility : SummonAbilityBase
{
    private void Awake()
    {
        summonCount = 5;
        summonRadius = 2f;
    }

    public override void Activate()
    {
        Summon();
    }
}
