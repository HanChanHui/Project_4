using System.Collections.Generic;
using UnityEngine;

namespace HornSpirit {
    public class Wave 
    {
        private List<SubWave> subWaves = new List<SubWave>();
        private float spawnRate;
        private float timeBetweenSubWaves;

        //Normal Wave
        public Wave(List<SubWave> subWaves, float spawnRate, float timeBetweenSubWaves) {
            this.subWaves = subWaves;
            this.spawnRate = spawnRate;
            this.timeBetweenSubWaves = timeBetweenSubWaves;
        }

        //Boss Wave
        public Wave(SubWave bossSubWave) {
            subWaves.Add(bossSubWave);
            spawnRate = 0;
            timeBetweenSubWaves = 1;
        }

        public float GetSpawnRate()
        {
            return spawnRate;
        }

        public List<SubWave> GetSubWaves()
        {
            return subWaves;
        }

        public float GetTimeBetweenSubWaves()
        {
            return timeBetweenSubWaves;
        }
    }
}
