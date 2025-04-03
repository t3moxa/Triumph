using UnityEngine;
using UnityEngine.Tilemaps;

public class StateTile : Tile
{
    public Building StateBuilding { get; private set; }
    public void SetBuilding(Building building)
    {
        StateBuilding = building;
    }
}
