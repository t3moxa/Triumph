using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Hero : Unit
{
    public int Level = 1;
    public int Experience = 0;
    public int ExperienceNextLevelThreshold = 50;
    public List<GameObject> Army { private set; get; } = new();
    public void LateUpdate()
    {
        if (IsSelected)
        {
            UpdateHeroPanel();
        }
    }
    public void UpdateHeroPanel()
    {
        PlayerController.HeroInfoPanel.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "Atk: " + Atk.ToString() + "|Def: " + Def.ToString();
        PlayerController.HeroInfoPanel.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = CurrentHealth.ToString() + "/" + MaxHealth.ToString();
        PlayerController.HeroInfoPanel.transform.GetChild(5).GetComponent<TMPro.TextMeshProUGUI>().text = Army.Count.ToString();
        PlayerController.HeroInfoPanel.transform.GetChild(7).GetComponent<TMPro.TextMeshProUGUI>().text = Experience.ToString() + "/" + ExperienceNextLevelThreshold.ToString() + " Lvl: " + Level.ToString();
    }
    public void AddSoldier(GameObject soldier)
    {
        Army.Add(soldier);
    }

    public void RemoveSoldier(GameObject soldier)
    {
        Army.Remove(soldier);
    }

    public void ReleaseSoldiers()
    {
        foreach (GameObject soldier in Army)
        {
            soldier.GetComponent<Soldier>().LeaveHero();
        }
        Army.Clear();
    }
    new public void RemoveFromBattle()
    {
        Battle = null;
        Unlock();
        ForceMove(transform.position);
    }
    new public void RemoveFromSiege()
    {
        Siege = null;
        Unlock();
        ForceMove(transform.position);
    }
    public override void MoveUnit()
    {
        if (!IsLocked)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = mousePosition;
            if (Army.Any())
            {
                foreach (GameObject soldier in Army)
                {
                    soldier.GetComponent<Soldier>().FollowHero(targetPosition);
                }
            }
        }
        AnimateMove();
    }
    public void ForceMove(Vector2 position)
    {
        targetPosition = position;
        if (Army.Any())
        {
            foreach (GameObject soldier in Army)
            {
                soldier.GetComponent<Soldier>().FollowHero(position);
            }
        }
        AnimateMove();
    }
    public override void Die()
    {
        if (Army.Any())
        {
            ReleaseSoldiers();
        }
        if (Battle != null)
        {
            Battle.AddExperienceToBattle(Owner, MaxHealth * 10 + Level * 100);
            Battle.RemoveUnit(Owner, this);
            RemoveFromBattle();
        }
        HideAllPanels();
        Owner.RemoveUnit(this);
        Destroy(gameObject);
    }
    public override void ShowInfoPanel()
    {
        IsSelected = true;
        if (Owner == PlayerController.PlayerKingdom)
        {
            PlayerController.HeroInfoPanel.transform.GetChild(8).gameObject.SetActive(true);
        }
        PlayerController.HeroInfoPanel.SetActive(true);
    }

    public override void HideAllPanels()
    {
        IsSelected = false;
        PlayerController.HeroInfoPanel.transform.GetChild(8).gameObject.SetActive(false);
        PlayerController.HeroInfoPanel.SetActive(false);
    }

    public void AddExperience(int XP)
    {
        Experience += XP;
        if (Experience >= ExperienceNextLevelThreshold)
        {
            LevelUp();
            int leftoverXP = Experience - ExperienceNextLevelThreshold;
            Experience = 0;
            ExperienceNextLevelThreshold *= 2;
            AddExperience(leftoverXP);
        }
    }

    public void LevelUp()
    {
        switch (Random.Range(0, 4))
        {
            case 0: Atk += 1;
                break;
            case 1: Def += 2;
                break;
            case 2: MaxHealth += 2; CurrentHealth += 2;
                break;
            //case 3: MaxMorale += 2; CurrentMorale += 2;
            //    break;
            default: Atk += 1;
                break;
        }
        Level += 1;
    }
}
