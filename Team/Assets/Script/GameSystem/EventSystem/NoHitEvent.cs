using System.Collections;
using UnityEngine;

public class NoHitEvent : EventSystemObject
{
  private bool isFailed = false;
  

  protected override void Start()
  {
    base.Start();
  }

  public override void TriggerEvent()
  {
    if (eventInProgress) return;
    base.TriggerEvent(); // 부모의 기본 설정 (타이머 UI 활성화 등)

    isFailed = false;

    if (playerc != null)
      playerc.OnHit += FailEvent; // 플레이어 피격 시 실패 처리

    StartCoroutine(NoHitRoutine());
  }

  private IEnumerator NoHitRoutine()
  {
    while (eventInProgress && !isFailed)
    {
      yield return null; // 매 프레임 체크
    }

    if (isFailed)
    {
      Fail(); // 실패 처리
    }
    else
    {
      Success(); // 성공 처리
    }
  }

  private void FailEvent()
  {
    isFailed = true;
  }

  protected override void Success()
  {
    if (playerc != null)
      playerc.OnHit -= FailEvent; // 이벤트 구독 해제

    base.Success(); // 보상 생성 + 이벤트 종료 처리
  }

  protected override void Fail()
  {
    if (playerc != null)
      playerc.OnHit -= FailEvent; // 이벤트 구독 해제

    base.Fail(); // 실패 처리 + 이벤트 종료
  }
}