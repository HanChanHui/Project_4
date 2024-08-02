using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TankerTower : Tower
{

    protected override void Start() {
        base.Start();
        //AstarPath.active.Scan();
    }

    protected override IEnumerator CoCheckDistance()
    {
        while (true)
        {
            if (enemiesInRange.Count > 0)
            {
                CoAttack();
                yield return new WaitForSeconds(attackSpeed);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CoAttack()
    {
        foreach (Enemy enemy in enemiesInRange)
        {
            if(enemy != null)
            {
                if (Vector2.Distance(transform.position, enemy.transform.position) <= maxDistance) {
                    AttackEnemy(enemy);
                }
            }
        }
    }

     public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (health <= 0)
        {
           foreach(GridPosition gridPosition in gridPositionList)
           {
                LevelGrid.Instance.RemoveTowerAtGridPosition(gridPosition, this);
           }
        }
    }

    private void AttackEnemy(Enemy enemy)
    {
        enemy.TakeDamage(attackDamage); // 적에게 데미지를 입힘
    }

    private void OnDisable() {
        StopCoroutine(CoCheckDistance());
    }
}
