using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
using TMPro;

public abstract class BaseEnemy : LivingEntity
{
    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    private SpriteRenderer spriteRenderer;
    private GridPosition gridPosition;
    private GridPosition beforeGridPosition;
    private Transform originalTarget;

    [SerializeField] protected int maxDistance;
    [SerializeField] private int attackRange = 1; // 공격 범위
    [SerializeField] protected float attackDamage = 10f; // 공격 데미지
    [SerializeField] protected float attackInterval = 1f; // 공격 간격
    [SerializeField] private float knockbackForce = 1f;
    [SerializeField] private float knockbackDuration = 0.5f; // 밀려나는 시간

    
    [SerializeField] private EnemyHealthLabel healthBar;
    [SerializeField] private GameObject textDamage;
    [SerializeField] private Transform textDamageSpawnPosition;

    protected bool isAttackingTower = false;

    [SerializeField] protected Tower targetTower; // **탑들을 감지하는 리스트**
    protected Coroutine checkDistanceCoroutine;

    public delegate void EnemyDestroyedHandler(BaseEnemy enemy);
    public static event EnemyDestroyedHandler OnEnemyDestroyed;

    protected virtual void Start()
    {
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalTarget = ResourceManager.Instance.EnemyTarget;
        destinationSetter.target = originalTarget;

        StartCoroutine(MainRoutine());
        SetHealth(100);
        healthBar.Init();

        checkDistanceCoroutine = StartCoroutine(CoCheckDistance());
    }

