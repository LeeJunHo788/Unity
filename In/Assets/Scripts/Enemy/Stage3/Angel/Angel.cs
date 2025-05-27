using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Angel : EnemyController
{
  public GameObject warningLinePrefab;  // LineRenderer�� ���� ������
  public GameObject attack;             // �Ѿ� ������


  protected override void Awake()
  {
    range = 35.0f; // Ž�� ����
    speed = 1.8f; // �̵��ӵ�
    att = 5;     // ���ݷ�
    def = 5;     // ����
    hp = 35;      // ü��
    exp = 30;     // ����ġ
    gold = 10;     // ��
    defIgn = 15;   // ���� ����
    spawnWeight = 5;

    canMove = false;
    canAttack = true;
    attackRange = 100.0f;
  }

  protected override void Start()
  {
    currentHp = hp;
    base.Start();

    // �ִϸ������� ��Ʈ�ѷ����� ��� �ִϸ��̼� Ŭ�� ��������
    RuntimeAnimatorController controller = anim.runtimeAnimatorController;
    AnimationClip[] clips = controller.animationClips;

    foreach (AnimationClip clip in clips)
    {
      // ���� �ִϸ��̼��� ���� ��������
      if (clip.name == mobName + "_Attack")
      {
        attackTime = clip.length;
        break;
      }
    }

  }

  protected override void Update()
  {
    base.Update();
  }

  protected override IEnumerator PerformAttack()
  {
    anim.SetTrigger("Attack");

    isAttack = true;
    isAttacking = true;
    rb.linearVelocity = Vector2.zero;
    state = EnemyState.Attack;

    Vector3 playerPos = player.transform.position + new Vector3(0, 0.4f); // �÷��̾� �Ӹ� �� ����

    
    yield return StartCoroutine(Attack(transform.position, playerPos));
    isAttacking = false;

    // ������ ���� �ڿ��� �ٽ� �� �� üũ
    if (currentHp <= 0)
    {
      isAttacking = false;
      isAttack = false;
      yield break;
    }

    state = EnemyState.Idle;
    yield return new WaitForSeconds(3f); // ���� �� ��Ÿ��

    isAttack = false;
  }

  IEnumerator Attack(Vector3 start, Vector3 player)
  {

    // 1. ����� ����
    GameObject warningLine = Instantiate(warningLinePrefab, start, Quaternion.identity);
    LineRenderer line = warningLine.GetComponent<LineRenderer>();

    if (line != null)
    {
      Vector3 startPos = start;
      Vector3 direction = (player - start).normalized; // �÷��̾� ����
      Vector3 endPos = start + direction * 100f;         // 100 �Ÿ���ŭ �� �߱�

      line.startWidth = 0.1f;
      line.endWidth = 0.1f;
      line.SetPosition(0, startPos);
      line.SetPosition(1, endPos);

      // ���� ���� ����, ���ĸ� 0.4�� ����
      Color currentStart = line.startColor;
      Color currentEnd = line.endColor;

      // ���İ��� ����
      currentStart.a = 0.4f;
      currentEnd.a = 0.4f;

      // �ٽ� ����
      line.startColor = currentStart;
      line.endColor = currentEnd;
      
      
    }

   

    yield return new WaitForSeconds(1.3f); // 1.3�� ��ٸ���

    if (currentHp <= 0)
    {
      Destroy(warningLine); // ����� ����
      isAttack = false;
      isAttacking = false;
      anim.SetInteger("State", 4);
      yield break;
    }

    Destroy(warningLine); // ����� ����
    GameObject clone = Instantiate(attack, start, Quaternion.identity);
    clone.name = attack.name;

    if (clone.name.Contains("Angel_Bullet")) // �̸��� Angel_Bullet ���ԵǸ�
    {
      BulletController bullet = clone.GetComponent<BulletController>();
      if (bullet != null)
      {
        Vector2 warnDir = (player - start).normalized;
        bullet.SetDirection(warnDir);
      }
    }

    Destroy(clone, 10f);
  }
}
