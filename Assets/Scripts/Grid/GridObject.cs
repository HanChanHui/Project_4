using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;
    private List<Tower> unitList;
    private List<Block> blockList;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitList = new List<Tower>();
        blockList = new List<Block>();
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
    public void AddBlock(Block block)
    {
        blockList.Add(block);
    }

    public void RemoveUnit(Tower unit)
    {
        unitList.Remove(unit);
    }
    public void RemoveBlock(Block block)
    {
        blockList.Remove(block);
    }

    public List<Tower> GetUnitList()
    {
        return unitList;
    }

    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }

    public bool HasAnyBlock()
    {
        return blockList.Count > 0;
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

    public Block GetBlock()
    {
        if(HasAnyBlock())
        {
            return blockList[0];
        }
        else
        {
            return null;
        }
    }
}
