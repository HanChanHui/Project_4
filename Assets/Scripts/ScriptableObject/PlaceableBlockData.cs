using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace HornSpirit {
    [CreateAssetMenu(fileName = "NewPlaceable", menuName = "Unity Royale/Placeable Block Data")]
    public class PlaceableBlockData : ScriptableObject, IPlaceable 
    {
        public string Name => blockName;
        public int ID => blockID;
        public GameObject Prefab => blockPrefab;
        public GameObject IconPrefab => blockSpritePrefab;
        public Consts.GridRangeType gridRangeType => blockGridRangeType;
        public int placeableCost => blockCost;

        [Header("Block Specific")]
        public string blockName;
        public int blockID;
        public Consts.GridRangeType blockGridRangeType;
        public GameObject blockPrefab;
        public GameObject blockSpritePrefab;

        [Header("Tower Cost")]
        public int blockCost;

        [Header("Block Layout")]
        public int width;
        public int height;
        

        public bool GetGridPositionList(GridPosition gridPosition, out List<GridPosition> gridPositionList) {
            gridPositionList = new List<GridPosition>();
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    GridPosition testGridPosition = new GridPosition(x, y) + gridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                        return false;
                    }

                    if (LevelGrid.Instance.HasAnyTowerOnGridPosition(testGridPosition)
                      || LevelGrid.Instance.HasAnyBlockOnGridPosition(testGridPosition)) {
                        return false;
                    }

                    gridPositionList.Add(testGridPosition);
                }
            }
            return true;
        }

        public bool GetSingleOneLayerGridPosition(GridPosition gridPosition) {
            if (!LevelGrid.Instance.IsValidGridPosition(gridPosition)) {
                return false;
            }

            if (LevelGrid.Instance.HasAnyTowerOnGridPosition(gridPosition)
                || LevelGrid.Instance.HasAnyBlockOnGridPosition(gridPosition)) {
                return false;
            }

            return true;
        }

        public bool CanPlaceBlock(Vector3 blockPosition, Vector3 startPosition, Vector3 targetPosition) {
            // 블록 설치 후의 가상 경로 계산
            Block tempBlock = Instantiate(blockPrefab, blockPosition, Quaternion.identity).GetComponent<Block>();
            AstarPath.active.Scan();

            // 경로가 유효한지 검사
            Path path = ABPath.Construct(startPosition, targetPosition, null);
            AstarPath.StartPath(path);
            path.BlockUntilCalculated();

            bool pathIsValid = path.error == false;

            // 임시 블록 제거
            Destroy(tempBlock);

            return pathIsValid;
        }


    }
}
