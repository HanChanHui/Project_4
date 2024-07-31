using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static Action<Enemy, float> OnEnemyHit;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float minDistanceToDealDamage = 0.1f;
    private float damage;
    public float Damage{get {return damage;} set{damage = value;}}

    private Enemy enemyTarget;

    private void Update() 
    {
        if(enemyTarget != null)
        {
            Move();
            RotateBullet();
        }
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position,
            enemyTarget.transform.position, moveSpeed * Time.deltaTime);
        float distanceToTarget = (enemyTarget.transform.position - transform.position).magnitude;
        if(distanceToTarget < minDistanceToDealDamage)
        {
            OnEnemyHit?.Invoke(enemyTarget, damage);
        }
    }

    private void RotateBullet()
    {
        Vector3 enemyPos = enemyTarget.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.up, enemyPos, transform.forward);
        transform.Rotate(0f, 0f, angle);
    }

    public void SetEnemy(Enemy enemy)
    {
        enemyTarget = enemy;
    }

}
