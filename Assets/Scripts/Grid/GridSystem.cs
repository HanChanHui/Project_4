using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjectArray;
    private Vector3 startPosition;
    private LayerMask layerMask;

    public GridSystem(int width, int height, float cellSize, Vector3 startPosition, LayerMask layerMask,Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.startPosition = startPosition;
        this.layerMask = layerMask;

        gridObjectArray = new TGridObject[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                Vector3 worldPosition = GetWorldPosition(gridPosition);

                if (!Physics2D.Raycast(worldPosition, Vector2.down, 0.5f, layerMask))
                {
                    gridObjectArray[x, y] = createGridObject(this, gridPosition);
                }
                else 
                {
                    float heightAdjustment = 0.35f;
                    worldPosition += new Vector3(0, heightAdjustment, 0); // 원하는 높이로 조정
                    heightAdjustments[gridPosition] = heightAdjustment; // 높이 정보를 딕셔너리에 저장
                    gridObjectArray[x, y] = createGridObject(this, gridPosition);
                    // worldPosition += new Vector3(0, 0.35f, 0); // 원하는 높이로 조정
                    // GridPosition newGridPosition = GetGridPosition(worldPosition);
                    // Debug.Log(newGridPosition);
                    // gridObjectArray[x, y] = createGridObject(this, newGridPosition);
                }
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
         return new Vector3(gridPosition.x, gridPosition.y) * cellSize + startPosition;
    }

    private Dictionary<GridPosition, float> heightAdjustments = new Dictionary<GridPosition, float>();
    public Vector3 GetAdjustedWorldPosition(GridPosition gridPosition)
    {
        Vector3 worldPosition = GetWorldPosition(gridPosition);
        if (heightAdjustments.TryGetValue(gridPosition, out float heightAdjustment))
        {
            worldPosition += new Vector3(0, heightAdjustment, 0);
        }
        return worldPosition;
    }


    public GridPosition GetGridPosition(Vector3 worldPosition) 
    {
        int xPosition = Mathf.Clamp(Mathf.RoundToInt((worldPosition - startPosition).x / cellSize), 0, width - 1);
        int yPosition = Mathf.Clamp(Mathf.RoundToInt((worldPosition - startPosition).y / cellSize), 0, height - 1);

        return new GridPosition(xPosition, yPosition);
    }

    public void CreateDebugObject(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.y];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && 
               gridPosition.y >= 0 && 
               gridPosition.x < width && 
               gridPosition.y < height;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }
}
