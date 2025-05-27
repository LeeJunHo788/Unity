using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class FlyingEye : EnemyController
{
  bool isCoolTime = false;
  float coolTime = 2.5f;

  

  protected override void Awake()
  {
    range = 50.0f; // Ž�� ����
    speed = 2.3f; // �̵��ӵ�
    att = 7;     // ���ݷ�
    def = 5;     // ����
    hp = 15;      // ü��
    exp = 15;     // ����ġ
    gold = 5;     // ��
    defIgn = 10;   // ���� ����
    spawnWeight = 10f;

    isFlyingEnemy = true;
    canAttack = true;
    isAttackMovementAllowed = true;
    attackRange = 5.0f;
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
    // ��Ÿ���̸� ����
    if (isCoolTime)
      yield break;

    if (isRecentKnockback)
      yield break;


    anim.SetTrigger("Attack");
    state = EnemyState.Attack;

    isAttack = true;
    isAttacking = true;
    rb.linearVelocity = Vector2.zero;


    Vector2 dashDir = (player.transform.position + new Vector3(0, 0.6f) - transform.position).normalized;
    float dashPower = 6f;     // ���� �ӵ�
    float chargeTime = 0.4f;  // �غ� �ð�
    float duration = 0.8f;    // ���� ���� �ð�

    dashCoroutine = StartCoroutine(DashAttack(dashDir, dashPower, chargeTime, duration));

    yield return dashCoroutine;

    isAttacking = false;
    state = EnemyState.Idle;

    yield return new WaitForSeconds(1.0f); // ���� ��������
    isAttack = false;

    yield return new WaitForSeconds(coolTime);  // ��Ÿ��
    isCoolTime = false;
  }

  
}

