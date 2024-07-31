using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class Tower : MonoBehaviour
{
    SpriteRenderer sprite;

    private List<GridPosition> gridPositionList = new List<GridPosition>();
    public List<GridPosition> GridPositionList { get{ return gridPositionList; } set{ gridPositionList = value; }}
    [SerializeField] private List<Enemy> enemiesInRange = new List<Enemy>();
    public List<Enemy> EnemiesInRange { get { return enemiesInRange; } }


    [SerializeField] private int maxDistance;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shooterPos;

    private void Awake() 
    {
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

        LevelGrid.Instance.OnEnemyEnteredGridPosition += OnEnemyEnteredGridPosition;
        LevelGrid.Instance.OnEnemyExitedGridPosition += OnEnemyExitedGridPosition;
        Enemy.OnEnemyDestroyed += OnEnemyDestroyed;

        StartCoroutine(CoCheckDistance());
    }

    private void OnEnemyEnteredGridPosition(Enemy enemy, GridPosition gridPosition)
    {
        if (IsWithinRange(gridPosition))
        {
            if (!enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    private void OnEnemyExitedGridPosition(Enemy enemy, GridPosition gridPosition)
    {
        if (enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
        }
    }

    private void OnEnemyDestroyed(Enemy enemy)
    {
        if (enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
        }
    }

    private bool IsWithinRange(GridPosition gridPosition)
    {
        foreach (GridPosition towerPosition in gridPositionList)
        {
            int distance = Mathf.Abs(gridPosition.x - towerPosition.x) + Mathf.Abs(gridPosition.y - towerPosition.y);
            if (distance <= maxDistance)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator CoCheckDistance()
    {
        while(true)
        {

            if(enemiesInRange.Count > 0)
            {
                CoAttack();
                yield return new WaitForSeconds(attackSpeed);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }


    private void CoAttack()
    {
        if (enemiesInRange.Count > 0)
        {
            Enemy targetEnemy = enemiesInRange[0];
            ShootBullet(targetEnemy);
        }
    }

    private void ShootBullet(Enemy target)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, shooterPos.position, Quaternion.identity);
        Bullet bullet = bulletInstance.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetEnemy(target);
            bullet.Damage = attackDamage; // 설정한 데미지 값
        }
    }

    private void OnDestroy()
    {
        LevelGrid.Instance.OnEnemyEnteredGridPosition -= OnEnemyEnteredGridPosition;
        LevelGrid.Instance.OnEnemyExitedGridPosition -= OnEnemyExitedGridPosition;
        Enemy.OnEnemyDestroyed -= OnEnemyDestroyed;
    }
}
