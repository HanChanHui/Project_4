using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridSystemVisual;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    public enum GridVisualType
    {
        Empty,
        White,
    }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    [SerializeField] private Transform gridSystemVisualSingPrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one CharacterActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
            ];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int y = 0; y < LevelGrid.Instance.GetHeight(); y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSingPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualSingleTransform.transform.parent = transform;
                gridSystemVisualSingleArray[x, y] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        List<GridPosition> gridPositionList = new List<GridPosition>();
        GridPosition StartPosition = new GridPosition(0, 0);
        for (int y = 0; y < LevelGrid.Instance.GetHeight(); y++)
        {
            for(int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
            {
                GridPosition testGridPosition = StartPosition  + new GridPosition(x, y);
                gridPositionList.Add(testGridPosition);
            }
        }
        ShowGridPositionList(gridPositionList, GridVisualType.White);

    }

    public void HideAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int y = 0; y < LevelGrid.Instance.GetHeight(); y++)
            {
                gridSystemVisualSingleArray[x, y].Hide();
            }
        }
    }
    public void ShowAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int y = 0; y < LevelGrid.Instance.GetHeight(); y++)
            {
                gridSystemVisualSingleArray[x, y].Show(GetGridVisualMaterial());
            }
        }
    }

    private void ShowSingleGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, y);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if(gridPosition != testGridPosition)
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for(int x = -range; x <= range; x++)
        {
            for(int y = -range; y <= range; y++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, y);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);
                if (testDistance > range)
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach(GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.y].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    public void DestroyGridPositionList()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int y = 0; y < LevelGrid.Instance.GetHeight(); y++)
            {
                Destroy(gridSystemVisualSingleArray[x, y].gameObject);
            }
        }
    }

    // private void UpdateGridVisual()
    // {
    //     HideAllGridPosition();

    //     Unit selectedUnit = UnitActionSystem.Instance.GetSelecterdUnit();
    //     BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

    //     GridVisualType gridVisualType;

    //     switch (selectedAction)
    //     {
    //         default:
    //         case MoveAction moveAction:
    //             gridVisualType = GridVisualType.White;              
    //             break;
    //         case EmptyAction EmptyAction:
    //             gridVisualType = GridVisualType.Empty;

    //             break;
    //     }
    // }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach(GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }

    private Material GetGridVisualMaterial()
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            return gridVisualTypeMaterial.material;
        }

        
        return null;
    }

}
