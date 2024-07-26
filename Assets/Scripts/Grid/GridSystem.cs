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
                if (!IsObjectAtGridPosition(gridPosition))
                {
                    gridObjectArray[x, y] = createGridObject(this, gridPosition);
                }
                else
                {
                    Vector3 worldPosition = GetWorldPosition(gridPosition);
                    worldPosition.y += 0.5f; // 원하는 높이로 조정
                    GridPosition newGridPosition = GetGridPosition(worldPosition);
                    Debug.Log(worldPosition + " " + newGridPosition);
                    if (IsValidGridPosition(newGridPosition))
                    {
                        gridObjectArray[x, y] = createGridObject(this, newGridPosition);
                    }
                }
            }
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

    public bool IsObjectAtGridPosition(GridPosition gridPosition)
    {
        Vector3 worldPosition = GetWorldPosition(gridPosition);
        return Physics2D.Raycast(worldPosition, Vector2.down, 0.5f, layerMask);
    }

    public void CreateGridAboveObjects(Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                if (IsObjectAtGridPosition(gridPosition))
                {
                    
                    Vector3 worldPosition = GetWorldPosition(gridPosition);
                    worldPosition.y += 0.5f; // 원하는 높이로 조정
                    GridPosition newGridPosition = GetGridPosition(worldPosition);
                    Debug.Log(worldPosition + " " + newGridPosition);
                    if (IsValidGridPosition(newGridPosition))
                    {
                        gridObjectArray[newGridPosition.x, newGridPosition.y] = createGridObject(this, newGridPosition);
                    }
                }
            }
        }
    }

}