using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Blue
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
    defIgnore *= 2f;
  }
  protected override void Start()
  {
    base.Start();
  }
  protected override void Update()
  {
    base.Update();
  }
}
