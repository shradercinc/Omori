using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingPan : Weapon
{
    public override void AffectUser()
    {
        user = gameObject.GetComponent<BattleCharacter>();
        user.startingHealth += 10;
        user.startingAttack += 6;
    }
    public override void StartOfTurn()
    {
    }
}