using UnityEngine;

public class BackGround : MonoBehaviour
{
     public float speed = 1.0f; // kaydırma hızı (sola)
    private float width;       // sprite genişliği (dünya biriminde)

    void Awake()
    {
        // Sprite'ı dünya biriminde ölç
        var sr = GetComponent<SpriteRenderer>();
        width = sr.bounds.size.x;
    }

    void Update()
    {
        // Sola kaydır
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Bu parça tamamen ekranın soluna çıktıysa, kendini sağa, öbür parçanın arkasına taşı
        // ÖNEMLİ: Başlangıçta BG_A ve BG_B arka arkaya hizalı olmalı
        if (transform.position.x <= -width)
        {
            transform.position += new Vector3(width * 2f, 0f, 0f);
        }
    }
}

