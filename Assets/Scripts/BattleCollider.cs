using Pathfinding;
using System;
using UnityEngine;

public class BattleCollider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject AssignedUnit;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, -1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Unit>(out Unit unit))
        {
            if (other.gameObject.GetComponent<Unit>().Owner != transform.parent.gameObject.GetComponent<Unit>().Owner)
            {
                if ((transform.parent.gameObject.GetComponent<Unit>().Battle == null) && (other.gameObject.GetComponent<Unit>().Battle == null))
                {
                    StartBattle(transform.parent.gameObject, other.gameObject);
                }
                else if (transform.parent.gameObject.GetComponent<Unit>().Battle == other.gameObject.GetComponent<Unit>().Battle)
                {
                    return;
                }
                else if (transform.parent.gameObject.GetComponent<Unit>().Battle == null)
                {
                    other.gameObject.GetComponent<Unit>().Battle.AddUnit(transform.parent.gameObject);
                }
                else if (other.gameObject.GetComponent<Unit>().Battle == null)
                {
                    transform.parent.gameObject.GetComponent<Unit>().Battle.AddUnit(other.gameObject);
                }
            }
            //Debug.Log("Soldier spotted: " + other.gameObject.name);
        }
        else if (other.gameObject.TryGetComponent<Building>(out Building building))
        {
            if (other.gameObject.GetComponent<Building>().Owner != transform.parent.gameObject.GetComponent<Unit>().Owner)
            {
                if ((other.gameObject.GetComponent<Building>().Siege == null) && (transform.parent.gameObject.GetComponent<Unit>().Battle == null))
                {
                    StartSiege(transform.parent.gameObject, other.gameObject);
                }
            }
        }
    }

    private void StartBattle(GameObject side1, GameObject side2)
    {
        Vector3 spawnPosition = new ((side1.transform.position.x + side2.transform.position.x)/2, (side1.transform.position.y + side2.transform.position.y)/2, -2);
        GameObject StartedBattle = Instantiate(PlayerController.BattlePrefab, spawnPosition, Quaternion.identity);
        StartedBattle.GetComponent<Battle>().StartBattle(side1, side2);
    }

    private void StartSiege(GameObject besieger, GameObject defender)
    {
        Vector3 spawnPosition = new(defender.transform.position.x, defender.transform.position.y, -2);
        GameObject StartedSiege = Instantiate(PlayerController.SiegePrefab, spawnPosition, Quaternion.identity);
        StartedSiege.GetComponent<Siege>().StartSiege(besieger, defender);
    }
}
