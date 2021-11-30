using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : Weapon
{
    //Increase Aubrey's Attack and Defense. There is a 20% chance that Aubrey will miss her turn.

    public override void AffectUser()
    {
        user = FindObjectOfType<AubreySkills>().GetComponent<BattleCharacter>();
        user.startingAttack += 7;
        user.startingDefense += 7;
    }
    public override IEnumerator StartOfTurn()
    {
        if ((float)(Random.Range(0.0f, 1f)) < 0.8)
            user.paralyze = true;
        yield return null;
    }
}