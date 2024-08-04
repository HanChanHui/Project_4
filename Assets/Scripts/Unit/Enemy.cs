using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    private SpriteRenderer spriteRenderer;
    private GridPosition gridPosition;
    private GridPosition beforeGridPosition;
    private Transform originalTarget;

    [SerializeField] protected int maxDistance;
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private int attackRange = 1; // 공격 범위
    [SerializeField] private float attackDamage = 10f; // 공격 데미지
    [SerializeField] private float attackInterval = 1f; // 공격 간격
    [SerializeField] private Image healthBar;
    [SerializeField] private float knockbackForce = 1f;
    [SerializeField] private float knockbackDuration = 0.5f; // 밀려나는 시간

    private float currentHealth;
    private bool isAttackingTower = false;


    [SerializeField] private Tower targetTower; // **탑들을 감지하는 리스트**
    private Coroutine checkDistanceCoroutine;

    public delegate void EnemyDestroyedHandler(Enemy enemy);
    public static event EnemyDestroyedHandler OnEnemyDestroyed;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalTarget = ResourceManager.Instance.EnemyTarget;
        destinationSetter.target = originalTarget;

        StartCoroutine(MainRoutine());

        currentHealth = health / maxHealth;
        healthBar.fillAmount = currentHealth;

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
                OnTargetReached();
                yield break;
            }

            // 그리드 위치 업데이트
            gridPosition = LevelGrid.Instance.GetCameraGridPosition(transform.position);
            if (LevelGrid.Instance.IsValidGridPosition(gridPosition) && beforeGridPosition != gridPosition)
            {
                LevelGrid.Instance.EnemyMovedGridPosition(this, beforeGridPosition, gridPosition);
                beforeGridPosition = gridPosition;
            }

            TargetMove();

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

    private IEnumerator CoCheckDistance()
    {
        while (true)
        {
            if (!isAttackingTower)
            {
                targetTower = FindNearestTowerInRange();
                if (targetTower != null)
                {
                    SetNewTarget(targetTower.transform);
                    isAttackingTower = true;
                }
            }

            yield return new WaitForSeconds(0.1f); // 조정 가능한 딜레이
        }
    }

    private Tower FindNearestTowerInRange() {
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


        foreach (GridPosition findPosition in offsetGridPosition) {
            GridPosition testGridPosition = gridPosition + findPosition;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                continue;
            }

            if (LevelGrid.Instance.HasAnyTowerOnGridPosition(testGridPosition)) {
                return LevelGrid.Instance.GetTowerAtGridPosition(testGridPosition);
            }
        }

        return null;
    }


    private void TargetMove()
    {
        if (targetTower != null) {
            isAttackingTower = true;
            if (Vector3.Distance(transform.position, targetTower.transform.position) <= attackRange) {
                aiPath.canMove = false;
                if (checkDistanceCoroutine != null)
                {
                    StopCoroutine(checkDistanceCoroutine);
                    checkDistanceCoroutine = null;
                }
                StartCoroutine(AttackTower(targetTower));
            }
        } 
        else 
        {
            SetNewTarget(originalTarget);
            isAttackingTower = false;
            aiPath.canMove = true; // 이동 재개
        }
    }

    private IEnumerator AttackTower(Tower targetTower)
    {
        while (targetTower != null)
        {
            targetTower.TakeDamage(attackDamage);
            yield return new WaitForSeconds(attackInterval);
        }

        isAttackingTower = false;
        checkDistanceCoroutine = StartCoroutine(CoCheckDistance());
       
        yield return null;
    }

    public void TakeDamage(float damage)
    {
        health -= (int)damage;
        if (health < 0)
        {
            health = 0;
        }

        currentHealth = health / maxHealth;
        StartCoroutine(UpdateHealthBar());

        if (health <= 0)
        {
            DestroyEnemy();
        }
    }

    private IEnumerator UpdateHealthBar()
    {
        float elapsed = 0f;
        float duration = 0.05f; // Lerp over 0.5 seconds
        float startFillAmount = healthBar.fillAmount;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthBar.fillAmount = Mathf.Lerp(startFillAmount, currentHealth, elapsed / duration);
            yield return null;
        }
        healthBar.fillAmount = currentHealth;
    }

    private void HandleTowerPlaced(Tower tower)
    {
        foreach(GridPosition grid in tower.GridPositionList)
        {
            if(grid == gridPosition)
            {
                Vector3 directionToEnemy = (transform.position - tower.transform.position).normalized;
                Vector3 knockbackPosition = transform.position + directionToEnemy * knockbackForce;
                StartCoroutine(KnockbackRoutine(knockbackPosition));
            }
        }
        
    }

    private IEnumerator KnockbackRoutine(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < knockbackDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / knockbackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
    
    private void DestroyEnemy()
    {
        LevelGrid.Instance.RemoveEnemyAtGridPosition(gridPosition, this);
        OnEnemyDestroyed?.Invoke(this);
        gameObject.SetActive(false);
        Destroy(gameObject, 1f);
    }

    private void OnTargetReached()
    {
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        LevelGrid.Instance.OnTowerPlaced += HandleTowerPlaced; // 타워 설치 이벤트 구독
    }

    private void OnDisable() {
        LevelGrid.Instance.OnTowerPlaced -= HandleTowerPlaced;
        StopCoroutine(MainRoutine());
        StopCoroutine(UpdateHealthBar());

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
