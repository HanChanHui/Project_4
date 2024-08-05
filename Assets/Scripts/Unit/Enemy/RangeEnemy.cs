using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : BaseEnemy
{
    [SerializeField] private GameObject projectilePrefab; // 투사체 프리팹
    [SerializeField] private float projectileSpeed = 10f;

    protected override void AttackTarget(Tower targetTower)
    {
        StartCoroutine(AttackRoutine(targetTower));
    }

    private IEnumerator AttackRoutine(Tower targetTower)
    {
        while (targetTower != null)
        {
            ShootProjectile(targetTower);
            yield return new WaitForSeconds(attackInterval);
        }

        isAttackingTower = false;
        checkDistanceCoroutine = StartCoroutine(CoCheckDistance());
    }

    private void ShootProjectile(Tower targetTower)
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Vector3 direction = (targetTower.transform.position - transform.position).normalized;
            projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
            Destroy(projectile, 2f); // 일정 시간이 지나면 투사체를 파괴
        }
    }
}
