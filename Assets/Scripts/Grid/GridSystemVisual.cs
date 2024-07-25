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
        Green,
        Red,
    }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    [SerializeField] private LevelGrid levelGrid;
    [SerializeField] private Transform gridSystemVisualSingPrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (levelGrid == null)
        {
            levelGrid = GameObject.Find("LevelGrid").GetComponent<LevelGrid>();

            if (levelGrid == null)
            {
                Debug.LogError("LevelGrid 오브젝트를 찾을 수 없습니다. LevelGrid를 할당해주세요.");
            }
        }

        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            levelGrid.GetWidth(),
            levelGrid.GetHeight()
            ];

        for (int x = 0; x < levelGrid.GetWidth(); x++)
        {
            for (int y = 0; y < levelGrid.GetHeight(); y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSingPrefab, levelGrid.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualSingleTransform.transform.parent = transform;
                gridSystemVisualSingleArray[x, y] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        List<GridPosition> gridPositionList = new List<GridPosition>();
        GridPosition StartPosition = new GridPosition(0, 0);
        for (int y = 0; y < levelGrid.GetHeight(); y++)
        {
            for(int x = 0; x < levelGrid.GetWidth(); x++)
            {
                GridPosition testGridPosition = StartPosition  + new GridPosition(x, y);
                gridPositionList.Add(testGridPosition);
            }
        }
        ShowGridPositionList(gridPositionList, GridVisualType.White);

    }

    public void HideAllGridPosition()
    {
        for (int x = 0; x < levelGrid.GetWidth(); x++)
        {
            for (int y = 0; y < levelGrid.GetHeight(); y++)
            {
                gridSystemVisualSingleArray[x, y].Hide();
            }
        }
    }
    public void ShowAllGridPosition()
    {
        for (int x = 0; x < levelGrid.GetWidth(); x++)
        {
            for (int y = 0; y < levelGrid.GetHeight(); y++)
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

                if (!levelGrid.IsValidGridPosition(testGridPosition))
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

    private void ShowTowerGridPositionRange(List<GridPosition> gridPosition, GridVisualType gridVisualType)
    {
        ShowGridPositionList(gridPosition, gridVisualType);
    }

    public void ShowGridPositionRange(GridPosition gridPosition, int width, int height)
    {
        List<GridPosition> greedGridList = new List<GridPosition>();
        List<GridPosition> redGridList = new List<GridPosition>();

        for(int x = 0; x <= width; x++)
        {
            for(int y = 0; y <= height; y++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, y);

                if (!levelGrid.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if(!levelGrid.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    redGridList.Add(testGridPosition);
                    continue;
                }

                greedGridList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(greedGridList, GridVisualType.Green);
        ShowGridPositionList(redGridList, GridVisualType.Red);
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
        for (int x = 0; x < levelGrid.GetWidth(); x++)
        {
            for (int y = 0; y < levelGrid.GetHeight(); y++)
            {
                Destroy(gridSystemVisualSingleArray[x, y].gameObject);
            }
        }
    }

    private void UpdateGridVisual()
    {
        //Unit selectedUnit = UnitActionSystem.Instance.GetSelecterdUnit();

        GridVisualType gridVisualType = GridVisualType.White;

        switch (gridVisualType)
        {
            case GridVisualType.White:
                             
                break;
            case GridVisualType.Green:
                
                break;
            case GridVisualType.Red:
                
                break;
        }

        //ShowGridPositionList(, gridVisualType);
    }

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
