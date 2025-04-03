using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Siege : MonoBehaviour
{
    public List<Unit> Besiegers = new();
    public Building Defender;
    public TextMeshPro SiegeText;

    public void StartSiege(GameObject besieger, GameObject defender)
    {
        AddBesieger(besieger);
        Defender = defender.GetComponent<Building>();
        Defender.Besiege(this);
        SiegeText.color = besieger.GetComponent<Unit>().Owner.KingdomColor;
        StartCoroutine(SiegeCycle());
    }
    public void AddBesieger(GameObject besieger)
    {
        if (besieger.TryGetComponent<Soldier>(out Soldier soldier))
        {
            if (soldier.Commander != null)
            {
                AddHeroBesieger(soldier.Commander.GetComponent<Hero>());
            }
            else
            {
                Besiegers.Add(soldier);
                soldier.AddToSiege(this);
            }
        }
        else
        {
            AddHeroBesieger(besieger.GetComponent<Hero>());
        }
    }
    public void AddHeroBesieger(Hero hero)
    {
        Besiegers.Add(hero);
        hero.AddToSiege(this);
        if (hero.Army.Count > 0)
        {
            foreach (GameObject soldier in hero.Army)
            {
                if (!Besiegers.Contains(soldier.GetComponent<Soldier>()))
                {
                    Besiegers.Add(soldier.GetComponent<Soldier>());
                    soldier.GetComponent<Soldier>().AddToSiege(this);
                }    
            }
        }
    }
    private IEnumerator SiegeCycle()
    {
        int MaxHealth = 20;
        int CurHealth = 20;
        while (true)
        {
            foreach (Unit unit in Besiegers)
            {
                CurHealth -= unit.Attack();
            }
            SiegeText.text = CurHealth.ToString() + "/" + MaxHealth.ToString();
            if (CurHealth <= 0)
            {
                break;
            }
            yield return new WaitForSeconds(3);
        }
        EndSiege();
    }

    public void EndSiege()
    {
        Defender.OnConquer(Besiegers[0].Owner);
        Defender.LiftSiege();
        if (Besiegers.OfType<Hero>().Any())
        {
            Besiegers.OfType<Hero>().First().AddExperience(50);
        }
        foreach (Unit unit in Besiegers)
        {
            unit.RemoveFromSiege();
        }
        Destroy(gameObject);
    }
}
