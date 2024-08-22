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


    public class WaveManager : MonoBehaviour 
    {
        [SerializeField] private SpawnerData spawnerData;
        [SerializeField] private string jsonFileName;
        [SerializeField] private int spawnerId;

        void Awake() {
            int currLvl = Preferences.GetCurrentLvl();
            LoadLevelData(1000 + currLvl); // 예를 들어 ID가 1001인 레벨을 로드
        }

        // Json 읽기
        private void LoadLevelData(int levelId) {
            string filePath = System.IO.Path.Combine(Application.dataPath + $"/Resources/JSON/{jsonFileName}.json");
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
