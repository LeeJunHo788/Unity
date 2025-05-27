using System.Collections;
using UnityEngine;

public class ChaseEvent : EventSystemObject
{
  public GameObject targetPrefab; // 도망가는 대상 프리팹
  public float spawnDistance = 3f;

  private bool isCaught = false;
  private GameObject target;

  public override void TriggerEvent()
  {
    if (eventInProgress) return;
    base.TriggerEvent(); // 부모 클래스의 기본 세팅 (UI 활성화 등)

    isCaught = false;

    // 타겟 생성
    Vector3 spawnPos = transform.position + (Vector3)(Random.insideUnitCircle.normalized * spawnDistance);
    target = Instantiate(targetPrefab, spawnPos, Quaternion.identity);

    StartCoroutine(ChaseRoutine());
  }

  private IEnumerator ChaseRoutine()
  {
    while (eventInProgress && !isCaught)
    {
      yield return null;
    }

    if (isCaught)
    {
      Success(); // 성공 처리
    }
    else
    {
      Fail(); // 실패 처리
    }
  }

  public void OnCatch()
  {
    isCaught = true;
  }

  protected override void Success()
  {
    if (target != null)
      Destroy(target); // 타겟 삭제

    base.Success(); // 보상 생성 + 이벤트 종료 처리
  }

  protected override void Fail()
  {
    if (target != null)
      Destroy(target); // 타겟 삭제

    base.Fail(); // 실패 처리 + 이벤트 종료
  }
}