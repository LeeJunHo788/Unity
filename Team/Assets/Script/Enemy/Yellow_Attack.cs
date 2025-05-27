using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yellow_Attack : GunFemale_Attack
{
  private Animator arrowAnimator; //화살에 연결되어있는 애니메이션
  protected override void Awake()
  {
    base.Awake();

    speed = 10f;
    lifeTime = 3f;
    attDamage = 10f + sm.currentWave * 2;      // 데미지 (GunFemale발사체보다 조금 높음)
    DefIgnore = Mathf.Min(100f, 0f + sm.currentWave*2); // 최대 100으로 제한
    destroyOnHit = true;     // 피격 시 오브젝트를 없앨지 여부
  }

  private void Start()
  {
    arrowAnimator = GetComponent<Animator>();
    arrowAnimator.SetTrigger("Fly"); //애니메이션 트리거 
  }

  protected override void Update()
  {
    base.Update();
  }
}
