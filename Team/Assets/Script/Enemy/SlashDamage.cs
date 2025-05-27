using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashDamage : EnemyAttack
{
  public float damage = 10f;
  public float duration = 5f;
  private float timer = 0f;

  private new void Awake()
  {
    speed = 5f;
    lifeTime = 3f;
    attDamage = 5f;       // ������
    DefIgnore = 0f;       // ���¹���
    destroyOnHit = false;     // �ǰ� �� ������Ʈ�� ������ ����
  }

  private new void Update()
  {
    //����Ʈ�̱� ������base.Update�� ��������
    timer += Time.deltaTime;

    if (timer >= duration)
    {
      Destroy(gameObject);
    }

  }
}
