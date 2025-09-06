using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI & Flow")]
    public GameObject gameOverPanel;
    public ScoreManager scoreManager;

    [Header("Parallax Layers (6 adet)")]
    // Her birine o katmanın Renderer'ını sürükle
    public Renderer[] parallaxLayers = new Renderer[6];

    // Her katman için hız (pozitif → sola doğru kayar)
    // En arkadan öne doğru artan hızlar örnek
    public float[] parallaxSpeeds = new float[6] { 0.1f, 0.2f, 0.35f, 0.55f, 0.8f, 1.0f };

    // İstersen oyunu hızlandırmak / yavaşlatmak için taban katsayı
    [Range(0f, 3f)]
    public float parallaxBaseSpeed = 1f;

    private Vector2[] _offsets;     // katman başına offset
    private Material[] _materials;  // paylaşımlı materyali klonlamamak için referanslar

    void Start()
    {
        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();

        // Güvenlik: dizi boyutlarını kontrol et
        if (parallaxLayers.Length != 6 || parallaxSpeeds.Length != 6)
            Debug.LogWarning("Parallax için 6 katman bekleniyor. Dizi boyutlarını kontrol et.");

        _offsets = new Vector2[parallaxLayers.Length];
        _materials = new Material[parallaxLayers.Length];

        // Her katmanın materyalini ve başlangıç offset'ini hazırla
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (parallaxLayers[i] == null) continue;

            // NOT: .material çağrısı o Renderer için materyalin bir kopyasını oluşturur.
            // Burada bilinçli olarak sahne-instanced materyal kullanıyoruz ki offset'i
            // sadece bu objede değiştirelim (sharedMaterial'ı etkilemeyelim).
            _materials[i] = parallaxLayers[i].material;
            _offsets[i] = Vector2.zero;

            // Wrap mode kontrolü (görsel amaçlı uyarı)
            if (_materials[i].mainTexture != null &&
                _materials[i].mainTexture.wrapMode != TextureWrapMode.Repeat)
            {
                Debug.LogWarning($"Parallax katmanı {i} için texture WrapMode = Repeat olmalı.");
            }
        }
    }

    void Update()
    {
        // Oyun durduğunda (Time.timeScale=0) kaydırma yapma
        if (Time.timeScale == 0f) return;

        // Her katmanın offset'ini hızına göre güncelle
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (_materials[i] == null) continue;

            float s = (i < parallaxSpeeds.Length ? parallaxSpeeds[i] : 0f) * parallaxBaseSpeed;
            _offsets[i].x += s * Time.deltaTime;
            _materials[i].mainTextureOffset = _offsets[i];
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        if (scoreManager != null) scoreManager.CheckHighScore();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // İsteğe bağlı: dışarıdan zorluk arttıkça parallax hızını yükseltmek için
    public void SetParallaxBaseSpeed(float value)
    {
        parallaxBaseSpeed = Mathf.Max(0f, value);
    }
}
