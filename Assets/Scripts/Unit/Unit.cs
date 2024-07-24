using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    [SerializeField] private int width;
    [SerializeField] private int height;
    public int Width {get {return width;}}
    public int Height {get {return height;}}
    public List<GridPosition> gridPosition = new List<GridPosition>();

    void Start()
    {
        //gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        //LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
    }


    public List<GridPosition> GetGridPositionList(Vector2 offset)
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GridPosition pos = LevelGrid.Instance.GetGridPosition(offset + new Vector2(x, y));
                gridPosition.Add(pos);
            }
        }
        return gridPosition;
    }

    
}
