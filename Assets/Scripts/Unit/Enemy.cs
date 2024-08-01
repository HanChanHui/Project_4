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

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
     [SerializeField] private int attackRange = 1; // 공격 범위
    [SerializeField] private float attackDamage = 10f; // 공격 데미지
    [SerializeField] private float attackInterval = 1f; // 공격 간격
    [SerializeField] private Image healthBar;

    private float currentHealth;
    private Coroutine attackCoroutine;

    public delegate void EnemyDestroyedHandler(Enemy enemy);
    public static event EnemyDestroyedHandler OnEnemyDestroyed;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalTarget = destinationSetter.target;

        StartCoroutine(MainRoutine());

        currentHealth = health / maxHealth;
        healthBar.fillAmount = currentHealth;
    }

     private IEnumerator MainRoutine()
    {
        while (true)
        {
            Direction();

            // 목표 도달 체크
            if (aiPath.reachedEndOfPath && aiPath.remainingDistance <= aiPath.endReachedDistance)
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

            Tower targetTower = FindNearestTower();
            if (targetTower != null)
            {
                SetNewTarget(targetTower.transform);
                aiPath.canMove = true;
                if (attackCoroutine == null)
                {
                    attackCoroutine = StartCoroutine(AttackTower(targetTower)); // 공격 코루틴 시작
                }
            }
            else
            {
                SetNewTarget(originalTarget);
                aiPath.canMove = true; // 이동 재개
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }
            }

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

     private Tower FindNearestTower()
    {
        if(LevelGrid.Instance.IsValidGridPosition(gridPosition))
        {
            foreach (Tower tower in LevelGrid.Instance.GetTowerListAtGridPosition(gridPosition)) {
                if (Vector2.Distance(transform.position, tower.transform.position) <= attackRange) {
                    return tower; // 가장 가까운 Tower를 반환합니다.
                }
            }
        }
        return null; // 근처에 Tower가 없으면 null 반환
    }

    private IEnumerator AttackTower(Tower targetTower)
    {
        while (targetTower != null && Vector2.Distance(transform.position, targetTower.transform.position) <= attackRange)
        {
            targetTower.TakeDamage(attackDamage);
            yield return new WaitForSeconds(attackInterval); // 공격 주기
        }
        attackCoroutine = null;
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

    private void OnDisable() {
        StopCoroutine(MainRoutine());
        StopCoroutine(UpdateHealthBar());
    }

    private void SetNewTarget(Transform newTarget)
    {
        destinationSetter.target = newTarget;
    }
}
