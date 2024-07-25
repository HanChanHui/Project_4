using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;
    private List<Tower> unitList;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitList = new List<Tower>();
    }

    public override string ToString()
    {
        string unitString = "";
        foreach (Tower unit in unitList)
        {
            unitString += unit + "\n";
        }

        return unitString;
    }

    public void AddUnit(Tower unit)
    {
        unitList.Add(unit);
    }

    public void RemoveUnit(Tower unit)
    {
        unitList.Remove(unit);
    }

    public List<Tower> GetUnitList()
    {
        return unitList;
    }

    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }


    public Tower GetUnit()
    {
        if(HasAnyUnit())
        {
            return unitList[0];
        }
        else
        {
            return null;
        }
    }
}
