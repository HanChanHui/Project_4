using Consts;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace HornSpirit {
    public class WaveSpawner : MonoBehaviour {
        
        //EVENTS
        public delegate void AllWavesCompleteDelegate();
        public event AllWavesCompleteDelegate OnAllWavesComplete;
        public delegate void WaveCompleteDelegate();
        public event WaveCompleteDelegate OnWaveComplete;

        public Transform target; // 목표 타겟
        private WaveFactory waveFactory;
        private SpawnState state = SpawnState.Waiting;

        private List<WaveInfoData> waveInfoDataList;
        private int currentWaveTermIndex = 0;
        private int currentWaveInfoIndex = 0;
        private int waveCount = 0;

        private void Awake() {
            state = SpawnState.Waiting;
            waveFactory = GetComponent<WaveFactory>();
            WaveManager.Instance.AddWaveSpawnerList(this);
        }

        private void Start() {
            
        }

        public void Init(WaveData waveData, Transform targetTr) 
        {
            waveInfoDataList = waveData.waveInfoList;
            target = targetTr;
            waveCount = waveData.waveCount;
            StartWaveSet();
        }


        public SpawnState GetCurrentState() => state;

        public int GetCurrentWaveTermIndex() => currentWaveTermIndex;

        public void StartWaveSet()
        {
            if(currentWaveInfoIndex < waveCount)
            {
                List<WaveTerm> waveTermList = waveInfoDataList[currentWaveInfoIndex].waveTermList;
                StartCoroutine(StartWaveCoroutine(waveTermList));
            }
            else
            {
                state = SpawnState.Finish;
                OnAllWavesComplete?.Invoke();
            }
        }

        private IEnumerator StartWaveCoroutine(List<WaveTerm> waveTermList)
        {
            while(currentWaveTermIndex < waveTermList.Count)
            {
                state = SpawnState.Spawning;
                WaveTerm currentTerm = waveTermList[currentWaveTermIndex];

                yield return StartCoroutine(waveFactory.SpawnSubWave(currentTerm, target, transform));

                state = SpawnState.Waiting;
                currentWaveTermIndex++;
            }

            while (GameManager.Instance.GetEnemyCount() > 0)
            {
                yield return null;
            }

            currentWaveInfoIndex++;
            currentWaveTermIndex = 0;
            state = SpawnState.Waiting;
            OnWaveComplete?.Invoke();
        }

    }
}
