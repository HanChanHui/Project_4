using System.Collections.Generic;

namespace HornSpirit {
    [System.Serializable]
    public class WaveInfoData 
    {
        public int waveNumber;
        public List<WaveTerm> waveTermList = new List<WaveTerm>();
       

        public WaveInfoData(List<WaveTerm> waveTermList, int waveNumber)
        {
            this.waveNumber = waveNumber;
            this.waveTermList = waveTermList;
        }
    }

    [System.Serializable]
    public class  WaveTerm
    {
        public int enemyId;
        public int enemySpawnMaxCount;
        public float Interval;

        public WaveTerm(int enemyId, int enemySpawnMaxCount, float Interval)
        {
            this.enemyId = enemyId;
            this.enemySpawnMaxCount = enemySpawnMaxCount;
            this.Interval = Interval;
        }
    }
}
