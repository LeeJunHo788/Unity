using UnityEngine;

public class KillEnemiesEvent : EventSystemObject
{
  int baseKillCount = 40;
  int targetKillCount = 100;
  private int currentKillCount = 0;

  public override void TriggerEvent()
  {
    base.TriggerEvent();
    currentKillCount = 0;

    SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
    int wave = spawnManager != null ? spawnManager.currentWave : 1;

    targetKillCount = baseKillCount + (wave - 1) * 2; // ���̺꺰�� ��ǥ ����

    eventNameText.text = $"�� óġ {currentKillCount} / {targetKillCount}";

    EnemyController.OnDeath += OnEnemyKilled; // ��ü �� ��� ����
  }

  private void OnEnemyKilled()
  {
    if (!eventInProgress) return;

    currentKillCount++;
    eventNameText.text = $"�� óġ {currentKillCount} / {targetKillCount}";

    if (currentKillCount >= targetKillCount)
    {
      Success();
    }
  }

  protected override void Success()
  {
    EnemyController.OnDeath -= OnEnemyKilled; // ���� ����
    base.Success();
  }

  protected override void Fail()
  {
    EnemyController.OnDeath -= OnEnemyKilled; // ���� ����
    base.Fail();
  }
}