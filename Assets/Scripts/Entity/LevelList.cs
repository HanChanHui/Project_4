
namespace HornSpirit {
    [System.Serializable]
    public class LevelList : SpreadToJSON
    {
        public LevelData[] Level;

        public override void ParseAndSave(string sheet, string fileName)
        {
            LevelList levelList = new() {
                Level = ParseJSON(column => {
                    LevelData level = new();
                    ReadData(column, level);
                    return level;
                }, sheet)
            };

            SaveJSONFile(levelList, fileName);
        }

        public void ReadData(string[] column, LevelData level) 
        {
            level.id = int.Parse(column[1]);
            level.stageType = int.Parse(column[2]);
            level.x = int.Parse(column[3]);
            level.y = int.Parse(column[4]);

            for(int i = 5; i < column.Length - 1; i++)
            {
                if(column[i] != "")
                {
                    string[] blockInfo = column[i].Split(',');
                    BlockData blockList = new BlockData(int.Parse(blockInfo[0]),
                                                    int.Parse(blockInfo[1]),
                                                    int.Parse(blockInfo[2]));
                    level.BlockInfoList.Add(blockList);
                }
                
            }
        }
    }
}
