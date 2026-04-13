using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapAssetTable", menuName = "Map/Asset Table")]
public class MapAssetTable : ScriptableObject
{
    [System.Serializable]
    public class BlockOption
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float probability;
    }

    [System.Serializable]
    public class DecorationOption
    {
        public GameObject prefab;
        public int count;  
    }

    [Header("Blocks")]
    public List<BlockOption> blockOptions;

    [Header("Decorations")]
    public List<DecorationOption> decorationOptions;
}
