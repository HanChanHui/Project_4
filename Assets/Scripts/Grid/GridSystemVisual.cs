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
    [SerializeField] private LayerMask layerMask;
    private List<GridPosition> placeGridPositionList = new List<GridPosition>();

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private string layerName = "Grid";

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
                Vector2 worldPosition = levelGrid.GetWorldPosition(gridPosition);
                if(!Physics2D.Raycast(worldPosition, Vector2.down, 0.5f, layerMask))
                {
                    Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSingPrefab, worldPosition, Quaternion.identity);
                    gridSystemVisualSingleTransform.transform.parent = transform;
                    gridSystemVisualSingleArray[x, y] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                }
                else
                {
                    worldPosition.y += 0.5f; // 원하는 높이로 조정
                    Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSingPrefab, worldPosition, Quaternion.identity);
                    gridSystemVisualSingleTransform.transform.parent = transform;
                    gridSystemVisualSingleArray[x, y] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                }
                Debug.Log(worldPosition + " " + gridPosition);
               
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
        ShowGridPositionList(gridPositionList, layerName, GridVisualType.White);
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

    public void ShowTowerGridPositionRange(GridPosition gridPosition, int width, int height)
    {
        List<GridPosition> greedGridList = new List<GridPosition>();
        List<GridPosition> redGridList = new List<GridPosition>();
        ShowBeforeTowerGridVisual();

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, y);

                if (!levelGrid.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if(levelGrid.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    redGridList.Add(testGridPosition);
                    continue;
                }

                greedGridList.Add(testGridPosition);
            }
        }
        placeGridPositionList.AddRange(redGridList);
        placeGridPositionList.AddRange(greedGridList);
        layerName = "PlaceGrid";
        ShowGridPositionList(greedGridList, layerName, GridVisualType.Green);
        ShowGridPositionList(redGridList, layerName, GridVisualType.Red);
    }

    public void ShowBeforeTowerGridVisual()
    {
        if(placeGridPositionList.Count > 0)
        {
            layerName = "Grid";
            ShowGridPositionList(placeGridPositionList, layerName, GridVisualType.White);
            placeGridPositionList.Clear();
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, string layerName, GridVisualType gridVisualType)
    {
        Material material = GetGridVisualTypeMaterial(gridVisualType);
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.y].Show(material);
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.y].GridLayerChange(layerName);
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
