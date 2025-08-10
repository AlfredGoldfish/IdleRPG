using UnityEngine;

/// <summary>
/// Spawns normal enemies and every 10th spawn a boss.
/// Designer can nudge the spawn point & uniform-scale each spawn batch.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;
    public GameObject bossPrefab;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Micro-placement")]
    public Vector2 spawnOffset = Vector2.zero;   // small X/Y nudge
    [Min(0.1f)] public float sizeScale = 1f;     // 1 = original size

    /* ─── runtime ─── */
    int enemiesDefeated;

    void Start() => SpawnEnemy();

    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
        SpawnEnemy();
    }

    /* core spawner */
    void SpawnEnemy()
    {
        // pick prefab
        GameObject prefab = ((enemiesDefeated + 1) % 10 == 0)
                            ? bossPrefab
                            : enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // position & scale
        Vector3 pos = spawnPoint.position + (Vector3)spawnOffset;
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.transform.localScale *= sizeScale;

        // link back
        var enemy = obj.GetComponent<Enemy>();
        if (enemy) enemy.spawner = this;
    }
}
