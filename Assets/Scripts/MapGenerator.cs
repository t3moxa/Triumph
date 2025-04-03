using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap watermap;
    public int Width = 40;
    public int Height = 40;
    public int NumberOfStates = 20;
    public float minDistance = 5f;
    public KingdomData NeutralKingdom;
    public KingdomData Kingdom1;
    public KingdomData Kingdom2;
    public int numOfClosestCities = 3;
    private List<GameObject> StateCenters = new();

    public void GenerateMap()
    {
        var tiles = CreateTiles();
        var waterTile = ScriptableObject.CreateInstance<StateTile>();
        waterTile.sprite = Resources.Load<Sprite>("Sprites/HighQuality/water"); ;

        while (StateCenters.Count < NumberOfStates)
        {
            // Генерация случайной позиции
            Vector3Int newStateCenter = new Vector3Int(Random.Range(1, Width-1), Random.Range(1, Height-1), -1);

            // Проверка расстояния до всех уже размещённых городов
            bool isValidPosition = true;
            foreach (GameObject city in StateCenters)
            {
                if (Vector3Int.Distance(newStateCenter, Vector3Int.RoundToInt(city.transform.position)) < minDistance)
                {
                    isValidPosition = false;
                    break;
                }
            }

            // Если позиция валидна, добавляем её в список
            if (isValidPosition)
            {
                var newCity = Instantiate(PlayerController.CityPrefab, newStateCenter, Quaternion.identity);
                newCity.GetComponent<City>().OnSpawn(NeutralKingdom);
                newCity.GetComponent<City>().SetStateTile(tiles[Random.Range(0, tiles.Count)]);
                StateCenters.Add(newCity);
            }
        }
        for (int i = -Height; i < Height*2; i++)
        {
            for (int j = -Width; j < Width*2; j++)
            {
                if ((i < 0) || (i > Height-1) || (j < 0) || (j > Width-1))
                { 
                    watermap.SetTile(new Vector3Int(i, j, 0), waterTile);
                }
            }
        }
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
            List<(GameObject obj, float distance)> distances = new();

            foreach (GameObject city in StateCenters)
            {
                float distance = Vector2.Distance(city.transform.position, new Vector2Int(i, j));
                distances.Add((city, distance));
            }

            distances.Sort((a, b) => a.distance.CompareTo(b.distance));

            tilemap.SetTile(new Vector3Int(i, j, 0), distances[0].obj.GetComponent<City>().StateTile);
            }
        }

        Vector2Int RightTop = new Vector2Int(40, 40);
        Vector2Int LeftBot = new Vector2Int(0, 0);
        float MinRTDist = 100;
        float MinLBDist = 100;
        GameObject RightTopCity = StateCenters[0];
        GameObject LeftBotCity = StateCenters[1];
        foreach (GameObject city in StateCenters)
        {
            if (Vector2.Distance(city.transform.position, RightTop) < MinRTDist)
            {
                MinRTDist = Vector2.Distance(city.transform.position, RightTop);
                RightTopCity = city;
            }
            else if (Vector2.Distance(city.transform.position, LeftBot) < MinLBDist)
            {
                MinLBDist = Vector2.Distance(city.transform.position, LeftBot);
                LeftBotCity = city;
            }
        }
        Kingdom1.SpawnKingdom(LeftBotCity);
        Kingdom2.SpawnAIKingdom(RightTopCity);
        StateCenters.Remove(LeftBotCity);
        StateCenters.Remove(RightTopCity);
        foreach (GameObject city in StateCenters)
        {
            TryAddGuard(city.GetComponent<City>());
        }
    }

    public void TryAddGuard(City city)
    {
        if(Random.Range(0, 3) == 0)
        {
            GameObject SpawnedSoldier = Instantiate(PlayerController.SoldierPrefab, new Vector3(city.gameObject.transform.position.x, city.gameObject.transform.position.y - 0.5f, -2), Quaternion.identity);
            SpawnedSoldier.GetComponent<Soldier>().OnSpawn(NeutralKingdom);
        }
    }
    public List<StateTile> CreateTiles()
    {
        List<StateTile> tiles = new();
        StateTile grass = ScriptableObject.CreateInstance<StateTile>();
        grass.sprite = Resources.Load<Sprite>("Sprites/HighQuality/luga"); ; // Присваиваем изображение тайлу
        tiles.Add(grass);
        StateTile snow = ScriptableObject.CreateInstance<StateTile>();
        snow.sprite = Resources.Load<Sprite>("Sprites/HighQuality/snow"); ; // Присваиваем изображение тайлу
        tiles.Add(snow);
        StateTile sand = ScriptableObject.CreateInstance<StateTile>();
        sand.sprite = Resources.Load<Sprite>("Sprites/HighQuality/desert"); ; // Присваиваем изображение тайлу
        tiles.Add(sand);
        StateTile swamp = ScriptableObject.CreateInstance<StateTile>();
        swamp.sprite = Resources.Load<Sprite>("Sprites/HighQuality/swamp"); ; // Присваиваем изображение тайлу
        tiles.Add(swamp);
        StateTile tree = ScriptableObject.CreateInstance<StateTile>();
        tree.sprite = Resources.Load<Sprite>("Sprites/HighQuality/forest"); ; // Присваиваем изображение тайлу
        tiles.Add (tree);
        return tiles;
    }    
}
