using UnityEngine;

public class MenuBackgroundStarter : MonoBehaviour
{
    public MapGenerator mapgen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapgen.GenerateMap();
    }
}
