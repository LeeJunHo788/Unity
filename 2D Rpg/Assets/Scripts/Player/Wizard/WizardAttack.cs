using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAttack : MonoBehaviour
{
  GameObject player;
  PlayerController pc;
  Animator anim;

  public float speed = 12f;   // �ӵ�
  public float lifeTime = 0.2f; // ����
  float targetRange = 7f; // Ž�� ����

  float explodeRadius = 0.4f;        // ���� ����
  bool exploding = false;    

  private Vector2 direction; // �⺻�� ���������� ����

  void Start()
  {
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();
    anim = GetComponent<Animator>();

    // ������ ���� �⺻ ���� ��ȭ(5����, 10����)
    if(pc.level > 9)
    {
      explodeRadius *= 2f;
      speed *= 1.4f;
      gameObject.transform.localScale = new Vector3(2f, 2f);
    }

    else if(pc.level > 4)
    {
      explodeRadius *= 1.2f;
      speed *= 1.2f;
      gameObject.transform.localScale = new Vector3(1.5f, 1.5f);
    }

    // �� ���� �� ����Ʈ ��ȯ
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    List<GameObject> allTargets = new List<GameObject>(enemies);

    GameObject nearestEnemy = null;
    float shortestDistance = Mathf.Infinity;

    foreach (GameObject enemy in allTargets)
    {
      EnemyController ec = enemy.GetComponent<EnemyController>();
      if (ec == null || ec.currentHp <= 0) continue; // ü���� 0 ������ ���� ����

      float distance = Vector2.Distance(transform.position, enemy.transform.position);
      Vector2 dirToEnemy = (enemy.transform.position - transform.position).normalized;

      // �÷��̾ ���� ����� ��ġ�ϴ��� Ȯ��
      if ((player.transform.rotation.y == 0 && dirToEnemy.x > 0) || (player.transform.rotation.y != 0 && dirToEnemy.x < 0))
      {
        if (distance < shortestDistance && distance <= targetRange)
        {
          shortestDistance = distance;
          nearestEnemy = enemy;
        }
      }
    }

    if (nearestEnemy != null)
    {
      // �� �������� ���� ���
      direction = (nearestEnemy.transform.position - transform.position).normalized;
    }
    else
    {
      // Ÿ���� ������ ������ �߻� 
      if (player.transform.rotation.y == 0)
      {
        direction = Vector2.right;
      }

      else
      {
        direction = Vector2.left;
      }
    }

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // Z�� �������� ȸ��

    Invoke("Explode", lifeTime);


  }

  void Update()
  {
    if (exploding)
      return;

    // ������ �������� �̵�
    transform.Translate(direction * (speed) * pc.attackSpeed * Time.deltaTime, Space.World);
  }



  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
    {
      bool isDead = false;

      EnemyController ec = other.GetComponent<EnemyController>();
      BossController bc = other.GetComponent<BossController>();

      if(ec != null)
      {
        if(ec.currentHp <= 0)
        {
          isDead = true;
        }
      }

      if(!isDead)
        Explode();
    }
  }

  void Explode()
  {
    if (exploding) return; //  �ߺ� ���� ����
    exploding = true;

    anim.SetTrigger("Explode");
    Vector2 center = transform.position;

    HashSet<GameObject> damagedTargets = new HashSet<GameObject>();

    // ���� Ž��
    Collider2D[] hits = Physics2D.OverlapCircleAll(center, explodeRadius);

    foreach (Collider2D hit in hits)
    {

      GameObject target = hit.gameObject;

      // �̹� ó���� ���̸� �ǳʶ�
      if (damagedTargets.Contains(target))
        continue;


      float dem;
      if (pc.level > 9)
        dem = pc.att * 1.4f;
      else if (pc.level > 4)
        dem = pc.att * 1.2f;
      else
        dem = pc.att;

      if (hit.CompareTag("Enemy")) // �� Ž��
      {
        EnemyController ec = hit.GetComponent<EnemyController>();
        if (ec != null)
        {
          // �˹� ������ hit ��ġ �������� ���
          Vector2 knockDir = hit.transform.position - transform.position;
          ec.StartCoroutine(ec.KnockBack(dem, knockDir));
          damagedTargets.Add(target); // ������ �� �� ���
        }
      }

      else if (hit.CompareTag("Boss")) // �� Ž��
      {
        BossController bc = hit.GetComponent<BossController>();
        if (bc != null)
        {
          // �˹� ������ hit ��ġ �������� ���
          Vector2 knockDir = hit.transform.position - transform.position;
          bc.StartCoroutine(bc.KnockBack(dem, knockDir));
          damagedTargets.Add(target); // ������ �� ���� ���
        }
      }
    }

    Destroy(gameObject, 0.7f);
  }

  void OnDrawGizmos()
  {
    // (2) �� ����� ���� ���� (������ ��)
    Gizmos.color = new Color(1f, 0f, 0f, 1f); // ������ ������

    // (3) �� ���� ������Ʈ ��ġ�� �߽����� �� �׸���
    Gizmos.DrawWireSphere(transform.position, explodeRadius); // ���� ���� �� �׸���

  }
}
