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
    range = 50.0f; // 탐색 범위
    speed = 2.3f; // 이동속도
    att = 7;     // 공격력
    def = 5;     // 방어력
    hp = 15;      // 체력
    exp = 15;     // 경험치
    gold = 5;     // 돈
    defIgn = 10;   // 방어력 무시
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

    // 애니메이터의 컨트롤러에서 모든 애니메이션 클립 가져오기
    RuntimeAnimatorController controller = anim.runtimeAnimatorController;
    AnimationClip[] clips = controller.animationClips;

    foreach (AnimationClip clip in clips)
    {
      // 공격 애니메이션의 길이 가져오기
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
    // 쿨타임이면 종료
    if (isCoolTime)
      yield break;

    if (isRecentKnockback)
      yield break;


    anim.SetTrigger("Attack");
    state = EnemyState.Attack;

    isAttack = true;
    isAttacking = true;
    rb.velocity = Vector2.zero;


    Vector2 dashDir = (player.transform.position + new Vector3(0, 0.6f) - transform.position).normalized;
    float dashPower = 6f;     // 돌진 속도
    float chargeTime = 0.4f;  // 준비 시간
    float duration = 0.8f;    // 돌진 지속 시간

    dashCoroutine = StartCoroutine(DashAttack(dashDir, dashPower, chargeTime, duration));

    yield return dashCoroutine;

    isAttacking = false;
    state = EnemyState.Idle;

    yield return new WaitForSeconds(1.0f); // 공격 정지상태
    isAttack = false;

    yield return new WaitForSeconds(coolTime);  // 쿨타임
    isCoolTime = false;
  }

  
}

