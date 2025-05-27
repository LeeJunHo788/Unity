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

    targetKillCount = baseKillCount + (wave - 1) * 2; // 웨이브별로 목표 증가

    eventNameText.text = $"적 처치 {currentKillCount} / {targetKillCount}";

    EnemyController.OnDeath += OnEnemyKilled; // 전체 적 사망 구독
  }

  private void OnEnemyKilled()
  {
    if (!eventInProgress) return;

    currentKillCount++;
    eventNameText.text = $"적 처치 {currentKillCount} / {targetKillCount}";

    if (currentKillCount >= targetKillCount)
    {
      Success();
    }
  }

  protected override void Success()
  {
    EnemyController.OnDeath -= OnEnemyKilled; // 구독 해제
    base.Success();
  }

  protected override void Fail()
  {
    EnemyController.OnDeath -= OnEnemyKilled; // 구독 해제
    base.Fail();
  }
}