using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Battle : Selectable
{
    private Dictionary<KingdomData, List<Unit>> BattleSides = new();
    private Dictionary<KingdomData, int> BattleExperience = new();
    public GameObject selectionCircle;
    private bool SelectedBattle = false;

    public override (GameObject, Selectable) OnSelect()
    {
        selectionCircle.SetActive(true); // Показываем круг выделения
        selectionCircle.transform.position = transform.position; // Устанавливаем позицию круга
        ShowInfoPanel();
        return (gameObject, this);
    }

    public override void OnDeselect()
    {
        selectionCircle.SetActive(false); // Скрываем круг выделения
        HideAllPanels();
        SelectedBattle = false;
        //Debug.Log("Панели спрятаны");
    }

    public override void OnOrder()
    {
    }
    private void ShowInfoPanel()
    {
        PlayerController.BattleInfoPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = BattleSides.ElementAt(0).Key.KingdomColor;
        PlayerController.BattleInfoPanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().color = BattleSides.ElementAt(1).Key.KingdomColor;
        PlayerController.BattleInfoPanel.SetActive(true);
        SelectedBattle = true;
    }
    public void HideAllPanels()
    {
        PlayerController.BattleInfoPanel.SetActive(false);
        SelectedBattle = false;
    }
    private void LateUpdate()
    {
        if (BattleContinues())
        {
            float x = 0;
            float y = 0;
            int unitCount = 0;
            foreach (List<Unit> list in BattleSides.Values)
            {
                foreach (Unit unit in list)
                {
                    x += unit.gameObject.transform.position.x;
                    y += unit.gameObject.transform.position.y;
                    unitCount++;
                }
            }
            transform.position = new Vector3(x / unitCount, y / unitCount, -2);
        }
        
        if (SelectedBattle)
        {
            float h1 = 0;
            float h2 = 0;
            foreach (Unit unit in BattleSides.ElementAt(0).Value)
            {
                h1 += unit.CurrentHealth;
            }
            foreach (Unit unit in BattleSides.ElementAt(1).Value)
            {
                h2 += unit.CurrentHealth;
            }
            PlayerController.BattleInfoPanel.transform.GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = BattleSides.ElementAt(0).Value.Count.ToString();
            PlayerController.BattleInfoPanel.transform.GetChild(0).GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = h1.ToString();
            PlayerController.BattleInfoPanel.transform.GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = BattleSides.ElementAt(1).Value.Count.ToString();
            PlayerController.BattleInfoPanel.transform.GetChild(1).GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = h2.ToString();
        }
    }
    public void StartBattle(GameObject side1, GameObject side2)
    {
        AddUnit(side1);
        AddUnit(side2);
        StartCoroutine(BattleCycle());
    }

    public void AddUnit(GameObject unit)
    {
        if (unit.TryGetComponent<Soldier>(out Soldier soldier))
        {
            if (BattleSides.ContainsKey(soldier.Owner))
            {
                if (!BattleSides[soldier.Owner].Contains(soldier))
                {
                    BattleSides[soldier.Owner].Add(soldier);
                    soldier.AddToBattle(this);
                    if ((soldier.Commander != null) && !BattleSides[soldier.Owner].Contains(soldier.Commander.GetComponent<Hero>()))
                    {
                        AddUnit(soldier.Commander);
                    }
                }
            }
            else
            {
                BattleSides[soldier.Owner] = new List<Unit> { soldier };
                BattleExperience[soldier.Owner] = 0;
                soldier.AddToBattle(this);
                if ((soldier.Commander != null) && !BattleSides[soldier.Owner].Contains(soldier.Commander.GetComponent<Hero>()))
                {
                    AddUnit(soldier.Commander);
                }
            }
        }
        else if (unit.TryGetComponent<Hero>(out Hero hero))
        {
            if (BattleSides.ContainsKey(hero.Owner))
            {
                if (BattleSides[hero.Owner].OfType<Hero>().Any())
                {
                    return;//only one hero in battle!!!
                }
                else if (!BattleSides[hero.Owner].Contains(hero))
                {
                    BattleSides[hero.Owner].Add(hero);
                    hero.AddToBattle(this);
                    AddArmy(hero);
                }
            }
            else
            {
                BattleSides[hero.Owner] = new List<Unit> { hero };
                BattleExperience[hero.Owner] = 0;
                hero.AddToBattle(this);
                AddArmy(hero);
            }
        }
    }

    private void AddArmy(Hero hero)
    {
        foreach (GameObject soldier in hero.Army)
        {
            AddUnit(soldier);
        }
    }

    private IEnumerator BattleCycle()
    {
        while (true)
        {
            var Attacks1 = CalculateAttacks(BattleSides.ElementAt(0).Value);
            var Attacks2 = CalculateAttacks(BattleSides.ElementAt(1).Value);
            AllocateAttacks(Attacks1, BattleSides.ElementAt(1).Key);
            AllocateAttacks(Attacks2, BattleSides.ElementAt(0).Key);
            if (!BattleContinues())
            {
                break;
            }
            yield return new WaitForSeconds(3);
        }
        EndBattle();
    }

    private bool BattleContinues()
    {
        return BattleSides.ElementAt(0).Value.Any() && BattleSides.ElementAt(1).Value.Any();
    }

    private void EndBattle()
    {
        KingdomData victor = BattleSides.FirstOrDefault(tuple => tuple.Value.Count > 0).Key;
        if (victor != null)
        {
            KingdomData loser = BattleSides.First(tuple => tuple.Key != victor).Key;
            if (BattleSides[victor].OfType<Hero>().Any())
            {
                BattleSides[victor].OfType<Hero>().First().AddExperience(BattleExperience[loser]);
            }
            foreach (Unit unit in BattleSides[victor])
            {
                unit.RemoveFromBattle();
            }
        }
        OnDeselect();
        Destroy(gameObject);
    }
    private List<int> CalculateAttacks(List<Unit> unitList)
    {
        List<int> Attacks = new();
        foreach(Unit unit in unitList)
        {
            Attacks.Add(unit.Attack());
        }
        return Attacks;
    }

    private void AllocateAttacks(List<int> attacks, KingdomData defender)
    {
        List<Unit> defenders = new(BattleSides[defender]);
        if (defenders.Count > 1)
        {
            if (defenders.OfType<Hero>().Any())
            {
                defenders.Remove(defenders.OfType<Hero>().First());
            }
        }
        foreach (int attack in attacks)
        {
            if (defenders.Count == 0)
            {
                return;
            }
            defenders[Random.Range(0, defenders.Count)].Defend(attack);
        }
    }

    public void RemoveUnit(KingdomData owner, Unit unit)
    {
        BattleSides[owner].Remove(unit);
    }

    public void AddExperienceToBattle(KingdomData Owner, int XP)
    {
        BattleExperience[Owner] += XP;
    }
}
