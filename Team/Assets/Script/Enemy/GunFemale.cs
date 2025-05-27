using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFemale : EnemyController
{
  //�߻�ü�� �� ���� ����  
  EnemyController enemy;                //������ �� ��Ʈ�ѷ�
  public GameObject enemyBulletPrefab; //�߻�ü ������
  public Transform firePoint;          //�Ѿ� ������ ��ġ

  public float shootRange = 5f;        //��Ÿ�
  public float shootCooldown = 2f;     //���ѹ߻� ������ ��
  float shootTimer = 0f;               //Ÿ�̸�

  protected virtual void Awake()
  {
    sm = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    maxHp = 15f + sm.currentWave;
    currentHp = maxHp;
    moveSpeed = 2.5f;
    attackDem = 5f + sm.currentWave;
    defence = 0f + sm.currentWave;
    defIgnore = Mathf.Min(100f, 10f + sm.currentWave); // �ִ� 100���� ����
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

    //�÷��̾���� �Ÿ� ���ϱ�
    float distance = Vector2.Distance(transform.position, player.transform.position);

    if(distance < shootRange)
    {
      rb.velocity = Vector2.zero; //�̵�����(�̵����� shoot�� �켱�õǵ���)
      if(shootTimer <= 0f )
      {
        Shoot();
        shootTimer = shootCooldown;
      }
    }
  }


  protected virtual void Shoot()
  {
    //��Ȯ�� �÷��̾� �߽���ġ���(�÷��̾ �������ϰ� ���ϵ���)
    Vector2 targetPos = player.GetComponent<SpriteRenderer>().bounds.center;
    Vector2 firePos = firePoint.position;

    //������
    Vector2 dir = (targetPos - firePos).normalized;

    //�Ѿ˻���
    GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);

    EnemyAttack ea = bullet.GetComponent<EnemyAttack>();
    ea.SetDirection(dir);

    Destroy(bullet, 3f); //3�ʵ� ����
  }
  
}
   
