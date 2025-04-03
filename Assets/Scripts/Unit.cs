using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Unit : Selectable
{
    public int Atk = 1;
    public int Def = 1;
    public int CurrentHealth = 10;
    public int MaxHealth = 10;
    public int CurrentMorale = 10;
    public int MaxMorale = 10;
    public float moveSpeed = 3f; // Скорость движения персонажа
    public int AttackRangeLow = 2;
    public int AttackRangeHigh = 4;
    protected Rigidbody2D rb;
    protected Vector2 targetPosition; // Целевая позиция для перемещения
    public GameObject selectionCircle; // Ссылка на объект выделения
    public GameObject banner;
    public GameObject battleCollider;
    public bool IsLocked { protected set; get; } = false;
    public Battle Battle { protected set; get; }
    public Siege Siege { protected set; get; }
    //public GameObject UnitInfoPanel;
    //public TMP_Text AtkText;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (Owner == PlayerController.PlayerKingdom)
        {
            //targetPosition = transform.position;
        }
    }

    void FixedUpdate()
    {
        // Перемещение персонажа к целевой позиции
        if (Vector2.Distance(rb.position, targetPosition) > 0.1f)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
        }
    }

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
        //Debug.Log("Панели спрятаны");
    }

    public override void OnOrder()
    {
        MoveUnit();
    }

    public void AddToBattle(Battle battle)
    {
        Battle = battle;
        Lock();
        ForceStop();
    }
    public void RemoveFromBattle()
    {
        Battle = null;
        Unlock();
    }
    public void AddToSiege(Siege siege)
    {
        Siege = siege;
        Lock();
        ForceStop();
    }
    public void RemoveFromSiege()
    {
        Siege = null;
        Unlock();
    }
    public int Attack()
    {
        return Random.Range(AttackRangeLow, AttackRangeHigh+1) + Atk;
    }
    public void Defend(int attack)
    {
        CurrentHealth -= attack - Def;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    public void OnSpawn(KingdomData owner)
    {
        Owner = owner;
        Owner.AddUnit(this);
        ChangeBanner();
        ChangeSelection();
        targetPosition = transform.position;
        battleCollider.SetActive(true);
    }
    public void ChangeBanner()
    {
        banner.GetComponent<SpriteRenderer>().color = Owner.KingdomColor;
    }
    public void ChangeSelection()
    {
        selectionCircle.GetComponent<SpriteRenderer>().color = Owner.KingdomColor;
    }
    public void Lock()
    {
        IsLocked = true;
    }
    public void Unlock()
    {
        IsLocked = false;
    }
    public void ForceStop()
    {
        targetPosition = transform.position;
    }
    public void AnimateMove()
    {
        if(targetPosition.x < transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }
    public abstract void Die();
    public abstract void MoveUnit();
    public abstract void ShowInfoPanel();
    public abstract void HideAllPanels();
}