using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweetheartWeapon : Weapon
{
    //Each turn, if the selected Friend doesn't match Sweetheart's emotion, they become toast.

    public override void AffectUser()
    {
        user = FindObjectOfType<SweetheartSkills>().GetComponent<BattleCharacter>();
    }
    public override IEnumerator StartOfTurn()
    {
        BattleCharacter target = user.nextTarget;

        if (target != null && !target.toast && target.currEmote != user.currEmote)
        {
            manager.AddText(target.name + " has the wrong emotion.", true);
            yield return target.TakeDamage(200);
        }
    }
}
