using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Capital : Building
{
    Coroutine SpawningCoroutine = null;
    public TextMeshPro BuildingText;

    public override void ShowBuildingInfoPanel()
    {
        PlayerController.CapitalInfoPanel.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = Name;
        if (Owner == PlayerController.PlayerKingdom)
        {
            PlayerController.CapitalInfoPanel.transform.GetChild(2).gameObject.SetActive(true);
            PlayerController.CapitalInfoPanel.transform.GetChild(3).gameObject.SetActive(true);
        }
        PlayerController.CapitalInfoPanel.SetActive(true);
    }

    public override void OnConquer(KingdomData Conqueror)
    {
        if (Owner = PlayerController.PlayerKingdom)
        {
            PlayerController.Lose();
        }
        else
        {
            PlayerController.Win();
        }
        SpawningCoroutine = null;
        Owner = Conqueror;
        LiftSiege();
        ChangeBanner();
    }
    public override void HideAllPanels()
    {
        PlayerController.CapitalInfoPanel.transform.GetChild(2).gameObject.SetActive(false);
        PlayerController.CapitalInfoPanel.transform.GetChild(3).gameObject.SetActive(false);
        PlayerController.CapitalInfoPanel.SetActive(false);
    }

    public void SpawnSoldier()
    {
        if (SpawningCoroutine == null)
        {
            SpawningCoroutine = StartCoroutine(SpawningSoldier());
            BuildingText.gameObject.SetActive(true);
        }
    }
    private IEnumerator SpawningSoldier()
    {
        int timeElapsed = 0;
        while (true)
        {
            timeElapsed++;
            BuildingText.text = timeElapsed.ToString() + "/5";
            if (timeElapsed > 4)
            {
                break;
            }
            yield return new WaitForSeconds(1);
        }
        GameObject SpawnedSoldier = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier.GetComponent<Soldier>().OnSpawn(Owner);
        BuildingText.gameObject.SetActive(false);
        SpawningCoroutine = null;
    }
    public void SpawnHero()
    {
        if (SpawningCoroutine == null)
        {
            SpawningCoroutine = StartCoroutine(SpawningHero());
            BuildingText.gameObject.SetActive(true);
        }
    }
    private IEnumerator SpawningHero()
    {
        int timeElapsed = 0;
        while (true)
        {
            timeElapsed++;
            BuildingText.text = timeElapsed.ToString() + "/8";
            if (timeElapsed > 7)
            {
                break;
            }
            yield return new WaitForSeconds(1);
        }
        GameObject SpawnedHero = Instantiate(PlayerController.HeroPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedHero.GetComponent<Hero>().OnSpawn(Owner);
        BuildingText.gameObject.SetActive(false);
        SpawningCoroutine = null;
    }
    public void SpawnStartingArmy()
    {
        GameObject SpawnedSoldier1 = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x - 1, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier1.GetComponent<Soldier>().OnSpawn(Owner);
        GameObject SpawnedSoldier2 = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x + 1, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier2.GetComponent<Soldier>().OnSpawn(Owner);
        GameObject SpawnedHero = Instantiate(PlayerController.HeroPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedHero.GetComponent<Hero>().OnSpawn(Owner);
    }
    public void SpawnAIStartingArmy(Vector2 position)
    {
        GameObject SpawnedHero = Instantiate(PlayerController.HeroPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedHero.GetComponent<Hero>().OnSpawn(Owner);
        GameObject SpawnedSoldier1 = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x - 1, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier1.GetComponent<Soldier>().OnSpawn(Owner);
        SpawnedSoldier1.GetComponent<Soldier>().FindAndJoinHero();
        GameObject SpawnedSoldier2 = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x + 1, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier2.GetComponent<Soldier>().OnSpawn(Owner);
        SpawnedSoldier2.GetComponent<Soldier>().FindAndJoinHero();
        SpawnedHero.GetComponent<Hero>().ForceMove(position);
    }
    public void AISpawnSoldier(Vector2 position)
    {
        if (SpawningCoroutine == null)
        {
            SpawningCoroutine = StartCoroutine(AISpawningSoldier(position));
            BuildingText.gameObject.SetActive(true);
        }
    }
    private IEnumerator AISpawningSoldier(Vector2 position)
    {
        int timeElapsed = 0;
        while (true)
        {
            timeElapsed++;
            BuildingText.text = timeElapsed.ToString() + "/7";
            if (timeElapsed > 6)
            {
                break;
            }
            yield return new WaitForSeconds(1);
        }
        GameObject SpawnedSoldier = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier.GetComponent<Soldier>().OnSpawn(Owner);
        SpawnedSoldier.GetComponent<Soldier>().ForceMove(position);
        BuildingText.gameObject.SetActive(false);
        SpawningCoroutine = null;
    }
    public void AISpawnSquad(Vector2 position)
    {
        if (SpawningCoroutine == null)
        {
            SpawningCoroutine = StartCoroutine(AISpawningSquad(position));
            BuildingText.gameObject.SetActive(true);
        }
    }

    public IEnumerator AISpawningSquad(Vector2 position)
    {
        int timeElapsed = 0;
        while (true)
        {
            timeElapsed++;
            BuildingText.text = timeElapsed.ToString() + "/12";
            if (timeElapsed > 11)
            {
                break;
            }
            yield return new WaitForSeconds(1);
        }
        GameObject SpawnedHero = Instantiate(PlayerController.HeroPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedHero.GetComponent<Hero>().OnSpawn(Owner);
        SpawnedHero.GetComponent<Hero>().AddExperience(50);
        GameObject SpawnedSoldier1 = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier1.GetComponent<Soldier>().OnSpawn(Owner);
        SpawnedSoldier1.GetComponent<Soldier>().FindAndJoinHero();
        GameObject SpawnedSoldier2 = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier2.GetComponent<Soldier>().OnSpawn(Owner);
        SpawnedSoldier2.GetComponent<Soldier>().FindAndJoinHero();
        SpawnedHero.GetComponent<Hero>().ForceMove(position);
        BuildingText.gameObject.SetActive(false);
        SpawningCoroutine = null;
    }

    public void AISpawnBigSquad(Vector2 position)
    {
        if (SpawningCoroutine == null)
        {
            SpawningCoroutine = StartCoroutine(AISpawningBigSqiad(position));
            BuildingText.gameObject.SetActive(true);
        }
    }

    public IEnumerator AISpawningBigSqiad(Vector2 position)
    {
        int timeElapsed = 0;
        while (true)
        {
            timeElapsed++;
            BuildingText.text = timeElapsed.ToString() + "/18";
            if (timeElapsed > 17)
            {
                break;
            }
            yield return new WaitForSeconds(1);
        }
        GameObject SpawnedHero = Instantiate(PlayerController.HeroPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedHero.GetComponent<Hero>().OnSpawn(Owner);
        SpawnedHero.GetComponent<Hero>().AddExperience(150);
        GameObject SpawnedSoldier1 = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier1.GetComponent<Soldier>().OnSpawn(Owner);
        SpawnedSoldier1.GetComponent<Soldier>().FindAndJoinHero();
        GameObject SpawnedSoldier2 = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier2.GetComponent<Soldier>().OnSpawn(Owner);
        SpawnedSoldier2.GetComponent<Soldier>().FindAndJoinHero();
        GameObject SpawnedSoldier3 = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier3.GetComponent<Soldier>().OnSpawn(Owner);
        SpawnedSoldier3.GetComponent<Soldier>().FindAndJoinHero();
        GameObject SpawnedSoldier4 = Instantiate(PlayerController.SoldierPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), Quaternion.identity);
        SpawnedSoldier4.GetComponent<Soldier>().OnSpawn(Owner);
        SpawnedSoldier4.GetComponent<Soldier>().FindAndJoinHero();
        SpawnedHero.GetComponent<Hero>().ForceMove(position);
        BuildingText.gameObject.SetActive(false);
        SpawningCoroutine = null;
    }
}
