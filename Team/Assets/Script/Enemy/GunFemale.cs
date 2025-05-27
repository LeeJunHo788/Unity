using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFemale : EnemyController
{
  //발사체형 적 전용 변수  
  EnemyController enemy;                //참조용 적 컨트롤러
  public GameObject enemyBulletPrefab; //발사체 프리팹
  public Transform firePoint;          //총알 나가는 위치

  public float shootRange = 5f;        //사거리
  public float shootCooldown = 2f;     //무한발사 방지용 쿨
  float shootTimer = 0f;               //타이머

  protected virtual void Awake()
  {
    sm = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    maxHp = 15f + sm.currentWave;
    currentHp = maxHp;
    moveSpeed = 2.5f;
    attackDem = 5f + sm.currentWave;
    defence = 0f + sm.currentWave;
    defIgnore = Mathf.Min(100f, 10f + sm.currentWave); // 최대 100으로 제한
  }
  protected override void Start()
  {
    base.Start();
    enemy = GetComponent<EnemyController>();
  }

  protected override void Update()
  {
    base.Update();

    shootTimer -= Time.deltaTime;

    //플레이어와의 거리 구하기
    float distance = Vector2.Distance(transform.position, player.transform.position);

    if(distance < shootRange)
    {
      rb.velocity = Vector2.zero; //이동정지(이동보다 shoot이 우선시되도록)
      if(shootTimer <= 0f )
      {
        Shoot();
        shootTimer = shootCooldown;
      }
    }
  }


  protected virtual void Shoot()
  {
    //정확한 플레이어 중심위치계산(플레이어를 디테일하게 향하도록)
    Vector2 targetPos = player.GetComponent<SpriteRenderer>().bounds.center;
    Vector2 firePos = firePoint.position;

    //방향계산
    Vector2 dir = (targetPos - firePos).normalized;

    //총알생성
    GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);

    EnemyAttack ea = bullet.GetComponent<EnemyAttack>();
    ea.SetDirection(dir);

    Destroy(bullet, 3f); //3초뒤 제거
  }
  
}
   
