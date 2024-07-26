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

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one CharacterActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystem = new GridSystem<GridObject>(width, height, cellSize, startPosition, layerMask, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
        //gridSystem.CreateGridAboveObjects((GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
        //gridSystem.CreateDebugObject(gridDebugObjectPrefab);
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



}