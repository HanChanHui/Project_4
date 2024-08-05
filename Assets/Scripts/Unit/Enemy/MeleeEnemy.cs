using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    protected override void AttackTarget(Tower targetTower)
    {
        StartCoroutine(AttackRoutine(targetTower));
    }

    private IEnumerator AttackRoutine(Tower targetTower)
    {
        while (targetTower != null)
        {
            targetTower.TakeDamage(attackDamage);
            yield return new WaitForSeconds(attackInterval);
        }

        isAttackingTower = false;
        checkDistanceCoroutine = StartCoroutine(CoCheckDistance());
    }
}
