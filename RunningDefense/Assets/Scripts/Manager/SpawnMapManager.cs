using System.Collections.Generic;
using UnityEngine;

public class SpawnMapManager : MonoBehaviour
{
    [Header("Map Settings")]
    public int sizeX = 25;
    public int sizeZ = 25;
    private float blockSize = 2f;

    [Header("Chunk Settings")]
    public int chunkSize = 10;
    public Camera mainCamera;
    public GameObject mapParent;

    [Header("Assets Table")]
    public MapAssetTable assetTable;

    // chunk data thay cho list<GameObject>
    class ChunkData
    {
        public GameObject go;
        public List<Vector3> blockCenters = new List<Vector3>();
        public int cx;
        public int cz;
        public MeshRenderer chunkRenderer;
    }

    // thông tin từng ô block trên toàn bản đồ, dùng để spawn decoration
    struct BlockInfo
    {
        public Vector3 center;
        public int chunkIndex;
        public int worldX;
        public int worldZ;
        public BlockInfo(Vector3 c, int ci, int wx, int wz) { center = c; chunkIndex = ci; worldX = wx; worldZ = wz; }
    }

    private List<ChunkData> chunks = new List<ChunkData>();
    private List<BlockInfo> allBlocks = new List<BlockInfo>();

    private float offsetX;
    private float offsetZ;

    void Awake()
    {
        assetTable = GameDataManager.Instance.currentMapConfig.mapAssetTable;
    }

    void Start()
    {
        // Tính offset để căn tâm bản đồ
        offsetX = (sizeX * blockSize) / 2f;
        offsetZ = (sizeZ * blockSize) / 2f;

        GenerateChunks();
        SpawnDecorations();
    }

    void Update()
    {
        UpdateChunkVisibility();
    }

