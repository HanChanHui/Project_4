using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private LayerMask layerMask;

    private GridSystem<GridObject> gridSystem;
    private GridSystem<GridObject> gridSystem2;
    private List<GridPosition> layer2GridPositions;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one CharacterActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //layer2GridPositions = new List<GridPosition>();
        gridSystem = new GridSystem<GridObject>(width, height, cellSize, startPosition, layerMask, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
        
        //InitializeTowersOnGrid();

        //if (layer2GridPositions.Count > 0) {
        //    InitializeLayer2GridSystem();
        //}
        //gridSystem.CreateDebugObject(gridDebugObjectPrefab);
    }

    private void InitializeTowersOnGrid() {
        foreach (var gridPosition in gridSystem.GetAllGridPositions()) 
        {
            Vector3 worldPosition = gridSystem.GetWorldPosition(gridPosition);
            Collider2D collider = Physics2D.OverlapCircle(worldPosition, cellSize / 2, layerMask);

            if (collider != null) {
                Tower tower = collider.GetComponent<Tower>();
                if (tower != null) 
                {
                    Debug.Log(tower);
                    //AddUnitAtGridPosition(gridPosition, tower);
                    layer2GridPositions.Add(gridPosition);
                }
            }
        }
    }
    private void InitializeLayer2GridSystem() {
        Vector3 startPosition2 = startPosition + new Vector3(0, 0.35f, 0);

        gridSystem2 = new GridSystem<GridObject>(layer2GridPositions, cellSize, startPosition2, layerMask, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Tower unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        if (!gridObject.HasAnyUnit())
        {
            gridObject.AddUnit(unit);
        }
    }

    public List<Tower> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Tower unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        if (gridObject.HasAnyUnit())
        {
            gridObject.RemoveUnit(unit);
        }
    }

    public void UnitMovedGridPosition(Tower unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);

        AddUnitAtGridPosition(toGridPosition, unit);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    public Vector3 GetWorldPosition2(GridPosition gridPosition) => gridSystem2.GetWorldPosition(gridPosition);

    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    public float GetCellSize() => gridSystem.GetCellSize();

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        if(gridObject == null) return false;
        return gridObject.HasAnyUnit();
    }

    public Tower GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        if(gridObject == null) return null;
        return gridObject.GetUnit();
    }

    public GridSystem<GridObject> GetGridSystem()
    {
        return gridSystem;
    }

    public GridSystem<GridObject> GetGridSystem2() {
        return gridSystem2;
    }

    public List<GridPosition> GetLayer2GridPosition() 
    {
        return layer2GridPositions;
    }

}
