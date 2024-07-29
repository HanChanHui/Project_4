using Consts;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TowerObject : ScriptableObject
{
    [Header("Tower")]
    public int towerID;
    public string towerName;
    public TowerType towerType;

    [Header("Tower Battle")]
    public float towerHP;
    public float towerDefence;
    public TowerAttackType towerAttackType;
    public float towerAttack;
    public float towerAttackSpeed;
    public float attackRange;

    [Header("Tower Cost")]
    public int towerCost;
    public int towerRecoveryCost;
    public int towerRecoveryCostIncrease;

    [Header("Tower Layout")]
    public Transform prefab;
    public Transform icon;
    public int width;
    public int height;

    public List<GridPosition> GetGridPositionList(GridPosition gridPosition)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GridPosition testGridPosition = new GridPosition(x, y) + gridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) 
                {
                    return null;
                }

                gridPositionList.Add(testGridPosition);
            }
        }
        return gridPositionList;
    }

}
