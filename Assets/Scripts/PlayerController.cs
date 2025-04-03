using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static KingdomData PlayerKingdom;
    public static KingdomData EnemyKingdom;
    public static EnemyAI EnemyAI;
    GameObject Selected = null;
    Selectable SelectedComponent = null;
    bool IsStarted = false;
    public MapGenerator MapGen;
    public static GameObject CapitalInfoPanel;
    public static GameObject CityInfoPanel;
    public static GameObject SoldierInfoPanel;
    public static GameObject HeroInfoPanel;
    public static GameObject BattleInfoPanel;
    public static TextMeshProUGUI GoldNum;
    public static GameObject MainMenu;
    public static GameObject EndGameMenu;

    public static GameObject SoldierPrefab;
    public static GameObject HeroPrefab;
    public static GameObject CityPrefab;
    public static GameObject CapitalPrefab;
    public static GameObject BattlePrefab;
    public static GameObject SiegePrefab;
    public static GameObject TooltipPrefab;

    public static Button BuildSoldierButton;
    public static Button BuildHeroButton;
    public static Button JoinHeroButton;
    public static Button ReleaseSoldiersButton;
    public static Button StartGameButton;
    public static Button MainExitGameButton;
    public static Button ExitGameButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerKingdom = GameObject.Find("Kingdom1").GetComponent<KingdomData>();
        EnemyKingdom = GameObject.Find("Kingdom2").GetComponent<KingdomData>();
        EnemyAI = GameObject.Find("Kingdom2").GetComponent<EnemyAI>();
        CapitalInfoPanel = GameObject.Find("CapitalInfoPanel");
        CityInfoPanel = GameObject.Find("CityInfoPanel");
        SoldierInfoPanel = GameObject.Find("SoldierInfoPanel");
        HeroInfoPanel = GameObject.Find("HeroInfoPanel");
        BattleInfoPanel = GameObject.Find("BattleInfoPanel");
        GoldNum = GameObject.Find("GoldNum").GetComponent<TextMeshProUGUI>();
        MainMenu = GameObject.Find("MainMenu");
        EndGameMenu = GameObject.Find("EndGameMenu");

        SoldierPrefab = Resources.Load<GameObject>("Prefabs/SoldierPrefab");
        HeroPrefab = Resources.Load<GameObject>("Prefabs/HeroPrefab");
        CityPrefab = Resources.Load<GameObject>("Prefabs/CityPrefab");
        CapitalPrefab = Resources.Load<GameObject>("Prefabs/CapitalPrefab");
        BattlePrefab = Resources.Load<GameObject>("Prefabs/BattlePrefab");
        SiegePrefab = Resources.Load<GameObject>("Prefabs/SiegePrefab");
        TooltipPrefab = Resources.Load<GameObject>("Prefabs/TooltipPrefab");

        BuildSoldierButton = GameObject.Find("BuildSoldierButton").GetComponent<Button>();
        BuildHeroButton = GameObject.Find("BuildHeroButton").GetComponent<Button>();
        JoinHeroButton = GameObject.Find("JoinHeroButton").GetComponent<Button>();
        ReleaseSoldiersButton = GameObject.Find("ReleaseSoldiersButton").GetComponent<Button>();
        StartGameButton = GameObject.Find("StartGameButton").GetComponent<Button>();
        MainExitGameButton = GameObject.Find("MainExitGameButton").GetComponent<Button>();
        ExitGameButton = GameObject.Find("ExitGameButton").GetComponent<Button>();

        BuildSoldierButton.onClick.AddListener(BuildSoldier);
        BuildHeroButton.onClick.AddListener(BuildHero);
        JoinHeroButton.onClick.AddListener(OrderSoldierToJoin);
        ReleaseSoldiersButton.onClick.AddListener(OrderHeroToRelease);
        StartGameButton.onClick.AddListener(StartGame);
        MainExitGameButton.onClick.AddListener(ExitGame);
        ExitGameButton.onClick.AddListener(ExitGame);

        BuildSoldierButton.gameObject.SetActive(false);
        BuildHeroButton.gameObject.SetActive(false);
        JoinHeroButton.gameObject.SetActive(false);
        ReleaseSoldiersButton.gameObject.SetActive(false);

        CapitalInfoPanel.SetActive(false);
        CityInfoPanel.SetActive(false);
        SoldierInfoPanel.SetActive(false);
        HeroInfoPanel.SetActive(false);
        BattleInfoPanel.SetActive(false);
        EndGameMenu.SetActive(false);

        MapGen.GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        GoldNum.text = PlayerKingdom.Gold.ToString();
        if (Input.GetMouseButtonDown(0)) // Проверяем нажатие левой кнопки мыши
        {
            // Получаем позицию мыши в мировых координатах
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero); // Создаем Raycast

            if (EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log("Кастомный UI элемент был нажат: ");
                return;
            }

            if (IsStarted)
            {
                if (Selected != null)
                {
                    SelectedComponent.OnDeselect();
                    Selected = null;
                    SelectedComponent = null;
                }

                if ((hit.collider != null)
                    //&& !hit.collider.isTrigger 
                    && hit.collider.gameObject.GetComponent<Selectable>())
                {
                    //Debug.Log("Нажали на селектабл");
                    var SelectableTuple = hit.collider.gameObject.GetComponent<Selectable>().OnSelect();
                    Selected = SelectableTuple.Item1;
                    SelectedComponent = SelectableTuple.Item2;
                }
            }
        }
        if (Input.GetMouseButtonDown(1) && (Selected != null) && (SelectedComponent.Owner == PlayerKingdom))
        {
            SelectedComponent.OnOrder();
        }
    }
    void StartGame()
    {
        IsStarted = true;
        PlayerKingdom.OnStart();
        EnemyKingdom.OnStart();
        EnemyAI.OnStart();
        MainMenu.SetActive(false);
    }
    void ExitGame()
    {
        Application.Quit();
    }
    public void BuildSoldier()
    {
        if (SelectedComponent.Owner.Gold >= 5)
        {
            SelectedComponent.Owner.SubstractGold(5);
            if (SelectedComponent.Owner == PlayerKingdom)
            {
                Capital cap = SelectedComponent as Capital;
                cap.SpawnSoldier();
            }
        }
    }

    public void BuildHero()
    {
        if (SelectedComponent.Owner.Gold >= 10)
        {
            SelectedComponent.Owner.SubstractGold(10);
            if (SelectedComponent.Owner == PlayerKingdom)
            {
                Capital cap = SelectedComponent as Capital;
                cap.SpawnHero();
            }
        }
    }

    public void OrderSoldierToJoin()
    {
        if (SelectedComponent.Owner == PlayerKingdom)
        {
            Soldier s = SelectedComponent as Soldier;
            s.FindAndJoinHero();
        }
    }

    public void OrderHeroToRelease()
    {
        if (SelectedComponent.Owner == PlayerKingdom)
        {
            Hero h = SelectedComponent as Hero;
            h.ReleaseSoldiers();
        }
    }

    public void CreateTooltip(Vector2 spawnPosition, string text)
    {
        GameObject Tooltip = Instantiate(TooltipPrefab, spawnPosition, Quaternion.identity);
        Tooltip.GetComponent<TextMeshPro>().text = text;
        StartCoroutine(TooltipFade(Tooltip.GetComponent<TextMeshPro>()));
    }

    private IEnumerator TooltipFade(TextMeshPro TMP)
    {
        Color originalColor = TMP.color;
        float elapsedTime = 0f;

        while (elapsedTime < 5f)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0, elapsedTime / 5f);
            TMP.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        TMP.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        Destroy(TMP.gameObject);
    }
    public static void Lose()
    {
        EndGameMenu.SetActive(true);
        EndGameMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Defeat!";
    }
    public static void Win()
    {
        EndGameMenu.SetActive(true);
    }
}
