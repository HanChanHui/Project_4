using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    [SerializeField] private int width;
    [SerializeField] private int height;
    public int Width {get {return width;}}
    public int Height {get {return height;}}
    private List<GridPosition> gridPosition = new List<GridPosition>();
    public List<GridPosition> GridPosition { get{ return gridPosition; } set{ gridPosition = value; }}

    private void Start() {
        
    }

    
}
