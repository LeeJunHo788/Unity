using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeathBringer_Attack2 : EnemyAttackController
{
  BoxCollider2D box;

  protected override void Start()
  {
    att = 45;     // ���ݷ�
    defIgn = 50;   // ���� ����

    box = GetComponent<BoxCollider2D>();
    box.gameObject.SetActive(false);

    StartCoroutine(ColliderOn());

  }

  IEnumerator ColliderOn()
  {
    yield return new WaitForSeconds(1.85f);
    box.gameObject.SetActive(true);
  }
}
