using UnityEngine;
using Pathfinding;

public class Spawner : MonoBehaviour
{
    public GameObject Enemy;
    public Transform target;
    public float coolTime;
    public bool spawnerEnabled = false;

    private Transform trans;
    private float nextSpawnTime;

    void Awake()
    {
        this.trans = this.transform;
        //this.nextSpawnTime = Time.time + Random.Range(1.0f, 2.0f);
        this.nextSpawnTime = Time.time + coolTime;
    }

    void Update()
    {
        if (!spawnerEnabled)
        {
            return;
        }

        if (Time.time >= this.nextSpawnTime)
        {
            var numToSpawn = 1;

            for (var i = 0; i < numToSpawn; i++)
            {
                var spawnPos = this.trans.position;
                spawnPos.x = Random.Range(spawnPos.x - 5.5f, spawnPos.x + 5.5f);
                var enemyInstance = Instantiate(Enemy, spawnPos, Enemy.transform.rotation);

                var aiDestinationSetter = enemyInstance.GetComponent<AIDestinationSetter>();
                if (aiDestinationSetter != null)
                {
                    aiDestinationSetter.target = target;
                }
            }

            //this.nextSpawnTime = Time.time + Random.Range(1.0f, 2.0f);
            this.nextSpawnTime = Time.time + coolTime;
        }
    }
}
