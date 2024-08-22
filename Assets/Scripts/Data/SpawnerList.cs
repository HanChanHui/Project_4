using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;

namespace HornSpirit {
    [System.Serializable]
    public class SpawnerList : SpreadToJSON
    {
        public SpawnerData[] Spawner;
        int id = 0;
        int spawnerNumber;
        int targetHP;
        int maxEnemyCount;
        int spawnerId;
        int destinationId;
        int waveCount;

        List<WaveData> WaveList = new List<WaveData>();
        List<WaveInfoData> waveInfoList = new List<WaveInfoData>();

        public override void ParseAndSave(string sheet, string fileName)
        {
            SpawnerList spawnerList = new() {
                Spawner = WaveParseJSON((column, isEnd) => {
                    SpawnerData spawner = new();
                    ReadData(column, spawner, isEnd);
                    return spawner;
                }, sheet)
            };

            SaveJSONFile(spawnerList, fileName);
        }

        public void ReadData(string[] column, SpawnerData spawner, bool isEnd) 
        {
            UnityEngine.Debug.Log(isEnd);
            if(column[0] == "SpawnerNumber")
            {
                UnityEngine.Debug.Log("SpawnerNumber");
                if(id != 0)
                {
                    SetAndInit(spawner);
                }
                
                spawnerNumber = int.Parse(column[1]);
                targetHP = int.Parse(column[3]);
                maxEnemyCount = int.Parse(column[5]);
            }
            else if(column[0] == "" && column[1] == "WaveNumber" && column[2] != "")
            {
                UnityEngine.Debug.Log("WaveNumber");

                List<WaveTerm> waveTermList = new List<WaveTerm>();
                
                for (int i = 3; i < int.Parse(column[2]) + 3; i++) 
                {
                    if(column[i] != "")
                    {
                        string[] waveTermInfo = column[i].Split(',');
                        UnityEngine.Debug.Log(waveTermInfo[0] + ", " + waveTermInfo[1] + ", " + waveTermInfo[2]);
                        UnityEngine.Debug.Log(i + ", " + (int.Parse(column[2]) + 3));
                        WaveTerm waveTermData = new WaveTerm(int.Parse(waveTermInfo[0]),
                                                                    int.Parse(waveTermInfo[1]),
                                                                    float.Parse(waveTermInfo[2]));
                        waveTermList.Add(waveTermData);
                    }
                }
                waveInfoList.Add(new WaveInfoData(waveTermList, int.Parse(column[2])));

                if(waveInfoList.Count >= waveCount)
                {
                    WaveList.Add(new WaveData(waveInfoList, spawnerId, destinationId, waveCount));
                }
            }
            else if(column[0] != "")
            {
                UnityEngine.Debug.Log("Id");
                id = int.Parse(column[0]);
                
                spawnerId = int.Parse(column[1]);
                destinationId = int.Parse(column[2]);
                waveCount = int.Parse(column[3]);

                waveInfoList = new List<WaveInfoData>();
            }

            if(isEnd)
            {
                SetAndInit(spawner);
            }
        }

        public void SetAndInit(SpawnerData spawner)
        {
            spawner.id = id;
            spawner.spawnerNumber = spawnerNumber;
            spawner.targetHP = targetHP;
            spawner.maxEnemyCount = maxEnemyCount;
            spawner.WaveList = WaveList;

            id = 0;
            spawnerNumber = 0;
            targetHP = 0;
            maxEnemyCount = 0;
            WaveList = new List<WaveData>();
        }
    }
}