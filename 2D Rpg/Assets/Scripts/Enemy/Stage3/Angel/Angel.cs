using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Angel : EnemyController
{
  public GameObject warningLinePrefab;  // LineRenderer가 붙은 프리팹
  public GameObject attack;             // 총알 프리팹


  protected override void Awake()
  {
    range = 35.0f; // 탐색 범위
    speed = 1.8f; // 이동속도
    att = 5;     // 공격력
    def = 5;     // 방어력
    hp = 35;      // 체력
    exp = 30;     // 경험치
    gold = 10;     // 돈
    defIgn = 15;   // 방어력 무시
    spawnWeight = 8;

    canMove = false;
    canAttack = true;
    attackRange = 100.0f;
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

    Vector3 playerPos = player.transform.position + new Vector3(0, 0.4f); // 플레이어 머리 쪽 조준

    
    yield return StartCoroutine(Attack(transform.position, playerPos));
    isAttacking = false;

    // 공격이 끝난 뒤에도 다시 한 번 체크
    if (currentHp <= 0)
    {
      isAttacking = false;
      isAttack = false;
      yield break;
    }

    state = EnemyState.Idle;
    yield return new WaitForSeconds(3f); // 공격 후 쿨타임

    isAttack = false;
  }

  IEnumerator Attack(Vector3 start, Vector3 player)
  {

    // 1. 경고선 생성
    GameObject warningLine = Instantiate(warningLinePrefab, start, Quaternion.identity);
    LineRenderer line = warningLine.GetComponent<LineRenderer>();

    if (line != null)
    {
      Vector3 startPos = start;
      Vector3 direction = (player - start).normalized; // 플레이어 방향
      Vector3 endPos = start + direction * 100f;         // 100 거리만큼 선 긋기

      line.startWidth = 0.1f;
      line.endWidth = 0.1f;
      line.SetPosition(0, startPos);
      line.SetPosition(1, endPos);

      // 기존 색상 유지, 알파만 0.4로 설정
      Color currentStart = line.startColor;
      Color currentEnd = line.endColor;

      // 알파값만 변경
      currentStart.a = 0.4f;
      currentEnd.a = 0.4f;

      // 다시 적용
      line.startColor = currentStart;
      line.endColor = currentEnd;
      
      
    }

   

    yield return new WaitForSeconds(1.3f); // 1.3초 기다리기

    if (currentHp <= 0)
    {
      Destroy(warningLine); // 경고선 제거
      isAttack = false;
      isAttacking = false;
      anim.SetInteger("State", 4);
      yield break;
    }

    Destroy(warningLine); // 경고선 제거
    GameObject clone = Instantiate(attack, start, Quaternion.identity);
    clone.name = attack.name;

    if (clone.name.Contains("Angel_Bullet")) // 이름에 Angel_Bullet 포함되면
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
