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
                    worldPosition += new Vector3(0, 0.35f, 0); // 원하는 높이로 조정
                    GridPosition newGridPosition = GetGridPosition(worldPosition);
                    Debug.Log(newGridPosition);
                    gridObjectArray[x, y] = createGridObject(this, newGridPosition);
                }
            }
        }
    }

    public GridSystem(List<GridPosition> specificPositions, float cellSize, Vector3 startPosition, LayerMask layerMask, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.cellSize = cellSize;
        this.startPosition = startPosition;
        this.layerMask = layerMask;

        // **특정 위치들을 기반으로 width와 height 계산**
        int maxX = 0;
        int maxY = 0;

        foreach (var position in specificPositions)
        {
            if (position.x > maxX) maxX = position.x;
            if (position.y > maxY) maxY = position.y;
        }

        this.width = maxX + 1;
        this.height = maxY + 1;

        gridObjectArray = new TGridObject[width, height];

        foreach (var gridPosition in specificPositions)
        {
            Debug.Log(gridPosition.x + " " + gridPosition.y);
            gridObjectArray[gridPosition.x, gridPosition.y] = createGridObject(this, gridPosition);
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
         return new Vector3(gridPosition.x, gridPosition.y) * cellSize + startPosition;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) 
    {
        int xPosition = Mathf.Clamp(Mathf.RoundToInt((worldPosition - startPosition).x / cellSize), 0, width - 1);
        int yPosition = Mathf.Clamp(Mathf.RoundToInt((worldPosition - startPosition).y / cellSize), 0, height - 1);

        return new GridPosition(xPosition, yPosition);
    }

    public IEnumerable<GridPosition> GetAllGridPositions() 
    {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                yield return new GridPosition(x, y);
            }
        }
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
