using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

public enum Boss1State
{
  Idle,
  Walk,
  Attack,
  Dead
}
public class Stage1BossController : BossController
{
  //FSM ���� ����
  public Boss1State currentState = Boss1State.Idle;
  private Boss1State lastState;

  public float visionRange = 50f; //�þ�
  public float attackRange = 5f; //���� ���� ����

  public float attackDuartion = 1f; //��Ÿ��

  bool alternatePattern = true; //true = ����, false=�̸�

  //���� ����Ʈ ������
  public GameObject explosionEffect;
  public Transform[] explosionPoints; //������ �ֺ��� ��ġ�� SpawnPoint
  public Transform[] outerExplosionPoints; //2��(�ٱ���)
  public float explosionDelay = 0.75f; //1,2�� ���� �ð���

  //�̸� ��ȯ ������
  public GameObject minionPrefab;     //��ȯ�� �̸� ������
  public Transform[] minionSpawnPoints; //������ġ


  protected override void Start()
  {
    base.Start();
    currentState = Boss1State.Idle;
  }

  private new void Update()
  {
    if (player == null || isDead) return;

    float dis = Vector2.Distance(transform.position, player.transform.position);

    switch (currentState)
    {
      case Boss1State.Idle:
        if (dis <= visionRange) ChangeState(Boss1State.Walk);
        break;
      case Boss1State.Walk:
        if (dis <= attackRange && canAttack) ChangeState(Boss1State.Attack);
        else Chase();
        break;
      case Boss1State.Attack:
        Attack();
        break;
    }
  }

  void ChangeState(Boss1State newState)
  {
    if (currentState == newState) return;
    currentState = newState;

    switch (newState)
    {
      case Boss1State.Idle:
        animator.SetInteger("State", 0);
        break;
      case Boss1State.Walk:
        animator.SetInteger("State", 1);
        break;
      case Boss1State.Attack:
        animator.SetInteger("State", 2);
        break;
      case Boss1State.Dead:
        animator.SetTrigger("Dead");
        Dead(); //��ӹ޴� BossController�� Dead�Լ� ȣ��
        break;
    }
  }

  void Chase()
  {
    //EnemyController�� �̵� �Լ��� ����

    dir = (player.transform.position - transform.position).normalized;    // �÷��̾ ���ϴ� ����
    transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);    // �̵�

    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;              // ���� ���ϱ�

    animator.SetInteger("State", 1); //Walk�ִϸ��̼����� ����

    //���⿡ ���� flipó��
    if (dir.x >= 0)
    {
      transform.localScale = new Vector3(1, 1, 1);//������
      slider.transform.localScale = new Vector3(Mathf.Abs(originalSliderScale.x), originalSliderScale.y, originalSliderScale.z);
    }
    else
    {
      transform.localScale = new Vector3(-1, 1, 1); //�¿����
      slider.transform.localScale = new Vector3(-Mathf.Abs(originalSliderScale.x), originalSliderScale.y, originalSliderScale.z);
    }
  }
  void Attack()
  {
    animator.SetInteger("State", 2); //�ִϸ��̼� ����
    if (!isAttacking)
    {
      isAttacking = true;
      StartCoroutine(AttackCoroutine());
    }
  }

  IEnumerator AttackCoroutine()
  {
    canAttack = false;
    yield return new WaitForSeconds(0.6f);//���ݾִϸ��̼� ���� �� ���� ���
    currentState = Boss1State.Walk; //Walk����ȯ

    yield return new WaitForSeconds(attackDuartion); //��Ÿ��

    isAttacking = false;
    canAttack = true;
  }
  //�ִϸ��̼� �̺�Ʈ�� ȣ��� �Լ�
  public void AttackTrigger()
  {
    if (currentHp <= 200)
    {
      ExplosionAttack(); //����Ʈ ���� ����
    }

    if(currentHp <= 100) //ü�� 50%���Ϸ� �������� ����Ʈ ����
    {
      DoubleExplosion(); //���� ����Ʈ ���� ����
    }

    else if (currentHp <= 50f) //ü�� 30% ���ϸ� �̸� ��ȯ�� �߰�
    {
      if (alternatePattern)
      {
        SpawnMinion();
      }
      alternatePattern = !alternatePattern; //true/false��ȯ
    }
  }

  void ExplosionAttack()
  {
    foreach (Transform point in explosionPoints)
    {
      Instantiate(explosionEffect, point.position, Quaternion.identity);
    }
  }

  void DoubleExplosion()
  {
    StartCoroutine(DoubleExplosionRoutine());
  }

  IEnumerator DoubleExplosionRoutine()
  {
    //1�� ���� ����
    foreach (Transform point in explosionPoints)
    {
      Instantiate(explosionEffect, point.position, Quaternion.identity);
    }

    yield return new WaitForSeconds(explosionDelay); //�ð���

    //2�� ����
    foreach(Transform point in outerExplosionPoints)
    {
      Instantiate(explosionEffect, point.position, Quaternion.identity);
    }
  }

  void SpawnMinion()
  {
    if (minionSpawnPoints.Length == 0) return; //���� ����Ʈ ������ ����X
    
    foreach(Transform point in minionSpawnPoints)
    {
      Instantiate(minionPrefab, point.position, Quaternion.identity);
    }
   
  }

  public void ApplyFSMOnHit()
  {
    //������ ó���� ��ӹ޾� �����Ƿ� ����� ���� ��ȯ��
    if (currentHp <= 0 && !isDead)
    {
      isDead = true;
      ChangeState(Boss1State.Dead);
    }
    else if (currentState != Boss1State.Attack && currentState != Boss1State.Dead)
    {
      currentState = Boss1State.Walk;
    }
  }
}
