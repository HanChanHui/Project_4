using System.Collections.Generic;
using UnityEngine;

namespace HornSpirit {
    public interface IPlaceable
    {
        int ID { get; }
        string Name { get; }
        Consts.GridRangeType gridRangeType { get; }
        GameObject Prefab { get; }
        GameObject IconPrefab { get; }
        int placeableCost { get; }
        bool GetGridPositionList(GridPosition gridPosition, out List<GridPosition> gridPositionList);
        bool GetSingleOneLayerGridPosition(GridPosition gridPosition);
    }
}
