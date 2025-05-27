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
    base.TriggerEvent(); // �θ��� �⺻ ���� (Ÿ�̸� UI Ȱ��ȭ ��)

    isFailed = false;

    if (playerc != null)
      playerc.OnHit += FailEvent; // �÷��̾� �ǰ� �� ���� ó��

    StartCoroutine(NoHitRoutine());
  }

  private IEnumerator NoHitRoutine()
  {
    while (eventInProgress && !isFailed)
    {
      yield return null; // �� ������ üũ
    }

    if (isFailed)
    {
      Fail(); // ���� ó��
    }
    else
    {
      Success(); // ���� ó��
    }
  }

  private void FailEvent()
  {
    isFailed = true;
  }

  protected override void Success()
  {
    if (playerc != null)
      playerc.OnHit -= FailEvent; // �̺�Ʈ ���� ����

    base.Success(); // ���� ���� + �̺�Ʈ ���� ó��
  }

  protected override void Fail()
  {
    if (playerc != null)
      playerc.OnHit -= FailEvent; // �̺�Ʈ ���� ����

    base.Fail(); // ���� ó�� + �̺�Ʈ ����
  }
}