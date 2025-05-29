using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firewizard : EnemyController
{
  public GameObject attack;


  protected override void Awake()
  {
    range = 35.0f; // 탐색 범위
    speed = 0f; // 이동속도
    att = 5;     // 공격력
    def = 5;     // 방어력
    hp = 25;      // 체력
    exp = 20;     // 경험치
    gold = 7;     // 돈
    defIgn = 15;   // 방어력 무시
    spawnWeight = 5f;

    canAttack = true;
    attackRange = 15.0f;
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

    anim.SetTrigger("Attack");


    isAttack = true;
    isAttacking = true;
    rb.velocity = Vector2.zero;
    state = EnemyState.Attack;



    Vector2 hitDir = (player.transform.position - transform.position);

    if (hitDir.x > 0)
    {
      transform.rotation = Quaternion.Euler(0, 180, 0);
      Vector2 pos = transform.position + new Vector3(0.2f, -0.2f);
      // 슬라이더는 회전효과 제거
      hpSlider.transform.localRotation = Quaternion.Euler(0, 0, 0);
      StartCoroutine(Attack(pos));

    }

    else if (hitDir.x <= 0)
    {
      transform.rotation = Quaternion.Euler(0, 180, 0);
      Vector2 pos = transform.position + new Vector3(-0.2f, -0.2f);
      // 슬라이더는 회전효과 제거
      hpSlider.transform.localRotation = Quaternion.Euler(0, 0, 0);

      StartCoroutine(Attack(pos));

    }

    state = EnemyState.Idle;

    yield return new WaitForSeconds(attackTime); // 공격 후 쿨타임
    isAttacking = false;


    yield return new WaitForSeconds(2f); // 공격 후 쿨타임

    isAttack = false;
  }

  IEnumerator Attack(Vector2 pos)
  {

    yield return new WaitForSeconds(0.9f); // 0.8초 기다리기

    if (currentHp <= 0)
    {
      isAttack = false;
      isAttacking = false;
      anim.SetInteger("State", 4);
      yield break;
    }

    GameObject clone = Instantiate(attack, pos, Quaternion.identity);


    Destroy(clone, 10f);
  }
}
