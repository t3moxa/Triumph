using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class KingdomData : MonoBehaviour
{
    public int KingdomID = 0;
    public List<Unit> Units = new();
    public Capital Capital;
    public Color KingdomColor;
    public int Gold = 0;
    public int BaseSoldierAtk = 0;
    public int BaseSoldierDef = 0;
    public int BaseSoldierHealth = 10;
    public int BaseSoldierMorale = 10;
    public void AddGold(int gold)
    {
        Gold += gold;
    }
    public void SubstractGold(int gold)
    {
        Gold -= gold;
    }
    public void SpawnKingdom(GameObject city)
    {
        var capital = Instantiate(PlayerController.CapitalPrefab, city.transform.position, Quaternion.identity);
        capital.GetComponent<Capital>().OnSpawn(this);
        capital.GetComponent<Capital>().SpawnStartingArmy();
        Capital = capital.GetComponent<Capital>();
        Destroy(city);
    }
    public void SpawnAIKingdom(GameObject city)
    {
        var capital = Instantiate(PlayerController.CapitalPrefab, city.transform.position, Quaternion.identity);
        capital.GetComponent<Capital>().OnSpawn(this);
        Capital = capital.GetComponent<Capital>();
        Destroy(city);
    }
    public void AddUnit(Unit unit)
    {
        Units.Add(unit);
    }
    public void RemoveUnit(Unit unit)
    {
        Units.Remove(unit);
    }
    public void OnStart()
    {
        Capital.OnStart();
    }
}
