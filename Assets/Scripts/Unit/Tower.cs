using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class Tower : MonoBehaviour
{
    SpriteRenderer sprite;

    private List<GridPosition> gridPositionList = new List<GridPosition>();
    public List<GridPosition> GridPositionList { get{ return gridPositionList; } set{ gridPositionList = value; }}

    [SerializeField] private int maxDistance;

    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start() 
    {
        if(gridPositionList != null)
        {
            foreach(GridPosition pos in gridPositionList)
            {
                if(pos.y - 1 < 0)
                {
                    continue;
                }
                GridPosition position = new GridPosition(pos.x, pos.y - 1);
                if(LevelGrid.Instance.HasAnyBlockOnGridPosition(position))
                {
                    sprite.sortingLayerName = Consts.LayerName.BackTower.ToString();
                }
            }
        }

        StartCoroutine(CoCheckDistance());
    }

    private IEnumerator CoCheckDistance()
    {
        while(true)
        {

            if(CheckDistance())
            {
                CoAttack();
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool CheckDistance()
    {
        bool isCheck = false;
        for(int x = -maxDistance; x <= maxDistance; x++)
        {
            for(int y = -maxDistance; y <= maxDistance; y++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, y);
                GridPosition testGridPosition = gridPositionList[0] + offsetGridPosition;

                if(LevelGrid.Instance.HasAnyEnemyOnGridPosition(testGridPosition))
                {
                    isCheck = true;
                    break;
                }

                isCheck = false;
            }
        }

        return isCheck;
    }

    private IEnumerator CoAttack()
    {
        Debug.Log("공격");
        yield return null;
    }



}
