using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KelSkills : Skills
{
    public override void SetStartingStats()
    {
        //Attack:
        skillNames.Add("Attack");
        juiceCost.Add(0);
        skillTargets.Add(Target.FOE);
        skillDescription.Add("");

        //Skill 1:
        skillNames.Add("Snowball Fight");
        juiceCost.Add(15);
        skillTargets.Add(Target.FOE);
        skillDescription.Add("Make a Foe Happy and reduce their Luck. Then deal damage to them.");

        //Skill 2:
        skillNames.Add("Headbutt");
        juiceCost.Add(15);
        skillTargets.Add(Target.FOE);
        skillDescription.Add("Deal damage to a Foe. If Kel is Angry or Enraged, he first raises his Luck.");

        //Skill 3:
        skillNames.Add("Annoy");
        juiceCost.Add(5);
        skillTargets.Add(Target.FRIEND);
        skillDescription.Add("A Friend becomes Angry. They regain some Juice.");

        //Skill 4:
        skillNames.Add("Rebound");
        juiceCost.Add(10);
        skillTargets.Add(Target.ALLFOES);
        skillDescription.Add("Deals damage to all Foes.");

        //Follow Up 1:
        skillNames.Add("Pass to Omori");
        energyCost.Add(3);
        followUpRequire.Add(GameObject.Find("Omori").GetComponent<BattleCharacter>());
        skillDescription.Add("Omori becomes Happy. Deal damage to a Foe.");

        //Follow Up 2:
        skillNames.Add("Pass to Aubrey");
        energyCost.Add(3);
        followUpRequire.Add(GameObject.Find("Aubrey").GetComponent<BattleCharacter>());
        skillDescription.Add("Aubrey deals a lot of damage to a Foe.");

        //Follow Up 3:
        skillNames.Add("Pass to Hero");
        energyCost.Add(3);
        followUpRequire.Add(GameObject.Find("Hero").GetComponent<BattleCharacter>());
        skillDescription.Add("Kel deals damage to all Foes.");

        user = gameObject.GetComponent<BattleCharacter>();
        user.friend = true;
        user.order = 2;

        user.startingHealth = 90;
        user.startingJuice = 60;
        user.startingAttack = 30;
        user.startingDefense = 8;
        user.startingSpeed = 16;
        user.startingLuck = 0.05f;
        user.startingAccuracy = 0.95f;
    }

    public override IEnumerator UseSkillOne(BattleCharacter target)
    {
        bool check = juiceCost[1] <= user.currJuice;
        yield return user.DrainJuice(juiceCost[1]);

        if (check)
        {
            target = RedirectTarget(target, 1);
            manager.AddText("Kel starts a snowball fight against " + target.name + ".", true);

            yield return target.NewEmotion(BattleCharacter.Emotion.HAPPY);
            target.luckStat -= 0.15f;
            yield return target.ResetStats();
            manager.AddText(target.name + "'s Luck decreases.");

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
            manager.AddText("Kel headbutts " + target.name + ".", true);

            if (user.currEmote == BattleCharacter.Emotion.ANGRY || target.currEmote == BattleCharacter.Emotion.ENRAGED)
            {
                user.luckStat += 0.15f;
                yield return user.ResetStats();
                manager.AddText(user.name + "'s Luck increases.");
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
            manager.AddText("Kel annoys " + target.name + ".", true);
            target.TakeHealing(0, 20);
            yield return target.NewEmotion(BattleCharacter.Emotion.ANGRY);
        }
    }

    public override IEnumerator UseSkillFour(BattleCharacter target)
    {
        bool check = juiceCost[4] <= user.currJuice;
        yield return user.DrainJuice(juiceCost[4]);

        if (check)
        {
            List<BattleCharacter> allEnemies = manager.foes;
            for (int i = 0; i < allEnemies.Count; i++)
            {
                manager.AddText("Kel's ball bounces everywhere.", true);
                target = allEnemies[i];

                if (RollAccuracy(user.currAccuracy))
                {
                    int critical = RollCritical(user.currLuck);
                    int damage = (int)(critical * IsEffective(target) * (user.currAttack - target.currDefense));
                    yield return target.TakeDamage(damage);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public override IEnumerator FollowUpOne()
    {
        BattleCharacter omori = followUpRequire[0];
        yield return manager.AddEnergy(-energyCost[0]);
        manager.AddText("Kel passes the ball to Omori, who then throws it.", true);
        yield return omori.NewEmotion(BattleCharacter.Emotion.HAPPY);

        BattleCharacter target = RedirectTarget(user.nextTarget, 0);
        int critical = RollCritical(omori.currLuck);
        int damage = (int)(critical * omori.userSkills.IsEffective(target) * (user.currAttack + omori.currAttack - target.currDefense));
        yield return target.TakeDamage(damage);
    }
    public override IEnumerator FollowUpTwo()
    {
        BattleCharacter aubrey = followUpRequire[1];
        BattleCharacter target = RedirectTarget(user.nextTarget, 0);
        yield return manager.AddEnergy(-energyCost[1]);
        manager.AddText("Kel passes the ball to Aubrey, who knocks it out of the park.", true);

        int critical = RollCritical(aubrey.currLuck);
        int damage = (int)(critical * aubrey.userSkills.IsEffective(target) * (1.25f * user.currAttack + 1.25f * aubrey.currAttack - target.currDefense));
        yield return target.TakeDamage(damage);
    }
    public override IEnumerator FollowUpThree()
    {
        yield return manager.AddEnergy(-energyCost[2]);
        BattleCharacter hero = followUpRequire[2];
        List<BattleCharacter> allEnemies = manager.foes;

        for (int i = 0; i < allEnemies.Count; i++)
        {
            manager.AddText("Kel passes the ball to Hero, who throws it high up to let Kel do a slam dunk.", true);
            BattleCharacter target = allEnemies[i];

            int critical = RollCritical(hero.currLuck);
            int damage = (int)(critical * 1.25f * IsEffective(target) * (user.currAttack));

            yield return target.TakeDamage(damage);
            yield return new WaitForSeconds(1);
        }
    }
}
