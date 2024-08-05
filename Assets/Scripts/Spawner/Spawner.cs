using UnityEngine;
using Pathfinding;

public class Spawner : MonoBehaviour {
    public GameObject enemy;

    public float coolTime;
    public bool spawnerEnabled = false;

    private Transform trans;
    private float nextSpawnTime;

    void Awake() {
        this.trans = this.transform;
        //this.nextSpawnTime = Time.time + Random.Range(1.0f, 2.0f);
        this.nextSpawnTime = Time.time + coolTime;
    }

    private void Start() {
        enemy = ResourceManager.Instance.enemyPrefab;
    }

    void Update() {
        if (!spawnerEnabled) {
            return;
        }

        if (Time.time >= this.nextSpawnTime) {
            var numToSpawn = 1;

            for (var i = 0; i < numToSpawn; i++) {
                var spawnPos = this.trans.position;
                spawnPos.x = Random.Range(spawnPos.x - 5.5f, spawnPos.x + 5.5f);
                var enemyInstance = Instantiate(enemy, spawnPos, enemy.transform.rotation);

                //this.nextSpawnTime = Time.time + Random.Range(1.0f, 2.0f);
                this.nextSpawnTime = Time.time + coolTime;
            }
        }
    }
}
