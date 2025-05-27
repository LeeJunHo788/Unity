using System.Collections;
using UnityEngine;

public class ChaseEvent : EventSystemObject
{
  public GameObject targetPrefab; // �������� ��� ������
  public float spawnDistance = 3f;

  private bool isCaught = false;
  private GameObject target;

  public override void TriggerEvent()
  {
    if (eventInProgress) return;
    base.TriggerEvent(); // �θ� Ŭ������ �⺻ ���� (UI Ȱ��ȭ ��)

    isCaught = false;

    // Ÿ�� ����
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
      Success(); // ���� ó��
    }
    else
    {
      Fail(); // ���� ó��
    }
  }

  public void OnCatch()
  {
    isCaught = true;
  }

  protected override void Success()
  {
    if (target != null)
      Destroy(target); // Ÿ�� ����

    base.Success(); // ���� ���� + �̺�Ʈ ���� ó��
  }

  protected override void Fail()
  {
    if (target != null)
      Destroy(target); // Ÿ�� ����

    base.Fail(); // ���� ó�� + �̺�Ʈ ����
  }
}