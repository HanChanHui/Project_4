using UnityEngine;
using Pathfinding;

public class Spawner : MonoBehaviour {
    [SerializeField] GameObject enemy;
    [SerializeField] private Transform enemyTarget;

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

            for (var i = 0; i < numToSpawn; i++) 
            {
                GameObject enemyInstance = Instantiate(enemy, transform.position, enemy.transform.rotation);
                enemyInstance.GetComponent<BaseEnemy>().OriginalTarget = enemyTarget;
                
                GameManager.Instance.AddPlaceableEnemyList(enemyInstance.GetComponent<BaseEnemy>());

                this.nextSpawnTime = Time.time + coolTime;
            }
        }
    }
}