    private IEnumerator MainRoutine()
    {
        while (true)
        {
            Direction();

            // 목표 도달 체크
            if (!isAttackingTower && aiPath.reachedEndOfPath && aiPath.remainingDistance <= aiPath.endReachedDistance)
            {
                Destroy(gameObject);
                yield break;
            }

            // 그리드 위치 업데이트
            gridPosition = LevelGrid.Instance.GetCameraGridPosition(transform.position);
            if (LevelGrid.Instance.IsValidGridPosition(gridPosition) && beforeGridPosition != gridPosition)
            {
                LevelGrid.Instance.EnemyMovedGridPosition(this, beforeGridPosition, gridPosition);
                beforeGridPosition = gridPosition;
            }

            //TargetMove();

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Direction()
    {
        // 적의 방향 설정
        if (aiPath.desiredVelocity.x >= 0.01f) {
            spriteRenderer.flipX = false;
        } else if (aiPath.desiredVelocity.x <= -0.01f) {
            spriteRenderer.flipX = true;
        }
    }

    #region Tower Grid Find
        protected IEnumerator CoCheckDistance()
        {
            while (true)
            {
                if (!isAttackingTower)
                {
                    targetTower = FindNearestTowerInRange();
                    
                    if (targetTower != null)
                    {
                        SetNewTarget(originalTarget);
                        if (Vector3.Distance(transform.position, targetTower.transform.position) <= attackRange && !isAttackingTower) 
                        {
                            isAttackingTower = true;
                            aiPath.canMove = false;
                            if (checkDistanceCoroutine != null) {
                                StopCoroutine(checkDistanceCoroutine);
                                checkDistanceCoroutine = null;
                            }
                            AttackTarget(targetTower);
                        }
                    }
                    else
                    {
                        SetNewTarget(originalTarget);
                        isAttackingTower = false;
                        aiPath.canMove = true; // 이동 재개
                    }
                }
    
                yield return new WaitForSeconds(0.1f); // 조정 가능한 딜레이
            }
        }
    
        private Tower FindNearestTowerInRange() 
        {
            List<GridPosition> offsetGridPosition = new List<GridPosition>();
    
            float velX = aiPath.desiredVelocity.x;
            float velY = aiPath.desiredVelocity.y;
    
            if (Mathf.Abs(velX) > 0.01f && Mathf.Abs(velY) > 0.01f) {
                int xDirection = velX > 0 ? 1 : -1;
                int yDirection = velY > 0 ? 1 : -1;
    
                // 대각선 이동 처리
                offsetGridPosition.Add(new GridPosition(xDirection, 0));
                offsetGridPosition.Add(new GridPosition(0, yDirection));
                offsetGridPosition.Add(new GridPosition(xDirection, yDirection));
            } else if (Mathf.Abs(velX) > 0.01f) {
                int xDirection = velX > 0 ? 1 : -1;
    
                // 수평 이동 처리
                offsetGridPosition.Add(new GridPosition(xDirection, 0));
            } else if (Mathf.Abs(velY) > 0.01f) {
                int yDirection = velY > 0 ? 1 : -1;
    
                // 수직 이동 처리
                offsetGridPosition.Add(new GridPosition(0, yDirection));
            }
    
            foreach (GridPosition findPosition in offsetGridPosition) 
            {
                GridPosition testGridPosition = gridPosition + findPosition;
    
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                if(LevelGrid.Instance.HasAnyBlockOnGridPosition(testGridPosition))
                {
                    continue;
                }
    
                if (LevelGrid.Instance.HasAnyTowerOnGridPosition(testGridPosition)) 
                {
                    return LevelGrid.Instance.GetTowerAtGridPosition(testGridPosition);
                }
            }
    
            return null;
        }
    #endregion
   
    #region Tower Crash
        private void HandleTowerPlaced(Tower tower) {
            foreach (GridPosition grid in tower.GridPositionList) {
                if (grid == gridPosition) {
                    Vector3 directionToEnemy = (transform.position - tower.transform.position).normalized;
                    Vector3 knockbackPosition = GetSafeKnockbackPosition(directionToEnemy);
                    StartCoroutine(KnockbackRoutine(knockbackPosition));
                }
            }
        }
    
        private Vector3 GetSafeKnockbackPosition(Vector3 initialDirection) {
            Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
            Vector3 bestDirection = initialDirection;
    
            foreach (var direction in directions) {
                Vector3 checkPosition = transform.position + direction;
                if (IsPositionBlocked(checkPosition)) {
                    bestDirection = -direction;
                    break;
                }
            }
    
            return transform.position + bestDirection * knockbackForce;
        }
    
        private bool IsPositionBlocked(Vector3 position) {
            Collider2D hitCollider = Physics2D.OverlapCircle(position, 1f, LayerMask.GetMask("Block"));
            return hitCollider != null;
        }
    
        private IEnumerator KnockbackRoutine(Vector3 targetPosition) {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;
    
            while (elapsedTime < knockbackDuration) {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / knockbackDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
    
            transform.position = targetPosition;
        }
    #endregion
    

    protected abstract void AttackTarget(Tower targetTower);

    public override void TakeDamage(float damage, int obstacleDamage = 1, bool isCritical = false, bool showLabel = false)
    {
        base.TakeDamage(damage, obstacleDamage, isCritical, showLabel);
        EnemyHit(damage);
        healthBar.Show();
        healthBar.UpdateHealth(health, maxHealth);

        if (health <= 0)
        {
            DestroyEnemy();
        }
    }

    private void EnemyHit(float damage)
    {
        GameObject DamageLabel = Instantiate(textDamage, transform);
        DamageLabel.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();
        DamageLabel.transform.SetParent(textDamageSpawnPosition);
        Destroy(DamageLabel, 1f);
    }

    private void DestroyEnemy()
    {
        LevelGrid.Instance.RemoveEnemyAtGridPosition(gridPosition, this);
        OnEnemyDestroyed?.Invoke(this);
        gameObject.SetActive(false);
        Destroy(gameObject, 1f);
    }
    private void OnEnable()
    {
        LevelGrid.Instance.OnTowerPlaced += HandleTowerPlaced; // 타워 설치 이벤트 구독
    }

    private void OnDisable() {
        LevelGrid.Instance.OnTowerPlaced -= HandleTowerPlaced;
        StopCoroutine(MainRoutine());
        //StopCoroutine(UpdateHealthBar());

        if (checkDistanceCoroutine != null)
        {
            StopCoroutine(checkDistanceCoroutine);
        }
    }

    private void SetNewTarget(Transform newTarget)
    {
        destinationSetter.target = newTarget;
    }
}
