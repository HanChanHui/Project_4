using Consts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DealerNeco_1 : DealerTower
{
    private GridPosition gridPosition;
    private List<GridPosition> atkRangeGridList;
    private AttackDirection atkDirection;

    public JoystickController joystickController;


    protected override void Start()
    {
        base.Start();

        atkDirection = AttackDirection.Left;
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        GenerateAttackPattern(atkDirection);

        joystickController.AttackDirectionSelected += OnAttackDirectionSelected;
    }

    void OnAttackDirectionSelected(Vector2 direction)
    {
        // 공격 방향을 설정하고 행동을 수행
        Debug.Log("Tower attacks in direction: " + direction);
    }

    private IEnumerator CoCheckAttackRange()
    {
        while (true)
        {
            FindEnemy();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void GenerateAttackPattern(AttackDirection direction)
    {
        atkRangeGridList = new List<GridPosition>();

        Vector2Int directionVector = GetDirectionVector(direction);
        for (int i = 1; i <= maxDistance; i++)
        {
            GridPosition attackGridPosition = gridPosition + new GridPosition(directionVector.x * i, directionVector.y * i);
            atkRangeGridList.Add(attackGridPosition);
        }

        FilterInvalidGridPositions();
        StartCoroutine(CoCheckAttackRange());
    }

    private Vector2Int GetDirectionVector(AttackDirection direction)
    {
        switch (direction)
        {
            case AttackDirection.Up: return Vector2Int.up;
            case AttackDirection.Down: return Vector2Int.down;
            case AttackDirection.Left: return Vector2Int.left;
            case AttackDirection.Right: return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }

    private void FilterInvalidGridPositions()
    {
        atkRangeGridList.RemoveAll(gridPos =>
            !LevelGrid.Instance.IsValidGridPosition(gridPos) ||
            LevelGrid.Instance.HasAnyBlockOnGridPosition(gridPos) ||
            LevelGrid.Instance.HasAnyTowerOnGridPosition(gridPos)
        );
    }

    private void FindEnemy()
    {
        foreach (GridPosition gridPos in atkRangeGridList)
        {
            BaseEnemy enemy = LevelGrid.Instance.GetEnemiesAtGridPosition(gridPos);
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    private void OnDisable() 
    {
        joystickController.AttackDirectionSelected -= OnAttackDirectionSelected;
    }

}
