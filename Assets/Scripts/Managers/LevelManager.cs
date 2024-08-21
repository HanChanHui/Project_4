using Consts;
using UnityEngine;
using System.IO;
using System.Linq;

namespace HornSpirit {
    public class LevelManager : MonoBehaviour 
    {
        [SerializeField] private LevelData levelData;
        [SerializeField] private LevelGrid levelGrid;
        [SerializeField] private GridSystemVisual gridVisual;
        [SerializeField] private AstarPath astarPath;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject backGroundOj;
        [SerializeField] private string jsonFileName;
        [SerializeField] private int levelId;

        void Awake() 
        {
            LoadLevelData(1000 + (levelId <= 0 ? 1 : levelId)); // 예를 들어 ID가 1001인 레벨을 로드
        }

        private void Start() 
        {
            if(levelData != null)
            {
                levelGrid.Init(levelData.x, levelData.y, 2);
                gridVisual.Init(levelData.x, levelData.y);
                AdjustCameraToGrid(levelData.x, levelData.y, 2);
                BackgroundScaler();
                UpdateAstarGraph(levelData.x, levelData.y);
                BlockDeployment();
                astarPath.Scan();
            }
        }
        // Json 읽기
        private void LoadLevelData(int levelId) {
            string filePath = System.IO.Path.Combine(Application.dataPath + $"/Resources/JSON/{jsonFileName}.json");
            if (File.Exists(filePath)) 
            {
                string jsonData = File.ReadAllText(filePath);
                LevelMap levelMap = JsonUtility.FromJson<LevelMap>(jsonData);
                levelData = levelMap.Level.FirstOrDefault(level => level.id == levelId);
            } 
            else 
            {
                Debug.LogError("JSON file not found at " + filePath);
            }
        }
        // Block 배치
        private void BlockDeployment()
        {
            foreach(BlockData block in levelData.BlockInfoList)
            {
                GridPosition gridPosition = new GridPosition(block.x, block.y);
                SelectBlockType((BlockType)block.blockType, levelGrid.GetWorldPosition(gridPosition));
            }
        }
        // 설치할 Block Type 설정
        private void SelectBlockType(BlockType blockType, Vector3 gridPosition) 
        {
            GameObject prefab = null;

            switch (blockType) 
            {
                case BlockType.Block:
                    prefab = Resources.Load<GameObject>("Block/" + BlockType.Block.ToString());
                    break;
                case BlockType.BenBlock:
                    prefab = Resources.Load<GameObject>("Block/" + BlockType.BenBlock.ToString());
                    break;
                case BlockType.TargetBlock:
                    prefab = Resources.Load<GameObject>("Block/" + BlockType.TargetBlock.ToString());
                    break;
                case BlockType.SpawnerBlock:
                    prefab = Resources.Load<GameObject>("Block/" + BlockType.SpawnerBlock.ToString());
                    break;
                case BlockType.SpawnerTwoLayerBlock:
                    prefab = Resources.Load<GameObject>("Block/" + BlockType.SpawnerTwoLayerBlock.ToString());
                    break;
            }

            if (prefab != null) 
            {
                Instantiate(prefab, gridPosition, Quaternion.identity);
            } 
            else 
            {
                Debug.LogError("Prefab not found for BlockType: " + blockType);
            }

        }
        // Astar 크기 조절
        private void UpdateAstarGraph(int width, int depth)
        {
            var graph = astarPath.data.gridGraph;

            if (graph != null)
            {
                graph.SetDimensions(width, depth, 2);
                graph.Scan();
            }
            else 
            {
                Debug.LogError("GridGraph not found");
            }
        }

        // 카메라 줌(확대/축소)
        void AdjustCameraToGrid(int width, int height, int cellSize) 
        {
            float totalWidth = width * cellSize;
            float totalHeight = height * cellSize;
            Vector3 center = new Vector3(totalWidth / 2, totalHeight / 2, 0);

            Bounds gridBounds = new Bounds(center, new Vector3(totalWidth, totalHeight, 0));

            float gridHeight = gridBounds.size.y;

            float cameraSizeBasedOnHeight = gridHeight / 1.3f;

            mainCamera.orthographicSize = cameraSizeBasedOnHeight;
        }
        // 배경 크기 조절
        private void BackgroundScaler() 
        {
            SpriteRenderer sr = backGroundOj.GetComponent<SpriteRenderer>();
            Debug.Log(sr);
            //if (sr == null) return;

            float width = sr.sprite.bounds.size.x;
            float height = sr.sprite.bounds.size.y;

            float worldScreenHeight = mainCamera.orthographicSize * 2.0f;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            backGroundOj.transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);

            backGroundOj.AddComponent<BoxCollider2D>();
        }
    }
}