    void GenerateChunks()
    {
        allBlocks.Clear();
        chunks.Clear();

        float totalProb = 0f;
        foreach (var b in assetTable.blockOptions) totalProb += b.probability;
        if (totalProb <= 0f)
        {            return;
        }

        int chunkCountX = Mathf.CeilToInt((float)sizeX / chunkSize);
        int chunkCountZ = Mathf.CeilToInt((float)sizeZ / chunkSize);

        for (int cx = 0; cx < chunkCountX; cx++)
        {
            for (int cz = 0; cz < chunkCountZ; cz++)
            {
                // tạo chunk data và thêm vào list ngay để có index
                ChunkData chunkData = new ChunkData();
                chunkData.go = new GameObject($"Chunk_{cx}_{cz}");
                chunkData.go.transform.parent = mapParent.transform;
                chunkData.cx = cx;
                chunkData.cz = cz;

                List<CombineInstance> combine = new List<CombineInstance>();

                for (int x = 0; x < chunkSize; x++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        int worldX = cx * chunkSize + x;
                        int worldZ = cz * chunkSize + z;
                        if (worldX >= sizeX || worldZ >= sizeZ) continue;

                        GameObject prefab = GetRandomBlock(totalProb);
                        if (prefab == null) continue;

                        // lấy y gốc của prefab (pivot)
                        float prefabY = prefab.transform.position.y;

                        Vector3 pos = new Vector3(worldX * blockSize - offsetX,
                                                  prefabY,
                                                  worldZ * blockSize - offsetZ);

                        // instantiate tạm để lấy mesh transform rồi destroy
                        GameObject temp = Instantiate(prefab, pos, Quaternion.identity);
                        MeshFilter mf = temp.GetComponent<MeshFilter>();
                        if (mf != null && mf.sharedMesh != null)
                        {
                            CombineInstance ci = new CombineInstance();
                            ci.mesh = mf.sharedMesh;
                            ci.transform = mf.transform.localToWorldMatrix;
                            combine.Add(ci);
                        }

                        // lưu tâm block center (dùng để spawn decoration sau), + offset nửa block để center
                        Vector3 center = pos + new Vector3(blockSize / 2f, 0, blockSize / 2f);
                        chunkData.blockCenters.Add(center);

                        // thêm vào danh sách global (kèm index chunk và tọa độ lưới)
                        allBlocks.Add(new BlockInfo(center, chunks.Count, worldX, worldZ));

                        Destroy(temp);
                    }
                }

                // combine mesh cho chunk
                Mesh combinedMesh = new Mesh();
                combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                if (combine.Count > 0) combinedMesh.CombineMeshes(combine.ToArray(), true, true);

                chunkData.go.transform.localPosition = Vector3.zero;

                MeshFilter mfChunk = chunkData.go.AddComponent<MeshFilter>();
                MeshRenderer mrChunk = chunkData.go.AddComponent<MeshRenderer>();

                mfChunk.mesh = combinedMesh;

                // dùng material của blockOptions[0] nếu có
                if (assetTable.blockOptions.Count > 0 && assetTable.blockOptions[0].prefab != null)
                {
                    MeshRenderer prefabRenderer = assetTable.blockOptions[0].prefab.GetComponent<MeshRenderer>();
                    if (prefabRenderer != null)
                        mrChunk.sharedMaterial = prefabRenderer.sharedMaterial;
                }

                chunkData.chunkRenderer = mrChunk;
                chunks.Add(chunkData);
            }
        }
    }

    GameObject GetRandomBlock(float totalProb)
    {
        float rand = Random.value * totalProb;
        float cumulative = 0f;
        foreach (var b in assetTable.blockOptions)
        {
            cumulative += b.probability;
            if (rand <= cumulative)
                return b.prefab;
        }
        return assetTable.blockOptions[0].prefab;
    }

    void SpawnDecorations()
    {
        if (allBlocks.Count == 0) return;

        // lưu lại các vị trí decoration đã spawn (global)
        List<Vector3> spawnedPositions = new List<Vector3>();
        float minDistance = blockSize * 5f; // khoảng cách tối thiểu (2 block) giữa decorations
        int maxTriesPerDeco = 200;

        foreach (var deco in assetTable.decorationOptions)
        {
            for (int i = 0; i < deco.count; i++)
            {
                bool placed = false;
                int tries = 0;

                while (!placed && tries < maxTriesPerDeco)
                {
                    tries++;

                    // random block index trong toàn bản đồ
                    int idx = Random.Range(0, allBlocks.Count);
                    BlockInfo bi = allBlocks[idx];

                    // kiểm tra near border (cách viền >= 2 block)
                    bool nearBorder = bi.worldX < 2 || bi.worldX >= sizeX - 2 ||
                                      bi.worldZ < 2 || bi.worldZ >= sizeZ - 2;
                    if (nearBorder) continue;

                    // kiểm tra khoảng cách với các decoration đã spawn
                    bool tooClose = false;
                    foreach (var sp in spawnedPositions)
                    {
                        if (Vector3.Distance(sp, bi.center) < minDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                    if (tooClose) continue;

                    // ok, spawn vào chunk tương ứng và parent vào chunk GO
                    int chunkIdx = bi.chunkIndex;
                    if (chunkIdx < 0 || chunkIdx >= chunks.Count) continue;

                    // lấy y gốc của prefab decoration và áp dụng
                    float prefabY = deco.prefab.transform.position.y;
                    Vector3 spawnPos = new Vector3(bi.center.x, bi.center.y + prefabY, bi.center.z);

                    GameObject inst = Instantiate(deco.prefab, spawnPos, Quaternion.identity, chunks[chunkIdx].go.transform);
                    spawnedPositions.Add(bi.center);
                    placed = true;
                }
                // nếu sau nhiều lần không tìm được vị trí hợp lệ thì bỏ qua
            }
        }
    }

    void UpdateChunkVisibility()
    {
        if (mainCamera == null) return;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        for (int i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];
            if (chunk == null || chunk.chunkRenderer == null) continue;
            Bounds bounds = chunk.chunkRenderer.bounds;
            bool visible = GeometryUtility.TestPlanesAABB(planes, bounds);
            // bật/tắt toàn bộ chunk (bao gồm decorations con)
            chunk.go.SetActive(visible);
        }
    }
}
