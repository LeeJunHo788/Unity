using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class Golem : BossController
{
  public GameObject golem_Bullet;
  public GameObject defUpEffect;
  float attackRange = 20f;


  float buffCooldown = 10f;         // ���潺 ���� ��Ÿ�� �߰�
  float buffCooldownTimer = 0f;     // ���� Ÿ�̸�


  protected override void Start()
  {
    base.Start();
    isAttacking = false;
  }

  protected override void Update()
  {
    if (currentHp <= 0) return;

    base.Update();    // �̵� ���� ó��

    float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);

    // ���� ��ų ��Ÿ�� üũ
    if (buffCooldownTimer <= 0f)
    {
      StartCoroutine(DefenseBuff());
      buffCooldownTimer = buffCooldown; // ��Ÿ�� �ʱ�ȭ
    }
    else
    {
      buffCooldownTimer -= Time.deltaTime; // ��Ÿ�� ����
    }

    if (distanceToPlayer < attackRange)
    {
      if (!isAttacking)
      {
        StartCoroutine(Attack());
      }
    }
  }

  IEnumerator Attack()
  {
    if (isAttacking) yield break;
    isAttacking = true;

    animator.SetTrigger("Attack");

    yield return new WaitForSeconds(0.5f);

    Vector3 playerPos = player.transform.position;
    Vector3 enemyPos = transform.position;
    Vector3 dirToPlayer = (playerPos - enemyPos).normalized;

    float baseAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

    // ���� ���� ó��
    float directionMultiplier = transform.localScale.x < 0 ? -1f : 1f;

    CreateBulletAtAngle(enemyPos, baseAngle + (directionMultiplier * -30f));
    CreateBulletAtAngle(enemyPos, baseAngle + (directionMultiplier * -10f));
    CreateBulletAtAngle(enemyPos, baseAngle + (directionMultiplier * 0f));
    CreateBulletAtAngle(enemyPos, baseAngle + (directionMultiplier * 10f));
    CreateBulletAtAngle(enemyPos, baseAngle + (directionMultiplier * 30f));

    StartCoroutine(AttackCooldown());
  }

  // �Ѿ� ���� �޼��� 
  void CreateBulletAtAngle(Vector3 spawnPos, float angle)
  {
    float rad = angle * Mathf.Deg2Rad;
    Vector3 shootDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized;

    GameObject bullet = Instantiate(golem_Bullet, spawnPos, Quaternion.identity);
    bullet.GetComponent<Golem_Attack>().SetDirection(shootDir);

  }

  IEnumerator AttackCooldown()
  {
    isAttacking = true;
    yield return new WaitForSeconds(2f);
    isAttacking = false;
  }

  IEnumerator DefenseBuff()
  {
    if (isAttacking) yield break;
    isAttacking = true;

    animator.SetTrigger("Ability");

    // ���� �����ϴ� ��� EnemyController ã��
    EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();

    foreach (EnemyController enemy in allEnemies)
    {
      if (enemy == this) continue; // �ڱ� �ڽ��̸� �Ѿ��

      enemy.defence += 10f; // ���� 10��ŭ ����

      // �ݶ��̴��� �̿��ؼ� ����Ʈ ��ġ ���
      Collider2D collider = enemy.GetComponent<Collider2D>();

      Vector3 spawnPos = enemy.transform.position;
      if (collider != null)
      {
        // �ݶ��̴��� bounds�� �̿��� ���� ���� ���� ���
        spawnPos = collider.bounds.center + new Vector3(0f, collider.bounds.extents.y, 0f);

        // �׸��� �ణ ���������ε� �̵� (���ϴ� ��ŭ ���� ����)
        spawnPos += new Vector3(0.3f, 0.3f, 0f);
      }
      else
      {
        // �ݶ��̴��� ���ٸ� �⺻ ��ġ (��¦ ��)
        spawnPos += new Vector3(0.5f, 1.2f, 0f);
      }

      GameObject arrowObj = Instantiate(defUpEffect, spawnPos, Quaternion.identity, enemy.transform);

      Vector3 arrowScale = arrowObj.transform.localScale;
      arrowScale.x = enemy.transform.localScale.x > 0 ? Mathf.Abs(arrowScale.x) : -Mathf.Abs(arrowScale.x);
      arrowObj.transform.localScale = arrowScale;

      Destroy(arrowObj, 1.0f); // 1�� �� ȭ��ǥ ����
    }

    Debug.Log("��� �� ���� ���� + ȭ��ǥ ǥ��!");

    yield return null;

    StartCoroutine(AttackCooldown());
  }
}

