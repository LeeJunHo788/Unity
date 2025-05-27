using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yellow : GunFemale
{
  protected override void Awake()
  {
    base.Awake();

    //Blueó�� ������ ��, ü�¸���, ���ݷ� ���� ����
    maxHp *= 2f;
    currentHp = maxHp;
    moveSpeed *= 1.5f;
    attackDem *= 2f;
    defence *= 2f;
    defIgnore = Mathf.Min(100f, defIgnore * 2f); // �ִ� 100���� ����
  }
  protected override void Start()
  {
    base.Start();
  }

  protected override void Update()
  {
    if (currentHp <= 0) return;
    base.Update(); //�⺻�̵� + ��Ÿ�üũ + Shootó��
  }

  //�߻�� �ִϸ��̼Ǹ� �߰�
  protected override void Shoot()
  {
    animator.SetTrigger("Attack"); //ȭ�� ��� �ִϸ��̼� Ʈ���� �߰�
    base.Shoot();
  }
}
