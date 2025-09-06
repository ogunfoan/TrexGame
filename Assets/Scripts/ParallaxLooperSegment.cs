using System.Collections.Generic;
using UnityEngine;

public class ParallaxLooperSegment : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        [Header("Prefab & Settings")]
        public Transform segmentPrefab;     // SpriteRenderer içeren prefab
        public float speed = 2f;            // Kayma hızı (ön plan hızlı, arka plan yavaş)
        [Range(2, 10)]
        public int initialSegments = 3;     // En az 3 önerilir

        [HideInInspector] public float width;             // segment genişliği (world)
        [HideInInspector] public List<Transform> segs;    // canlı segmentler
    }

    [Header("Katmanlar")]
    public Layer[] layers;

    [Header("Hizalama")]
    public bool alignToCameraOnStart = true;
    [Tooltip("Seam çizgisini gizlemek için minik bindirme")]
    public float overlapMargin = 0.01f;

    void Start()
    {
        var cam = Camera.main;
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth  = camHalfHeight * cam.aspect;
        float camLeftX      = cam.transform.position.x - camHalfWidth;

        for (int i = 0; i < layers.Length; i++)
        {
            var L = layers[i];
            if (L.segmentPrefab == null)
            {
                Debug.LogWarning($"Layer {i}: segmentPrefab atanmadı.");
                continue;
            }

            // Prefab genişliği
            var sr = L.segmentPrefab.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogError($"Layer {i}: Prefab üzerinde SpriteRenderer yok.");
                continue;
            }
            L.width = sr.bounds.size.x;

            // Gerekli segment sayısını, ekran genişliğini kaplayacak şekilde garanti et
            int minNeeded = Mathf.Max(L.initialSegments, Mathf.CeilToInt((camHalfWidth * 2f) / Mathf.Max(0.0001f, L.width)) + 2);
            L.segs = new List<Transform>(minNeeded);

            // Başlangıç X konumu
            float startCenterX = alignToCameraOnStart
                ? camLeftX + (L.width * 0.5f)
                : L.segmentPrefab.position.x;

            // Segmentleri oluştur ve yan yana diz
            float x = startCenterX;
            for (int s = 0; s < minNeeded; s++)
            {
                Transform seg = Instantiate(L.segmentPrefab, new Vector3(x, L.segmentPrefab.position.y, L.segmentPrefab.position.z), Quaternion.identity, transform);
                L.segs.Add(seg);
                x += L.width - overlapMargin; // minik bindirme ile sorunsuz geçiş
            }
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        for (int i = 0; i < layers.Length; i++)
        {
            var L = layers[i];
            if (L.segs == null || L.segs.Count == 0) continue;

            float dx = -L.speed * Time.deltaTime;

            // Hepsini sola kaydır
            for (int s = 0; s < L.segs.Count; s++)
                L.segs[s].Translate(dx, 0f, 0f);

            // Soldan tamamen çıkan segmenti en sağa at
            // (en soldakini bul, en sağdakinin sağına yerleştir)
            int leftmostIndex = 0;
            int rightmostIndex = 0;
            float minX = float.MaxValue;
            float maxX = float.MinValue;

            for (int s = 0; s < L.segs.Count; s++)
            {
                float sx = L.segs[s].position.x;
                if (sx < minX) { minX = sx; leftmostIndex = s; }
                if (sx > maxX) { maxX = sx; rightmostIndex = s; }
            }

            // soldaki segment, sağdakinin solundan yeterince uzaklaştıysa döndür
            if (L.segs[leftmostIndex].position.x <= L.segs[rightmostIndex].position.x - L.width)
            {
                Vector3 pos = L.segs[rightmostIndex].position;
                pos.x += L.width - overlapMargin;
                L.segs[leftmostIndex].position = new Vector3(pos.x, L.segs[leftmostIndex].position.y, L.segs[leftmostIndex].position.z);
            }
        }
    }
}
