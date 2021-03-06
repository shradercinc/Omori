using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour
{
    public BattleCharacter user;
    public BattleManager manager;

    public List<string> skillNames = new List<string>();
    public List<int> juiceCost = new List<int>();
    public List<int> energyCost = new List<int>();
    public List<BattleCharacter> followUpRequire = new List<BattleCharacter>();
    public List<string> skillDescription = new List<string>();

    public enum Target { NONE, FRIEND, FOE, ANYONE, ALLFRIENDS, ALLFOES };
    public List<Target> skillTargets = new List<Target>();

    private void Awake()
    {
        manager = FindObjectOfType<BattleManager>().GetComponent<BattleManager>();
    }

    private BattleCharacter RandomTarget(int n)
    {
        switch (skillTargets[n])
        {
            case Target.FRIEND:
                return manager.friends[Random.Range(0, manager.friends.Count)];
            case Target.FOE:
                return manager.foes[Random.Range(0, manager.foes.Count)];
            case Target.ANYONE:
                List<BattleCharacter> allTargets = manager.GetAllTargets();
                return allTargets[Random.Range(0, allTargets.Count)];
            default:
                return null;
        }

    }

    public bool RollAccuracy(float value)
    {
        float n = (Random.Range(0.0f, 1f));
        bool result = (n <= value);
        if (!result)
            manager.AddText(gameObject.name + "'s attack misses.");
        return result;
    }

    public int RollCritical(float value)
    {
        float n = (Random.Range(0.0f, 1f));
        bool result = (n <= value);
        if (result)
        {
            manager.AddText("The attack hits right in the heart!");
            return 2;
        }
        return 1;
    }

    public virtual IEnumerator BasicAttack(BattleCharacter target)
    {
        target = RedirectTarget(target, 0);
        manager.AddText(user.name + " attacks " + target.name + ".", true);

        if (RollAccuracy(user.currAccuracy))
        {
            int critical = RollCritical(user.currLuck);
            int damage = (int)(critical * IsEffective(target) * (user.currAttack - target.currDefense));
            yield return target.TakeDamage(damage);
        }
    }

    public float IsEffective(BattleCharacter target)
    {
        float answer = 1.0f;
        if (user.currEmote == BattleCharacter.Emotion.HAPPY)
            switch (target.currEmote)
            {
                case (BattleCharacter.Emotion.ANGRY):
                    answer = 1.25f;
                    break;
                case (BattleCharacter.Emotion.ENRAGED):
                    answer = 1.25f;
                    break;
                case (BattleCharacter.Emotion.SAD):
                    answer = 0.75f;
                    break;
                case (BattleCharacter.Emotion.DEPRESSED):
                    answer = 0.75f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }
        else if (user.currEmote == BattleCharacter.Emotion.ECSTATIC)
            switch (target.currEmote)
            {
                case (BattleCharacter.Emotion.ANGRY):
                    answer = 1.5f;
                    break;
                case (BattleCharacter.Emotion.ENRAGED):
                    answer = 1.5f;
                    break;
                case (BattleCharacter.Emotion.SAD):
                    answer = 0.5f;
                    break;
                case (BattleCharacter.Emotion.DEPRESSED):
                    answer = 0.5f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }
        else if (user.currEmote == BattleCharacter.Emotion.ANGRY)
            switch (target.currEmote)
            {
                case (BattleCharacter.Emotion.SAD):
                    answer = 1.25f;
                    break;
                case (BattleCharacter.Emotion.DEPRESSED):
                    answer = 1.25f;
                    break;
                case (BattleCharacter.Emotion.HAPPY):
                    answer = 0.75f;
                    break;
                case (BattleCharacter.Emotion.ECSTATIC):
                    answer = 0.75f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }
        else if (user.currEmote == BattleCharacter.Emotion.ENRAGED)
            switch (target.currEmote)
            {
                case (BattleCharacter.Emotion.SAD):
                    answer = 1.5f;
                    break;
                case (BattleCharacter.Emotion.DEPRESSED):
                    answer = 1.5f;
                    break;
                case (BattleCharacter.Emotion.HAPPY):
                    answer = 0.5f;
                    break;
                case (BattleCharacter.Emotion.ECSTATIC):
                    answer = 0.5f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }
        else if (user.currEmote == BattleCharacter.Emotion.SAD)
            switch (target.currEmote)
            {
                case (BattleCharacter.Emotion.HAPPY):
                    answer = 1.25f;
                    break;
                case (BattleCharacter.Emotion.ECSTATIC):
                    answer = 1.25f;
                    break;
                case (BattleCharacter.Emotion.ANGRY):
                    answer = 0.75f;
                    break;
                case (BattleCharacter.Emotion.ENRAGED):
                    answer = 0.75f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }
        else if (user.currEmote == BattleCharacter.Emotion.DEPRESSED)
            switch (target.currEmote)
            {
                case (BattleCharacter.Emotion.HAPPY):
                    answer = 1.5f;
                    break;
                case (BattleCharacter.Emotion.ECSTATIC):
                    answer = 1.5f;
                    break;
                case (BattleCharacter.Emotion.ANGRY):
                    answer = 0.5f;
                    break;
                case (BattleCharacter.Emotion.ENRAGED):
                    answer = 0.5f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }

        if (answer < 1)
            manager.AddText("It was a dull attack...");
        else if (answer > 1)
            manager.AddText("It was a moving attack!");

        return answer;
    }

    public BattleCharacter RedirectTarget(BattleCharacter target, int n)
    {
        if (target == null || target.toast)
        {
            Debug.Log("Redirecting attack.");
            List<BattleCharacter> possibleTargets = new List<BattleCharacter>();

            if (skillTargets[n] == Target.ANYONE)
                possibleTargets = manager.GetAllTargets();
            else if (skillTargets[n] == Target.FRIEND)
                possibleTargets = manager.friends;
            else if (skillTargets[n] == Target.FOE)
                possibleTargets = manager.foes;

            return possibleTargets[Random.Range(0, possibleTargets.Count-1)];
        }
        else
            return target;
    }

    public virtual void SetStartingStats()
    {
    }
    public virtual IEnumerator UseSkillOne(BattleCharacter target)
    {
        yield return null;
    }
    public virtual IEnumerator UseSkillTwo(BattleCharacter target)
    {
        yield return UseSkillOne(target);
    }
    public virtual IEnumerator UseSkillThree(BattleCharacter target)
    {
        yield return UseSkillOne(target);
    }
    public virtual IEnumerator UseSkillFour(BattleCharacter target)
    {
        yield return UseSkillOne(target);
    }
    public virtual IEnumerator FollowUpOne()
    {
        yield return null;
    }
    public virtual IEnumerator FollowUpTwo()
    {
        yield return null;
    }
    public virtual IEnumerator FollowUpThree()
    {
        yield return null;
    }
    public virtual BattleCharacter ChooseTarget(int n)
    {
        return RandomTarget(n);
    }
}