using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AubreySkills : Skills
{
    public override void SetStartingStats()
    {
        //Attack:
        skillNames.Add("Attack");
        juiceCost.Add(0);
        skillTargets.Add(Target.FOE);
        skillDescription.Add("");

        //Skill 1:
        skillNames.Add("Mood Breaker");
        juiceCost.Add(15);
        skillTargets.Add(Target.FOE);
        skillDescription.Add("Make a Foe Sad and reduce their Defense. Then deal damage to them.");

        //Skill 2:
        skillNames.Add("Home Run");
        juiceCost.Add(15);
        skillTargets.Add(Target.FOE);
        skillDescription.Add("Deal damage to a Foe. If Aubrey is Happy or Ecstatic, she first raises her Accuracy.");

        //Skill 3:
        skillNames.Add("Cheer");
        juiceCost.Add(5);
        skillTargets.Add(Target.FRIEND);
        skillDescription.Add("A Friend becomes Happy. Gain 2 Energy.");

        //Skill 4:
        skillNames.Add("Big Swing");
        juiceCost.Add(25);
        skillTargets.Add(Target.FOE);
        skillDescription.Add("Deal a lot of damage to a Foe. Aubrey takes recoil damage.");

        //Follow Up 1:
        skillNames.Add("Look at Omori");
        energyCost.Add(3);
        followUpRequire.Add(GameObject.Find("Omori").GetComponent<BattleCharacter>());
        skillDescription.Add("Aubrey becomes Sad. Then deal damage to a Foe.");

        //Follow Up 2:
        skillNames.Add("Look at Kel");
        energyCost.Add(3);
        followUpRequire.Add(GameObject.Find("Kel").GetComponent<BattleCharacter>());
        skillDescription.Add("Both Aubrey and Kel become Angry. Raise both of their Attacks.");

        //Follow Up 3:
        skillNames.Add("Look at Hero");
        energyCost.Add(3);
        followUpRequire.Add(GameObject.Find("Hero").GetComponent<BattleCharacter>());
        skillDescription.Add("Aubrey gains more health and defense, and becomes Happy.");

        user = gameObject.GetComponent<BattleCharacter>();
        user.friend = true;
        user.order = 1;

        user.startingHealth = 120;
        user.startingJuice = 60;
        user.startingAttack = 40;
        user.startingDefense = 10;
        user.startingSpeed = 8;
        user.startingLuck = 0.03f;
        user.startingAccuracy = 0.95f;
    }

    public override IEnumerator UseSkillOne(BattleCharacter target)
    {
        bool check = juiceCost[1] <= user.currJuice;
        yield return user.DrainJuice(juiceCost[1]);

        if (check)
        {
            target = RedirectTarget(target, 1);
            manager.AddText("Aubrey breaks " + target.name + "'s spirit.", true);

            yield return target.NewEmotion(BattleCharacter.Emotion.SAD);
            target.defenseStat -= 0.2f;
            yield return target.ResetStats();
            manager.AddText(target.name + "'s Defense decreases.");

            if (RollAccuracy(user.currAccuracy))
            {
                int critical = RollCritical(user.currLuck);
                int damage = (int)(critical * IsEffective(target) * (1.25 * user.currAttack - target.currDefense));
                yield return target.TakeDamage(damage);
            }
        }
    }
    public override IEnumerator UseSkillTwo(BattleCharacter target)
    {
        bool check = juiceCost[2] <= user.currJuice;
        yield return user.DrainJuice(juiceCost[2]);

        if (check)
        {
            target = RedirectTarget(target, 2);
            manager.AddText("Aubrey hits a home run.", true);
            if (user.currEmote == BattleCharacter.Emotion.HAPPY || target.currEmote == BattleCharacter.Emotion.ECSTATIC)
            {
                user.accuracyStat += 0.15f;
                yield return user.ResetStats();
                manager.AddText(user.name + "'s Accuracy increases.");
            }
            if (RollAccuracy(user.currAccuracy))
            {
                int critical = RollCritical(user.currLuck);
                int damage = (int)(critical * IsEffective(target) * (1.25 * user.currAttack - target.currDefense));
                yield return target.TakeDamage(damage);
            }
        }
    }
    public override IEnumerator UseSkillThree(BattleCharacter target)
    {
        bool check = juiceCost[3] <= user.currJuice;
        yield return user.DrainJuice(juiceCost[3]);

        if (check)
        {
            target = RedirectTarget(target, 3);
            manager.AddText("Aubrey cheers on " + target.name + ".", true);
            yield return target.NewEmotion(BattleCharacter.Emotion.HAPPY);
            yield return manager.AddEnergy(2);
        }
    }
    public override IEnumerator UseSkillFour(BattleCharacter target)
    {
        bool check = juiceCost[4] <= user.currJuice;
        yield return user.DrainJuice(juiceCost[4]);

        if (check)
        {
            if (RollAccuracy(user.currAccuracy))
            {
                target = RedirectTarget(target, 4);
                manager.AddText("Aubrey swings as hard as she can.", true);
                int critical = RollCritical(user.currLuck);
                int damage = (int)(critical * IsEffective(target) * (1.75 * user.currAttack - target.currDefense));
                yield return target.TakeDamage(damage);
                yield return user.TakeDamage(damage/3);
            }
        }
    }
    public override IEnumerator FollowUpOne()
    {
        yield return manager.AddEnergy(-energyCost[0]);
        BattleCharacter target = RedirectTarget(user.nextTarget, 0);
        manager.AddText("Omori doesn't notice Aubrey, so she attacks again out of loneliness.", true);
        yield return user.NewEmotion(BattleCharacter.Emotion.SAD);

        int critical = RollCritical(user.currLuck);
        int damage = (int)(critical * IsEffective(target) * (user.currAttack - target.currDefense));
        yield return target.TakeDamage(damage);
    }
    public override IEnumerator FollowUpTwo()
    {
        yield return manager.AddEnergy(-energyCost[1]);
        BattleCharacter kel = followUpRequire[1];
        manager.AddText("Kel eggs on Aubrey.", true);

        user.attackStat += 0.15f;
        manager.AddText(user.name + "'s Attack increases.");
        yield return user.NewEmotion(BattleCharacter.Emotion.ANGRY);

        kel.attackStat += 0.15f;
        manager.AddText(kel.name + "'s Attack increases.");
        yield return kel.NewEmotion(BattleCharacter.Emotion.ANGRY);
    }
    public override IEnumerator FollowUpThree()
    {
        yield return manager.AddEnergy(-energyCost[2]);
        manager.AddText("Hero cheers on Aubrey.", true);
        user.defenseStat += 0.15f;
        manager.AddText(user.name + "'s defense increases.");

        int healing = (int) (user.startingHealth * 0.5f);
        yield return user.TakeHealing(healing, 0);
        yield return user.NewEmotion(BattleCharacter.Emotion.HAPPY);
    }
}
