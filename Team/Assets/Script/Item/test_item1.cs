using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_item1 : Itembase
{
    

    public float attackRange = 5.0f; // ���� ����
    public int attackPower = 2; // ���ݷ�
    public float attackCooldown = 1.0f; // ���� ����    

    private bool isAttacking = false; // ���� ���� ������ ����
    private GameObject currentTarget; // ���� ���� ���

    private void Update()
    {
        // ���� �� �� Ž��
        currentTarget = FindClosestEnemyInRange();
        if (currentTarget != null && !isAttacking)
        {
            StartCoroutine(Attack(currentTarget)); // ���� �ڷ�ƾ ����
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
                    enemyScript.currentHp -= attackPower; // ���� ü�� ����                
                }
            }
            
            yield return new WaitForSeconds(attackCooldown); // ���� ���
        }  
        isAttacking = false;
    }


    public override void Upgrade()
    {
        base.Upgrade(); // �⺻ ���׷��̵� ���� ȣ��
        attackPower += 1; // ���� �� �� ���ݷ� ����
        attackRange += 0.5f; // ���� �� �� ���� ���� ����        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // ���� ������ ������ ǥ��
    }
}