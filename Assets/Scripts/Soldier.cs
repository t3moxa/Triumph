using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Soldier : Unit
{
    public GameObject Commander { get; private set; }
    public void LateUpdate()
    {
        if (IsSelected)
        {
            UpdateSoldierPanel();
        }
    }
    public void UpdateSoldierPanel()
    {
        PlayerController.SoldierInfoPanel.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "Atk: " + Atk.ToString() + "|Def: " + Def.ToString();
        PlayerController.SoldierInfoPanel.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = CurrentHealth.ToString() + "/" + MaxHealth.ToString();
    }
    public void FindAndJoinHero()
    {
        List<Hero> targets = FindObjectsByType<Hero>(FindObjectsSortMode.None).Where(x => x.Owner == Owner).ToList(); // Находим все объекты типа Target

        if (targets.Count == 0)
        {
            return;
        }

        GameObject nearestTarget = null;
        float nearestDistance = Mathf.Infinity; // Инициализируем с бесконечным расстоянием

        foreach (Hero target in targets)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position); // Вычисляем расстояние до целевого объекта

            if (distance < nearestDistance) // Если найденный объект ближе, обновляем ближайший объект
            {
                nearestDistance = distance;
                nearestTarget = target.gameObject;
            }
        }

        if (nearestDistance < 2f)
        {
            JoinHero(nearestTarget);
            nearestTarget.GetComponent<Hero>().AddSoldier(gameObject);
        }
    }
    public void JoinHero(GameObject hero)
    {
        Commander = hero;
        Atk = Commander.GetComponent<Hero>().Atk;
        Def = Commander.GetComponent<Hero>().Def;

        CurrentMorale += Commander.GetComponent<Hero>().MaxMorale - MaxMorale;
        if (CurrentMorale <= 0)
            CurrentMorale = 1;
        MaxMorale = Commander.GetComponent<Hero>().MaxMorale;

        CurrentHealth += Commander.GetComponent<Hero>().MaxHealth - MaxHealth;
        if (CurrentHealth <= 0)
            CurrentHealth = 1;
        MaxHealth = Commander.GetComponent<Hero>().MaxHealth;
        OnDeselect();
    }
    public void LeaveHero()
    {
        Commander = null;
        Atk = Owner.BaseSoldierAtk;
        Def = Owner.BaseSoldierDef;

        CurrentMorale += Owner.BaseSoldierMorale - MaxMorale;
        if (CurrentMorale <= 0)
            CurrentMorale = 1;
        MaxMorale = Owner.BaseSoldierMorale;

        CurrentHealth += Owner.BaseSoldierHealth - MaxHealth;
        if (CurrentHealth <= 0)
            CurrentHealth = 1;
        MaxHealth = Owner.BaseSoldierHealth;
    }
    public override void ShowInfoPanel()
    {
        IsSelected = true;
        if (Owner == PlayerController.PlayerKingdom)
        {
            PlayerController.SoldierInfoPanel.transform.GetChild(4).gameObject.SetActive(true);
        }
        PlayerController.SoldierInfoPanel.SetActive(true);
    }

    public override void HideAllPanels()
    {
        IsSelected = false;
        PlayerController.SoldierInfoPanel.transform.GetChild(4).gameObject.SetActive(false);
        PlayerController.SoldierInfoPanel.SetActive(false);
    }


    public void FollowHero(Vector2 targetHeroPosition)
    {
        if (Vector2.Distance(transform.position, targetHeroPosition) > 2f)
        {
            targetPosition = targetHeroPosition + new Vector2(Random.Range(-2, 2), Random.Range(-2, 2));
        }
        else
        {
            Vector2 offset = Commander.transform.position - transform.position;
            targetPosition = targetHeroPosition - offset;
        }
        AnimateMove();
    }

    public override void MoveUnit()
    {
        if (!IsLocked)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = mousePosition;
        }
        AnimateMove();
    }

    public override void Die()
    {
        if (Commander != null)
        {
            Commander.GetComponent<Hero>().RemoveSoldier(gameObject);
            LeaveHero();
        }
        if (Battle != null)
        {
            Battle.AddExperienceToBattle(Owner, MaxHealth * 10);
            Battle.RemoveUnit(Owner, this);
            RemoveFromBattle();
        }
        HideAllPanels();
        Owner.RemoveUnit(this);
        Destroy(gameObject);
    }
    public override (GameObject, Selectable) OnSelect()
    {
        Debug.Log(targetPosition);
        if (Commander != null)
        {
            return Commander.GetComponent<Hero>().OnSelect();
        }
        else
        {
            selectionCircle.SetActive(true); // Показываем круг выделения
            selectionCircle.transform.position = transform.position; // Устанавливаем позицию круга
            ShowInfoPanel();
            return (gameObject, this);
        }
    }
    public void ForceMove(Vector2 position)
    {
        targetPosition = position;
        AnimateMove();
    }
}
