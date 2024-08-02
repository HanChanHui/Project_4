using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    protected SpriteRenderer sprite;

    protected List<GridPosition> gridPositionList = new List<GridPosition>();
    public List<GridPosition> GridPositionList { get { return gridPositionList; } set { gridPositionList = value; } }
    [SerializeField] protected List<Enemy> enemiesInRange = new List<Enemy>();
    public List<Enemy> EnemiesInRange { get { return enemiesInRange; } }

    [SerializeField] protected int maxDistance;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float health = 100f;

    protected virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        if (gridPositionList != null)
        {
            foreach (GridPosition pos in gridPositionList)
            {
                if (pos.y - 1 < 0 || LevelGrid.Instance.HasAnyBlockOnGridPosition(pos))
                {
                    continue;
                }
                GridPosition position = new GridPosition(pos.x, pos.y - 1);
                if (LevelGrid.Instance.HasAnyBlockOnGridPosition(position))
                {
                    sprite.sortingLayerName = Consts.LayerName.BackObject.ToString();
                }
            }
        }

        LevelGrid.Instance.OnEnemyEnteredGridPosition += OnEnemyEnteredGridPosition;
        LevelGrid.Instance.OnEnemyExitedGridPosition += OnEnemyExitedGridPosition;
        Enemy.OnEnemyDestroyed += OnEnemyDestroyed;

        StartCoroutine(CoCheckDistance());
    }

    protected void OnEnemyEnteredGridPosition(Enemy enemy, GridPosition gridPosition)
    {
        if (IsWithinRange(gridPosition))
        {
            if (!enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    protected void OnEnemyExitedGridPosition(Enemy enemy, GridPosition gridPosition)
    {
        if(!IsWithinRange(gridPosition))
        {
            if (enemiesInRange.Contains(enemy)) 
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }

    protected void OnEnemyDestroyed(Enemy enemy)
    {
        if (enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
        }
    }

    protected bool IsWithinRange(GridPosition gridPosition)
    {
        foreach (GridPosition towerPosition in gridPositionList)
        {
            int dx = Mathf.Abs(gridPosition.x - towerPosition.x);
            int dy = Mathf.Abs(gridPosition.y - towerPosition.y);
            int distance = Mathf.Max(dx, dy);
            if (distance <= maxDistance)
            {
                return true;
            }
        }
        return false;
    }

    protected virtual IEnumerator CoCheckDistance()
    {
        yield return null;
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        LevelGrid.Instance.OnEnemyEnteredGridPosition -= OnEnemyEnteredGridPosition;
        LevelGrid.Instance.OnEnemyExitedGridPosition -= OnEnemyExitedGridPosition;
        Enemy.OnEnemyDestroyed -= OnEnemyDestroyed;
    }
}
