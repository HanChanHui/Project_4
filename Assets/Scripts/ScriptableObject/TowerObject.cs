using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TowerObject : ScriptableObject
{
    public string nameString;
    public Transform prefab;
    public int width;
    public int height;

    public List<Vector2> GetGridPositionList(Vector2 offset)
    {
        List<Vector2> gridPositionList = new List<Vector2>();
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                gridPositionList.Add(offset + new Vector2(x, y));
            }
        }
        return gridPositionList;
    }

}
