using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;

    private float timer = 0f;
    private float spawnInterval;

    void Start()
    {
        SetRandomInterval(); // ilk spawn aralığını ayarla
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnRandomObstacle();
            SetRandomInterval(); // bir sonraki spawn için yeni süre belirle
            timer = 0f;
        }
    }

    void SpawnRandomObstacle()
    {
        int index = Random.Range(0, obstaclePrefabs.Length);
        float yPos = 0f; // zemin hizası
        Instantiate(obstaclePrefabs[index], new Vector3(13f, yPos, 0), Quaternion.identity);
    }

    void SetRandomInterval()
    {
        spawnInterval = Random.Range(1f, 4f); // 1 ile 4 saniye arası rastgele süre
    }
}