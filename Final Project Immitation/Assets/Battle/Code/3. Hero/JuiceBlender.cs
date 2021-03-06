using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceBlender : Weapon
{
    public override void AffectUser()
    {
        user = FindObjectOfType<HeroSkills>().GetComponent<BattleCharacter>();
        description = "Hero starts with less Health and Juice. Everyone regains 10 Juice each turn.";
        user.startingHealth -= 20;
        user.startingJuice -= 30;
    }

    public override IEnumerator StartOfTurn()
    {
        for (int i = 0; i < manager.friends.Count; i++)
        {
            manager.AddText("The Juice Blender has finished blending.", true);
            yield return manager.friends[i].TakeHealing(0, 10);
        }
    }
}
