using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public KingdomData AIKingdom;
    public void OnStart()
    {
        AIKingdom.Capital.SpawnAIStartingArmy(FindClosestUncapturedBuildings(AIKingdom.Capital.transform.position, 3)[Random.Range(0, 3)].transform.position);
        StartCoroutine(BuildingCoroutine());
    }
    public IEnumerator BuildingCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(BuildAppropriateUnit(FindClosestUncapturedBuildings(AIKingdom.Capital.transform.position, 3)[Random.Range(0, 3)].transform.position));
        }
    }
    public List<Building> FindClosestUncapturedBuildings(Vector2 position, int numOfBuidlings)
    {
        List<Building> noncapturedBuildings = FindObjectsByType<Building>(FindObjectsSortMode.None).Where(x => x.Owner != AIKingdom).ToList();
        List<(Building, float)> distances = new();
        foreach (Building b in noncapturedBuildings)
        {
            distances.Add((b, Vector2.Distance(position, b.transform.position)));
        }
        distances.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        List<Building> outList = new(); 
        for (int i = 0; i < numOfBuidlings; i++)
        {
            outList.Add(distances[i].Item1);
        }
        return outList;
    }

    public int BuildAppropriateUnit(Vector2 position)
    {
        if (AIKingdom.Gold < 6)
        {
            AIKingdom.SubstractGold(5);
            AIKingdom.Capital.AISpawnSoldier(position);
            Debug.Log("Spawning soldier");
            return 8;
        }
        else if (AIKingdom.Gold < 11)
        {
            AIKingdom.SubstractGold(10);
            AIKingdom.Capital.AISpawnSquad(position);
            Debug.Log("Spawning squad");
            return 13;
        }
        else
        {
            AIKingdom.SubstractGold(15);
            AIKingdom.Capital.AISpawnBigSquad(position);
            Debug.Log("Spawning big squad");
            return 18;
        }
    }
}
