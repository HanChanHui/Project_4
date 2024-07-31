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

    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private Image healthBar;

    private float currentHealth;

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
        if(Input.GetKeyDown(KeyCode.A))
        {
            Demege(10f);
        }
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
        GridPosition beforeGridPosition = new GridPosition();
        while(true)
        {
            gridPosition = LevelGrid.Instance.GetCameraGridPosition(transform.position);
            if(LevelGrid.Instance.IsValidGridPosition(gridPosition) && beforeGridPosition != gridPosition)
            {
                Debug.Log("이동 : " + gridPosition);
                beforeGridPosition = gridPosition;
                LevelGrid.Instance.AddEnemyAtGridPosition(gridPosition, this);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.CompareTag("Bullet"))
        {
            Demege(10f);
        }
    }

    private void Demege(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
        }

        currentHealth = health / maxHealth;
        

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }


}
