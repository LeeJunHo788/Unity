using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gunman : EnemyController
{

  public GameObject attack;


  protected override void Awake()
  {
    range = 35.0f; // 탐색 범위
    speed = 2.1f; // 이동속도
    att = 5;     // 공격력
    def = 5;     // 방어력
    hp = 17;      // 체력
    exp = 15;     // 경험치
    gold = 5;     // 돈
    defIgn = 15;   // 방어력 무시

    canAttack = true;
    attackRange = 8.0f;
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
      transform.rotation = Quaternion.Euler(0, 0, 0);
      Vector2 pos = transform.position + new Vector3(0.4f, 0.2f);
      // 슬라이더는 회전효과 제거
      hpSlider.transform.localRotation = Quaternion.Euler(0, 0, 0);

      StartCoroutine(Attack(pos));

    }

    else if (hitDir.x <= 0)
    {
      transform.rotation = Quaternion.Euler(0, 180, 0);
      Vector2 pos = transform.position + new Vector3(-0.4f, 0.2f);
      // 슬라이더는 회전효과 제거
      hpSlider.transform.localRotation = Quaternion.Euler(0, 180, 0);

      StartCoroutine(Attack(pos));

    }


    yield return new WaitForSeconds(attackTime / 0.6f); // 공격 후 쿨타임
    isAttacking = false;

    // 공격이 끝난 뒤에도 다시 한 번 체크
    if (currentHp <= 0)
    {
      isAttacking = false;
      isAttack = false;
      yield break;
    }

    state = EnemyState.Idle;
    yield return new WaitForSeconds(1f); // 공격 후 쿨타임

    isAttack = false;
  }

  IEnumerator Attack(Vector2 pos)
  {

    Vector2 savedPlayerPos = player.transform.position;   // 플레이어 위치 저장

    yield return new WaitForSeconds(0.85f); // 사격 준비

    if (currentHp <= 0)
    {
      isAttack = false;
      isAttacking = false;
      anim.SetInteger("State", 4);
      yield break;
    }

    GameObject clone1 = Instantiate(attack, pos, Quaternion.identity);
    GameObject clone2 = Instantiate(attack, pos, Quaternion.identity);
    GameObject clone3 = Instantiate(attack, pos, Quaternion.identity);

    
    BulletController bullet1 = clone1.GetComponent<BulletController>();
    BulletController bullet2 = clone2.GetComponent<BulletController>();
    BulletController bullet3 = clone3.GetComponent<BulletController>();

    
    Vector2 baseDir = (savedPlayerPos + Vector2.up * 0.5f - pos).normalized;   // 1번 총알은 플레이어 정조준

    // 퍼지는 각도 설정 
    float angleOffset = 15f; // 원하는 퍼짐 각도
    float rad = angleOffset * Mathf.Deg2Rad;

    Vector2 dirLeft = RotateVector(baseDir, rad);   // 왼쪽으로 퍼짐
    Vector2 dirRight = RotateVector(baseDir, -rad); // 오른쪽으로 퍼짐


    bullet1.SetDirection(baseDir);   // 가운데
    bullet2.SetDirection(dirLeft);   // 왼쪽
    bullet3.SetDirection(dirRight);  // 오른쪽
    
    Destroy(clone1, 5f);
    Destroy(clone2, 5f);
    Destroy(clone3, 5f);
  }

  // 각도 전환 메서드
  Vector2 RotateVector(Vector2 v, float radians)
  {
    float cos = Mathf.Cos(radians);
    float sin = Mathf.Sin(radians);
    return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
  }
}