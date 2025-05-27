using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yellow : GunFemale
{
  protected override void Awake()
  {
    base.Awake();

    //Blue처럼 돌진형 적, 체력많고, 공격력 높게 설정
    maxHp *= 2f;
    currentHp = maxHp;
    moveSpeed *= 1.5f;
    attackDem *= 2f;
    defence *= 2f;
    defIgnore = Mathf.Min(100f, defIgnore * 2f); // 최대 100으로 제한
  }
  protected override void Start()
  {
    base.Start();
  }

  protected override void Update()
  {
    if (currentHp <= 0) return;
    base.Update(); //기본이동 + 사거리체크 + Shoot처리
  }

  //발사시 애니메이션만 추가
  protected override void Shoot()
  {
    animator.SetTrigger("Attack"); //화살 쏘는 애니메이션 트리거 추가
    base.Shoot();
  }
}
