using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace HornSpirit {
    [CreateAssetMenu(fileName = "NewPlaceable", menuName = "Unity Royale/Placeable Block Data")]
    public class PlaceableBlockData : ScriptableObject, IPlaceable {
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

        public bool CanPlaceBlock(List<Transform> startPosition, Vector3 targetPosition) 
        {
            // 경로를 계산하여 경로가 있는지 확인
            foreach(Transform spawnerPosition in startPosition)
            {
                Path path = ABPath.Construct(spawnerPosition.position, targetPosition, null);
                AstarPath.StartPath(path);
                path.BlockUntilCalculated();

                if (path.vectorPath.Count > 0) {
                    // 경로의 마지막 지점
                    Vector3 lastPoint = path.vectorPath[path.vectorPath.Count - 1];

                    // 목표 지점과 마지막 지점 간의 거리
                    float distanceToTarget = Vector3.Distance(lastPoint, targetPosition);

                    // 거리 임계값 설정 (예: 0.5 유닛 이하이면 경로가 유효하다고 간주)
                    float threshold = 0.5f;

                    // 경로가 목표 지점에 도달했는지 확인
                    bool pathIsValid = distanceToTarget <= threshold;
                    if(!pathIsValid)
                    {
                        Debug.Log("경로가 없다");
                        return false;
                    }
                }
                else
                {
                    Debug.Log("경로가 없다");
                    return false;
                }
            }

            Debug.Log("경로가 존재");
            return true;
        }
        private Dictionary<string, Path> pathCache = new Dictionary<string, Path>();

        public Path GetOrCreatePath(Vector3 startPosition, Vector3 targetPosition) {
            string cacheKey = $"{startPosition}-{targetPosition}";
            if (pathCache.TryGetValue(cacheKey, out Path cachedPath)) {
                return cachedPath;
            }

            Path path = ABPath.Construct(startPosition, targetPosition, null);
            AstarPath.StartPath(path);
            path.BlockUntilCalculated();

            pathCache[cacheKey] = path;
            return path;
        }
        // public bool CanPlaceBlock(List<Transform> startPosition, Vector3 targetPosition) 
        // {
        //     Path path = GetOrCreatePath(startPosition[0].position, targetPosition);
        //     if (path.vectorPath.Count > 0) 
        //     {
        //         Vector3 lastPoint = path.vectorPath[path.vectorPath.Count - 1];
        //         float distanceToTarget = Vector3.Distance(lastPoint, targetPosition);
        //         float threshold = 0.5f;

        //         bool pathIsValid = distanceToTarget <= threshold;

        //         Debug.Log("경로 유효성: " + pathIsValid);
        //         return pathIsValid;
        //     }

        //     Debug.Log("경로가 존재하지 않음");
        //     return false;
        // }

        // public bool CanPlaceBlock(List<Transform> startPosition, Vector3 targetPosition) 
        // {
        //     Path path = ABPath.Construct(startPosition[0].position, targetPosition, null);
        //     AstarPath.StartPath(path);
        //     path.BlockUntilCalculated();

        //     // 경로의 유효성을 검증
        //     if (path.vectorPath.Count > 0) {
        //         // 경로의 마지막 지점
        //         Vector3 lastPoint = path.vectorPath[path.vectorPath.Count - 1];

        //         // 목표 지점과 마지막 지점 간의 거리
        //         float distanceToTarget = Vector3.Distance(lastPoint, targetPosition);

        //         // 거리 임계값 설정 (예: 0.5 유닛 이하이면 경로가 유효하다고 간주)
        //         float threshold = 0.5f;

        //         // 경로가 목표 지점에 도달했는지 확인
        //         bool pathIsValid = distanceToTarget <= threshold;

        //         Debug.Log("경로 유효성: " + pathIsValid);
        //         return pathIsValid;
        //     }

        //     Debug.Log("경로가 존재하지 않음");
        //     return false;
        // }
    }
}
