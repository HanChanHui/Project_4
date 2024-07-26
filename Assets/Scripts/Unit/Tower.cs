using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    private List<GridPosition> gridPosition = new List<GridPosition>();
    public List<GridPosition> GridPosition { get{ return gridPosition; } set{ gridPosition = value; }}

    private void Start() {
        
    }

    
}
