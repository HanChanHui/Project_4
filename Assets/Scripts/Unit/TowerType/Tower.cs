using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : LivingEntity
{
    protected SpriteRenderer sprite;
    private Color originalColor;

    protected List<GridPosition> gridPositionList = new List<GridPosition>();
    public List<GridPosition> GridPositionList { get { return gridPositionList; } set { gridPositionList = value; } }
    [SerializeField] protected List<BaseEnemy> enemiesInRange = new List<BaseEnemy>();
    public List<BaseEnemy> EnemiesInRange { get { return enemiesInRange; } }

    [SerializeField] protected int maxDistance;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] private HealthLabel healthBar;

    protected virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            originalColor = sprite.color;
        }
    }

    protected virtual void Start()
    {
        if (gridPositionList != null)
        {
            foreach (GridPosition pos in gridPositionList)
            {
                LevelGrid.Instance.AddTowerAtGridPosition(pos, this);
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
        BaseEnemy.OnEnemyDestroyed += OnEnemyDestroyed;

        //SetHealth(100);
        if(healthBar != null)
        {
            healthBar.Init();
        }
        StartCoroutine(CoCheckDistance());
        GridRangeCheck();
    }

    // private IEnumerator CheckEnemy()
    // {
    //     while(enemiesInRange == null)
    //     {
    //         GridRangeCheck();
    //         yield return new WaitForSeconds(1f);
    //     }
        
    // }

    protected void OnEnemyEnteredGridPosition(BaseEnemy enemy, GridPosition gridPosition)
    {
        if (IsWithinRange(gridPosition))
        {
            if (!enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    protected void OnEnemyExitedGridPosition(BaseEnemy enemy, GridPosition gridPosition)
    {
        if(!IsWithinRange(gridPosition))
        {
            if (enemiesInRange.Contains(enemy)) 
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }

    protected void OnEnemyDestroyed(BaseEnemy enemy)
    {
        if (enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
        }
        GridRangeCheck();
        // if(enemiesInRange == null)
        // {
        //     StartCoroutine(CheckEnemy());
        // }
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

    protected void GridRangeCheck()
    {
        for(int x = -maxDistance; x <= maxDistance; x++)
        {
            for(int y = -maxDistance; y <= maxDistance; y++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, y);
                GridPosition testGridPosition = gridPositionList[0] + offsetGridPosition;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if(LevelGrid.Instance.HasAnyEnemyOnGridPosition(testGridPosition) && IsWithinRange(testGridPosition))
                {
                    enemiesInRange.Add(LevelGrid.Instance.GetEnemiesAtGridPosition(testGridPosition));
                }
            }
        }
    }

    protected virtual IEnumerator CoCheckDistance()
    {
        yield return null;
    }

    public override void TakeDamage(float damage, int obstacleDamage = 1, bool isCritical = false, bool showLabel = false)
    {
        base.TakeDamage(damage, obstacleDamage, isCritical, showLabel);
        if(healthBar != null)
        {
            healthBar.Show();
            healthBar.UpdateHealth(health, maxHealth);
        }

        StartCoroutine(FlashRed());

        if (health <= 0)
        {
           foreach(GridPosition gridPosition in gridPositionList)
           {
                LevelGrid.Instance.RemoveTowerAtGridPosition(gridPosition, this);
           }
           GameManager.Instance.RemovePlaceableTowerList(this);
           Destroy(gameObject);
        }
    }

    private IEnumerator FlashRed()
    {
        if (sprite != null)
        {
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sprite.color = originalColor;
        }
    }

    protected virtual void OnDestroy()
    {
        LevelGrid.Instance.OnEnemyEnteredGridPosition -= OnEnemyEnteredGridPosition;
        LevelGrid.Instance.OnEnemyExitedGridPosition -= OnEnemyExitedGridPosition;
        BaseEnemy.OnEnemyDestroyed -= OnEnemyDestroyed;
    }
}
