using Consts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HornSpirit {
    public class WaveSpawner : MonoBehaviour {
        
        [SerializeField] private int numOfWaves;
        //[SerializeField] private GameObject spawnEffectPrefab;

        //EVENTS
        public delegate void AllWavesCompleteDelegate();
        public event AllWavesCompleteDelegate OnAllWavesComplete;
        public delegate void WaveCompleteDelegate();
        public event WaveCompleteDelegate OnWaveComplete;

        private WaveFactory waveFactory;
        private GameManager gameManager;
        private SpawnState state;
        private int currWaveIndex;

        private void Awake() {
            state = SpawnState.Waiting;
            waveFactory = GetComponent<WaveFactory>();
        }

        private void Init(int startingWave) 
        {
            currWaveIndex = startingWave;
            print(currWaveIndex);
            gameManager = GameManager.Instance;
            //_gameManager.SetCurrentWaveIndex(_currWaveIndex); //for UI count
        }

        private void Update() {
            //TODO some other class needs to do this!
            // if (currWaveIndex == numOfWaves && gameManager.GetEnemyCount() == 0 && state == SpawnState.Waiting) {
            //     state = SpawnState.Finish;
            //     OnAllWavesComplete?.Invoke();
            // }
        }

        public int GetMaxWaveCount() => numOfWaves;

        public SpawnState GetCurrentState() => state;

        public int GetCurrentWaveIndex() => currWaveIndex;

        //check if there are waves first
        public void SpawnCurrentWave() {
            if (state == SpawnState.Waiting && gameManager.GetEnemyCount() == 0) {
                if (currWaveIndex < numOfWaves) {
                    state = SpawnState.Spawning;
                    //_gameManager.SetCurrentWaveIndex(_currWaveIndex); //for UI count
                    Wave currWave = waveFactory.GetWave(currWaveIndex);
                    StartCoroutine(SpawnAllSubWaves(currWave)); //spawn all sub waves
                }
            }
        }

        //spawn all enemies in the current wave and setup next wave when done
        private IEnumerator SpawnAllSubWaves(Wave currWave) 
        {
            //Extract all sub waves of current wave
            List<SubWave> subWaves = currWave.GetSubWaves();

            for (int i = 0; i < subWaves.Count; i++) {
                SubWave currSubWave = subWaves[i];

                for (int j = 0; j < currSubWave.GetNumOfEnemies(); j++) {
                    SpawnEnemy(currSubWave);
                    yield return new WaitForSeconds(currWave.GetSpawnRate());
                }

                yield return new WaitForSeconds(currWave.GetTimeBetweenSubWaves());
            }
            //Wave Ended
            state = SpawnState.Waiting;
            currWaveIndex++;
            OnWaveComplete?.Invoke(); //trigger event at Game Manager
        }

        private void SpawnEnemy(SubWave currSubWave) {
            StartCoroutine(SpawnAnimationEffect(currSubWave));
        }

        private IEnumerator SpawnAnimationEffect(SubWave currSubWave) {
            Vector2 position = transform.position;
            yield return new WaitForSeconds(0.3f);
            Instantiate(currSubWave.GetEnemyPrefab(), position, Quaternion.identity);
        }

    }
}
