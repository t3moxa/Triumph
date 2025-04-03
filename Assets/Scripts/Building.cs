using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Building : Selectable
{
    public GameObject selectionCircle; // Ссылка на объект выделения
    public GameObject banner;
    protected String Name;
    List<String> Names = new() { "Fandalin", "Flamekeep", "Wroat", "Neverwinter", "Fairhaven", "Korth", "Sigil", "Baldur's Gate", "Waterdeep", "Bilgewater", "Piltover", "Brin-Shander", "Osgiliath", "Menzoberranzan", "Suzail", "New Antioch", "Babylon", "Jerusalem", "City X", "Enoch"};
    public StateTile StateTile { private set; get; }
    public Siege Siege { private set; get; }
    Coroutine GoldGenerationCoroutine;
    public override (GameObject, Selectable) OnSelect()
    {
        selectionCircle.SetActive(true); // Показываем круг выделения
        selectionCircle.transform.position = transform.position; // Устанавливаем позицию круга
        ShowBuildingInfoPanel();
        return (gameObject, this);
    }

    public override void OnDeselect()
    {
        selectionCircle.SetActive(false); // Скрываем круг выделения
        HideAllPanels();
    }
    public void OnSpawn(KingdomData owner)
    {
        Owner = owner;
        Name = Names[Random.Range(0, Names.Count)];
        ChangeBanner();
        ChangeSelection();
    }
    public void OnStart()
    {
        GoldGenerationCoroutine = StartCoroutine(GoldGeneration());
    }
    public void ChangeBanner()
    {
        banner.GetComponent<SpriteRenderer>().color = Owner.KingdomColor;
    }
    public void ChangeSelection()
    {
        selectionCircle.GetComponent<SpriteRenderer>().color = Owner.KingdomColor;
    }
    public override void OnOrder()
    {
        
    }
    public void SetStateTile(StateTile st)
    {
        StateTile = st;
    }
    public void Besiege(Siege siege)
    {
        StopAllCoroutines();
        Siege = siege;
    }
    public void LiftSiege()
    {
        Siege = null;
        GoldGenerationCoroutine = StartCoroutine(GoldGeneration());
    }
    private IEnumerator GoldGeneration()
    {
        while (true)
        {
            Owner.AddGold(1);
            yield return new WaitForSeconds(2);
        }
    }
    public abstract void ShowBuildingInfoPanel();
    public abstract void OnConquer(KingdomData Conqueror);
    public abstract void HideAllPanels();
}
