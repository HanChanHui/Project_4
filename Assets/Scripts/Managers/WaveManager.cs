using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace HornSpirit {
    [System.Serializable]
    public class SpawnerInfo{
        public List<SpawnerData> Spawner;
    }

    public class WaveManager : Singleton<WaveManager> 
    {
        [SerializeField] private SpawnerData spawnerData;
        [SerializeField] private List<WaveSpawner> waveSpawnerList;
        [SerializeField] private string jsonFileName;
        [SerializeField] private int spawnerId;

        private int activeSpawners;
 

        void Awake() {
            //int currLvl = Preferences.GetCurrentLvl();
            LoadLevelData(1000 + GameManager.Instance.GetLevelAndSpawnId); // 예를 들어 ID가 1001인 레벨을 로드
        }

        public void Init() 
        {
            InitializeWaveSpawners();
            foreach(WaveSpawner waveSpawner in waveSpawnerList)
            {
                waveSpawner.OnWaveComplete += OnWaveSpawnerComplete;
                waveSpawner.OnAllWavesComplete += OnAllWaveSpawnerComplete;
            }
        }

        private void InitializeWaveSpawners() 
        {
            int spawnerCount = spawnerData.spawnerNumber;
            for (int i = 0; i < spawnerCount; i++) 
            {
                if (i < waveSpawnerList.Count) 
                {
                    int count = spawnerData.WaveList[i].destinationId;
                    Transform target = GameManager.Instance.GetTargetList()[count - 1];
                    waveSpawnerList[i].Init(spawnerData.WaveList[i], target);
                    activeSpawners++;
                } else {
                    Debug.LogWarning($"WaveSpawner 리스트에 인덱스 {i}에 해당하는 Spawner가 없습니다.");
                }
            }
        }

        private void OnWaveSpawnerComplete()
        {
            activeSpawners--;
            Debug.Log("호출" + activeSpawners);
            if (activeSpawners == 0)
            {
                activeSpawners = waveSpawnerList.Count;
                StartNextWaveSet();
            }
        }

        private void OnAllWaveSpawnerComplete()
        {
            Debug.Log("All wave spawners have completed all their waves.");
        }

        private void StartNextWaveSet()
        {
            if (GameManager.Instance.GetEnemyCount() == 0)
            {
                foreach (WaveSpawner waveSpawner in waveSpawnerList)
                {
                    waveSpawner.StartWaveSet();
                }
            }
        }

        public void AddWaveSpawnerList(WaveSpawner wave) => waveSpawnerList.Add(wave);
        public int MaxEnemyDeathCount() => spawnerData.maxEnemyCount;

        // Json 읽기
        private void LoadLevelData(int spawnerId) {
            string filePath = Path.Combine(Application.dataPath + $"/Resources/JSON/{jsonFileName}.json");
            if (File.Exists(filePath)) {
                string jsonData = File.ReadAllText(filePath);
                SpawnerInfo spawnerInfo = JsonUtility.FromJson<SpawnerInfo>(jsonData);
                spawnerData = spawnerInfo.Spawner.FirstOrDefault(Spawner => Spawner.id == spawnerId);
            } else {
                Debug.LogError("JSON file not found at " + filePath);
            }
        }

    }
}
