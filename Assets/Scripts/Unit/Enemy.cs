using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private AIPath aiPath;
    private SpriteRenderer spriteRenderer;
    private GridPosition gridPosition;
    private GridPosition beforeGridPosition;

    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private Image healthBar;

    private float currentHealth;

    public delegate void EnemyDestroyedHandler(Enemy enemy);
    public static event EnemyDestroyedHandler OnEnemyDestroyed;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(ResetEnemyGridPosition());

        currentHealth = health / maxHealth;
        healthBar.fillAmount = currentHealth;
    }

    void Update()
    {
        // Check the AIPath velocity to determine the movement direction
        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (aiPath.desiredVelocity.x <= -0.01f)
        {
            spriteRenderer.flipX = true;
        }

        if (aiPath.reachedEndOfPath)
        {
            Destroy(gameObject);
        }
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth, Time.deltaTime * 10f);
    }

    private IEnumerator ResetEnemyGridPosition()
    {
        while(true)
        {
            gridPosition = LevelGrid.Instance.GetCameraGridPosition(transform.position);
            if(LevelGrid.Instance.IsValidGridPosition(gridPosition) 
                && beforeGridPosition != gridPosition)
            {
                LevelGrid.Instance.EnemyMovedGridPosition(this, beforeGridPosition, gridPosition);
                beforeGridPosition = gridPosition;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Debug.Log("들어오는지");
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                Damage(bullet.Damage);
                Destroy(other.gameObject);
            }
        }
    }

    private void Damage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
        }

        currentHealth = health / maxHealth;

        if (health <= 0)
        {
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        LevelGrid.Instance.RemoveEnemyAtGridPosition(gridPosition, this);
        OnEnemyDestroyed?.Invoke(this);
        Destroy(gameObject);
    }


}
