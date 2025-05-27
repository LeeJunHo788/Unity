using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFemale_Attack : EnemyAttack
{
  protected override void Awake()
  {
    base.Awake();

    speed = 7f;
    lifeTime = 3f;
    attDamage = 5f + sm.currentWave;       // ������
    DefIgnore = Mathf.Min(100f, 0f + sm.currentWave); // �ִ� 100���� ����
    destroyOnHit = true;     // �ǰ� �� ������Ʈ�� ������ ����
  }
  protected override void Update()
  {
    base.Update();
  }
 
}
