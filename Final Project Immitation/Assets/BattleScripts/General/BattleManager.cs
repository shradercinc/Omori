using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class BattleManager : MonoBehaviour
{
    Transform friendGroup;
    Transform foeGroup;

    public List<BattleCharacter> friends = new List<BattleCharacter>();
    public List<BattleCharacter> foes = new List<BattleCharacter>();
    public List<BattleCharacter> toast = new List<BattleCharacter>();
    List<BattleCharacter> SpeedQueue = new List<BattleCharacter>();

    public TMP_Text battleLog;
    public TMP_Text energyLog;

    public int energy = 3;
    bool battleContinue = true;

    void Awake()
    {
        friendGroup = GameObject.Find("FriendGroup").GetComponent<Transform>();
        foeGroup = GameObject.Find("FoeGroup").GetComponent<Transform>();

        foreach (Transform child in friendGroup)
        {
            friends.Add(child.GetComponent<BattleCharacter>());
        }
        foreach (Transform child in foeGroup)
        {
            foes.Add(child.GetComponent<BattleCharacter>());
        }
        StartCoroutine(NewRound());
    }

    public void AddEnergy()
    {
        energy++;
        if (energy > 10)
            energy = 10;
    }

    public void AddText(string x, bool reset)
    {
        if (reset)
            battleLog.text = x;
        else
        {
            battleLog.text += "\n";
            battleLog.text += x;
        }
    }

    public void AddText(string x)
    {
        AddText(x, false);
    }

    IEnumerator NewRound()
    {
        friends = friends.OrderBy(o => o.order).ToList();
        energyLog.text = "Energy: " + energy;

        for (int i = 0; i < friends.Count; i++)
        {
            SpeedQueue.Add(friends[i]);
            yield return friends[i].ChooseSkill();
        }
        for (int i = 0; i < foes.Count; i++)
        {
            SpeedQueue.Add(foes[i]);
            foes[i].ChooseRandomSkill();
        }
        StartCoroutine(PlayRound());
    }

    IEnumerator PlayRound()
    {
        while (battleContinue && SpeedQueue.Count > 0)
        {
            SpeedQueue = SpeedQueue.OrderByDescending(o => o.currSpeed).ToList();
            BattleCharacter nextInLine = SpeedQueue[0];
            SpeedQueue.Remove(nextInLine);

            if (!nextInLine.toast)
            {
                if (nextInLine.weapon != null)
                    nextInLine.weapon.StartOfTurn();
                nextInLine.UseMove();

                if (energy >= 3 && nextInLine.friend && nextInLine.currMove == BattleCharacter.Move.ATTACK)
                    yield return FollowUp(nextInLine);
                else
                    yield return new WaitForSeconds(1.5f);
            }

            energyLog.text = "Energy: " + energy;
            battleContinue = (friends.Count > 0 && foes.Count > 0);
        }

        if (battleContinue)
            StartCoroutine(NewRound());
        else
        {
            AddText("The battle is over.", true);
        }
    }

    IEnumerator FollowUp(BattleCharacter user)
    {
        yield return new WaitForSeconds(1.5f);
        bool decision = true;
        Skills userSkills = user.userSkills;

        AddText("Follow up?", true);
        AddText("1: Skip");

        bool skillOne = (energy >= userSkills.energyCost[0] && !userSkills.followUpRequire[0].toast);
        if (skillOne)
            AddText("2: " + userSkills.skillNames[5]);
        else
            AddText("Cannot " + userSkills.skillNames[5]);

        bool skillTwo = (energy >= userSkills.energyCost[1] && !userSkills.followUpRequire[1].toast);
        if (skillTwo)
            AddText("3: " + userSkills.skillNames[6]);
        else
            AddText("Cannot " + userSkills.skillNames[6]);

        bool skillThree = (energy >= userSkills.energyCost[2] &&
        (friends.Count == 4 || !userSkills.followUpRequire[2].toast));
        if (skillThree)
            AddText("4: " + userSkills.skillNames[7]);
        else
            AddText("Cannot " + userSkills.skillNames[7]);

        while (decision)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2) && skillOne)
            {
                user.userSkills.FollowUpOne();
                decision = false;
                yield return new WaitForSeconds(1.5f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && skillTwo)
            {
                user.userSkills.FollowUpTwo();
                decision = false;
                yield return new WaitForSeconds(1.5f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && skillThree)
            {
                user.userSkills.FollowUpThree();
                decision = false;
                yield return new WaitForSeconds(1.5f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                decision = false;
            }
            yield return null;
        }
    }

    public void ReturnToList(BattleCharacter target)
    {
        friends.Add(target);
        toast.Remove(target);
    }

    public void RemoveFromList(BattleCharacter target)
    {
        if (target.friend)
        {
            friends.Remove(target);
            toast.Add(target);
        }
        else
        {
            foes.Remove(target);
        }
    }

    public List<BattleCharacter> GetAllTargets()
    {
        List<BattleCharacter> allTargets = new List<BattleCharacter>();

        for (int i = 0; i < friends.Count; i++)
            allTargets.Add(friends[i]);
        for (int i = 0; i < foes.Count; i++)
            allTargets.Add(foes[i]);

        return allTargets;
    }
}