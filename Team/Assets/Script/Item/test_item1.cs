using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_item1 : Itembase
{
    

    public float attackRange = 5.0f; // 공격 범위
    public int attackPower = 2; // 공격력
    public float attackCooldown = 1.0f; // 공격 간격    

    private bool isAttacking = false; // 현재 공격 중인지 여부
    private GameObject currentTarget; // 현재 공격 대상

    private void Update()
    {
        // 범위 내 적 탐색
        currentTarget = FindClosestEnemyInRange();
        if (currentTarget != null && !isAttacking)
        {
            StartCoroutine(Attack(currentTarget)); // 공격 코루틴 시작
        }
    }

    private GameObject FindClosestEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= attackRange)
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        return closestEnemy;
    }

    IEnumerator Attack(GameObject enemy)
    {
        isAttacking = true;
        while (enemy != null && enemy.activeSelf && Vector2.Distance(transform.position, enemy.transform.position) <=attackRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, attackRange);
            if (hit.collider != null && hit.collider.gameObject == enemy)
            {
                EnemyController enemyScript = enemy.GetComponent<EnemyController>();
                if (enemyScript != null)
                {
                    enemyScript.currentHp -= attackPower; // 적의 체력 감소                
                }
            }
            
            yield return new WaitForSeconds(attackCooldown); // 공격 대기
        }  
        isAttacking = false;
    }


    public override void Upgrade()
    {
        base.Upgrade(); // 기본 업그레이드 로직 호출
        attackPower += 1; // 레벨 업 시 공격력 증가
        attackRange += 0.5f; // 레벨 업 시 공격 범위 증가        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // 공격 범위를 기즈모로 표시
    }
}