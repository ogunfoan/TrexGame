using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject shieldPrefab;
    public float minSpawnTime = 10f;
    public float maxSpawnTime = 20f;
    private float timer = 0f;
    private float spawnInterval;

    void Start()
    {
        SetRandomInterval();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnShield();
            SetRandomInterval();
            timer = 0f;
        }
    }

    void SpawnShield()
    {
        float xPos = Random.Range(8f, 12f);     // görünür alanda
        float yPos = Random.Range(-3f, 0f);     // zemin üstü seviyesinde
        Instantiate(shieldPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
    }

    void SetRandomInterval()
    {
        spawnInterval = Random.Range(minSpawnTime, maxSpawnTime);
    }
}