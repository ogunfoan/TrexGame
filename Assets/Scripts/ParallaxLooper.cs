using System.Collections.Generic;
using UnityEngine;

public class ParallaxLooper : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        [Header("Initial Tiles")]
        public Transform tileA;              // ilk parça
        public Transform tileB;              // ikinci parça
        public Transform tilePrefab;         // klonlanacak referans (A ya da B ile aynı sprite)

        [Header("Motion")]
        public float speed = 2f;

        [HideInInspector] public float width;
        [HideInInspector] public List<Transform> tiles = new List<Transform>();
    }

    [Header("Layers")]
    public Layer[] layers;

    [Tooltip("Sağdan-soldan bindirme; seam çizgisini gizlemek için ufak negatif offset")]
    public float overlapMargin = 0.01f;

    [Header("Başlangıç hizalaması")]
    public bool alignToCameraOnStart = true;

    [Header("Havuzlama")]
    [Tooltip("Her katmanda en fazla kaç tile aktif tutulacak (3 önerilir)")]
    public int maxTilesPerLayer = 3;

    void Start()
    {
        var cam = Camera.main;
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth  = camHalfHeight * cam.aspect;
        float camLeftX      = cam.transform.position.x - camHalfWidth;

        for (int i = 0; i < layers.Length; i++)
        {
            var L = layers[i];
            if (L.tileA == null || L.tileB == null) continue;

            // Genişlik ölç
            var sr = L.tileA.GetComponent<SpriteRenderer>();
            L.width = sr.bounds.size.x;

            // Clean list & add initial two
            L.tiles.Clear();
            L.tiles.Add(L.tileA);
            L.tiles.Add(L.tileB);

            // B'yi A'nın sağına hizala
            L.tileB.position = new Vector3(
                L.tileA.position.x + L.width - overlapMargin,
                L.tileA.position.y,
                L.tileA.position.z
            );

            // Başlangıçta kameraya göre hizala (ekrana çok uzakta başlamasın)
            if (alignToCameraOnStart)
            {
                float aCenterX = camLeftX + (L.width * 0.5f);
                float y = L.tileA.position.y;
                float z = L.tileA.position.z;

                L.tileA.position = new Vector3(aCenterX, y, z);
                L.tileB.position = new Vector3(aCenterX + L.width - overlapMargin, y, z);
            }

            // Başlangıçta sağda boşluk kalıyorsa 3. tile'ı bir kez oluştur
            TryEnsureRightCoverage(L);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        var cam = Camera.main;
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth  = camHalfHeight * cam.aspect;
        float camLeftX      = cam.transform.position.x - camHalfWidth;
        float camRightX     = cam.transform.position.x + camHalfWidth;

        for (int i = 0; i < layers.Length; i++)
        {
            var L = layers[i];
            if (L.tiles.Count < 2) continue;

            // 1) Tüm tile'ları sola kaydır
            float dx = -L.speed * Time.deltaTime;
            for (int t = 0; t < L.tiles.Count; t++)
            {
                if (L.tiles[t] != null)
                    L.tiles[t].Translate(dx, 0f, 0f);
            }

            // 2) Soldan tamamen çıkan varsa, en sağın arkasına taşı (recycle)
            //    NOT: İstersen burada Destroy/Instantiate de yapabilirsin fakat recycle performanslıdır.
            int rightmostIndex = GetRightmostIndex(L);
            float rightmostX   = L.tiles[rightmostIndex].position.x;

            for (int t = 0; t < L.tiles.Count; t++)
            {
                var tile = L.tiles[t];
                if (tile == null) continue;

                // Tile tamamen kameranın solundan ve bir genişlik kadar dışarı çıktıysa taşı
                if (tile.position.x <= camLeftX - L.width)
                {
                    // yeni konum: mevcut en sağ tile'ın sağı
                    float newX = rightmostX + L.width - overlapMargin;
                    tile.position = new Vector3(newX, tile.position.y, tile.position.z);

                    // yeni rightmost güncelle
                    rightmostX = newX;
                }
            }

            // 3) Sağda boşluk oluşursa (özellikle geniş ekranlarda), bir kez daha tile oluşturmayı dene
            TryEnsureRightCoverage(L, camRightX);
        }
    }

    /// <summary>
    /// Sağda kamera görünümünü kapatacak kadar tile var mı? Yoksa bir kez daha klonla.
    /// </summary>
    private void TryEnsureRightCoverage(Layer L, float camRightX = float.NegativeInfinity)
    {
        if (L.tilePrefab == null) return;
        if (L.tiles.Count >= maxTilesPerLayer) return;

        // En sağdaki tile'ı bul
        int rightmostIndex = GetRightmostIndex(L);
        float rightmostX   = L.tiles[rightmostIndex].position.x;

        // Eğer çağrı camRightX ile geldiyse (Update), sağda boşluk olup olmadığını kontrol et
        // camRightX parametresi verilmediyse (Start), direkt bir tile daha ekleyip "tam kaplama" yapabiliriz.
        bool needAnother =
            (camRightX == float.NegativeInfinity) ||
            (rightmostX + L.width - overlapMargin < camRightX);

        if (needAnother)
        {
            // Klonla ve en sağa koy
            var clone = Instantiate(L.tilePrefab, L.tiles[0].parent);
            clone.position = new Vector3(
                rightmostX + L.width - overlapMargin,
                L.tiles[0].position.y,
                L.tiles[0].position.z
            );
            L.tiles.Add(clone);
        }
    }

    private int GetRightmostIndex(Layer L)
    {
        int idx = 0;
        float maxX = float.NegativeInfinity;
        for (int t = 0; t < L.tiles.Count; t++)
        {
            if (L.tiles[t] == null) continue;
            float x = L.tiles[t].position.x;
            if (x > maxX)
            {
                maxX = x;
                idx = t;
            }
        }
        return idx;
    }
}
