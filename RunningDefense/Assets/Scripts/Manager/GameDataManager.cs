using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    [Header("Map Data Configs")]
    public MapEnemyConfig map1Config;
    public MapEnemyConfig map2Config;
    public MapEnemyConfig map3Config;

    [Header("Current Selected Map")]
    public MapEnemyConfig currentMapConfig;
    public MapType currentMap = MapType.None;

    public enum MapType { None, Map1, Map2, Map3 }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCurrentMap(MapType map)
    {
        currentMap = map;

        switch (map)
        {
            case MapType.Map1:
                currentMapConfig = map1Config;
                break;
            case MapType.Map2:
                currentMapConfig = map2Config;
                break;
            case MapType.Map3:
                currentMapConfig = map3Config;
                break;
            default:
                currentMapConfig = null;
                break;
        }

    }
}
