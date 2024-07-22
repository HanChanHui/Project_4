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
    private Camera mainCamera;

    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject, Camera mainCamera)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.mainCamera = mainCamera;

        gridObjectArray = new TGridObject[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                gridObjectArray[x, y] = createGridObject(this, gridPosition);
            }
        }

    }

   public Vector2 GetWorldPosition(GridPosition gridPosition)
    {
        Vector2 cameraCenter = mainCamera.transform.position;
        float gridWidth = width * cellSize;
        float gridHeight = height * cellSize;
        
        // 그리드의 중심을 카메라의 중심에 맞추기 위한 오프셋 계산
        float startX = cameraCenter.x - (gridWidth / 2) + (cellSize / 2);
        float startY = cameraCenter.y - (gridHeight / 2) + (cellSize / 2);
        
        return new Vector2(startX + gridPosition.x * cellSize, startY + gridPosition.y * cellSize);
    }

    public Vector2 GetObjectWorldPosition(GridPosition gridPosition)
    {
        //Debug.Log("gridPosition.x : " + gridPosition.x + ", " + "gridPosition.y : " + gridPosition.y);
        return new Vector3(gridPosition.x, gridPosition.y) * cellSize; 
    }

    public GridPosition GetGridPosition(Vector2 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.y / cellSize)
            );
    }

    public void CreateDebugObject(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                debugTransform.localScale = new Vector2(cellSize, cellSize);

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


}