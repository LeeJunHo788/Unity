using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : EnemyController
{
  

  public bool IsDead => isDead; //Lock에서 읽기전용으로 참조

  public WaveController waveController; //UI

  protected override void Start()
  {
    base.Start(); //enemycontroller base 가져오기
    waveController = GameObject.Find("WaveUI").GetComponent<WaveController>();
  }

  protected override void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("PlayerAttack"))       // 공격과 부딪혔을 경우
    {
      //중복 피격 방지를 위해 발사체를 먼저 삭제한다
      Destroy(collision.gameObject);

      //여기서부터 슬라이더 체력깎는 관련코드
      BulletController bc = collision.GetComponent<BulletController>();   // 계수를 가져오기 위해 불릿 컨트롤러 가져오기
      float damage = pc.attackDem * bc.damageCoefficient;                 // 받는 피해량 = 플레이어 공격력 * 공격의 계수

      HitEnemy(damage);
    }
  }

  public override void HitEnemy(float damage)
  {

    float finalDamage = Mathf.RoundToInt(damage - (damage * (defence - (defence * (pc.defIgnore * 0.01f))) * 0.01f));
    bool isCritical = Random.value < (pc.criticalChance / 100f);

    // 치명타 일 경우
    if (isCritical)
    {
      finalDamage *= pc.criticalDem;
    }


    // 데미지 텍스트 생성
    GameObject dmgText = Instantiate(damageTextPrefab, transform.position + Vector3.up, Quaternion.identity);
    dmgText.GetComponent<DamageTextController>().SetDamage(finalDamage, isCritical);   // 데미지, 치명타 여부 전달

    currentHp -= Mathf.RoundToInt(finalDamage);          //피해 적용
    slider.value = currentHp; //슬라이더갱신

    if(currentHp < 0)
    {
      Dead();
    }

    //보스FSM용 상태 전환 처리
    Stage1BossController stageBoss = this as Stage1BossController;
    if(stageBoss != null)
    {
      stageBoss.ApplyFSMOnHit();
    }
  }
  public override void Dead()
  {
    isDead = true; //이동막기용
    currentHp = 0; //마이너스로 내려가는거 방지
    rb.velocity = Vector2.zero; //넉백멈춤
    rb.simulated = false; //물리 비활성화
    animator.SetTrigger("Dead"); //Dead 애니메이션 재생

    Debug.Log("사망");

    if(waveController != null)
    {
      //보스 클리어 하면Clear UI뜨게 하기
      // waveController.ShowGameClearUI();
    }
     OnDeath?.Invoke();
  }
  //적 개체 없어지는 이벤트용 함수
  public override void EnemyDie()
  {
    DropExp();      // 경험치 오브젝트 드랍

    pc.killCount++;       // 처치 횟수 추가
    pc.KillCountUpdate(); // 처치 횟수 표시 업데이트

    Destroy(gameObject); //적 삭제

    EnemyController.OnBossDeath?.Invoke();

  }
  // 경험치 오브젝트 드랍
  void DropExp()
  {
    Instantiate(expObj, transform.position, Quaternion.Euler(0, 0, 0));
  }

}
