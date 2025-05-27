using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponController : MonoBehaviour
{
  public LayerMask enemyLayer;    // �� ���̾�
  public GameObject bulletPrefb;  // �Ѿ� ������

  float range = 30f;              // �� Ž�� ����
  float attackSpeed = 5f;              // ���� �ӵ�
  float attackTime;         // ���� ������ �ð�
  float attackRange = 8f;        // ���� ����
  Vector2 dir;                    // ���� ����
  float angle;                    // ���⿡ ���� ����
  


  bool isAttacking = false;       // ���� ����

  GameObject player;      // �÷��̾� ������Ʈ
  PlayerController pc;    // �÷��̾� ��Ʈ�ѷ�
  GameObject nearEnemy;       // ���� ����� ��

  void Start()
  {
    player = GameObject.FindWithTag("Player");                          // �÷��̾� ã��
    pc = player.GetComponent<PlayerController>();                       // �÷��̾� ��Ʈ�ѷ� ��������
    attackTime = 1 / (pc.attackSpeed * attackSpeed);   // ���� �ӵ� ��������
    StartCoroutine(FindEnemyCoroutine());    // ���� �ð� ���� �� ���� ����

  }

  void Update()
  {
    transform.position = player.transform.position + (Vector3.right * 1.5f);    // �÷��̾� �������� ��ġ ����

    // ���� �������� �ƴ϶�� ���� ����
    if(!isAttacking)
    {
      StartCoroutine(Attack());
    }
  }


  // �� Ž�� �޼���
  void FindEnemy()
  {
    
    float minDistance = Mathf.Infinity;   // �ּ� �Ÿ�
    
    nearEnemy = null;

    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);   // �÷��̾� �ֺ� Ž������ ���� �� Ž��

    foreach (var collider in hitColliders)
    {
      float dist = Vector2.Distance(transform.position, collider.transform.position);   // �Ÿ� Ȯ��

      if (dist < minDistance)
      {
        minDistance = dist;       // �ּ� �Ÿ� ����
        nearEnemy = collider.gameObject;
      }
    }

    if (nearEnemy != null)
    {
      dir = (nearEnemy.transform.position - transform.position).normalized;       // ���� ����
      angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;  
      transform.rotation = Quaternion.Euler(0, 0, angle);                         // ȸ�� ����

      if (dir.x < 0)    // ���� ���ʿ� �ִ� ���
      {
        transform.localScale = new Vector3(1, -1, 1);     // ��������Ʈ ����
      }

      else
      {
        transform.localScale = new Vector3(1, 1, 1);      // ��������Ʈ ����ȭ
      }
    }
  }

  // �����ð� ���� Ž�� �޼��� �ݺ�
  IEnumerator FindEnemyCoroutine()
  {
    while(true)
    {
      FindEnemy();
      yield return new WaitForSeconds(0.05f);
    }
  }
  
  // ���� ����
  IEnumerator Attack()
  {
    isAttacking = true;

    // ���� ������ ���� ����
    if (nearEnemy == null)
    {
      isAttacking = false;
      yield break; 
    }

    float dist = Vector2.Distance(transform.position, nearEnemy.transform.position);      // ���� ����� ������ �Ÿ�

    // ���� ���� �ȿ� �ִ� ���� ���� �ݺ�
    while (dist <= attackRange)
    {
      if (nearEnemy == null) break; //Ÿ���� �����Ǿ��ų� ������� ��� ���� Ż��

      float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

      GameObject bullet = Instantiate(bulletPrefb, transform.position, Quaternion.Euler(0, 0, angle));    // �Ѿ� ����
      bullet.GetComponent<BulletController>().SetDirection(dir);    // ���� �Ѱ��ֱ�

      yield return new WaitForSeconds(attackTime);       // ���� �ð���ŭ ���

      if (nearEnemy == null) break; //Ÿ���� �����Ǿ��ų� ������� ��� ���� Ż��
      dist = Vector2.Distance(transform.position, nearEnemy.transform.position);    // �Ÿ� �ٽ� ���

      Destroy(bullet, 10f);     // 10�� �� ����

    }

    isAttacking = false; // ���� ������ ���� ����
  }

}
