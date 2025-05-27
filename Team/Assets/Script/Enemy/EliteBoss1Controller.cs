using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EliteBoss1State
{
  Idle,
  Walk,
  Attack,
  Monster, //�̸� ��ȯ
  Dead
}

public class EliteBoss1Controller : BossController
{
  //FSM���ı���
  public EliteBoss1State currentState = EliteBoss1State.Idle;
  private EliteBoss1State lastState;

  public float visionRange = 50f; //�þ�(������ �Ѿƿ����� �аԼ���)

  //���� ���� ����
  public GameObject attackPrefabAnim; //���� ����Ʈ ������ �ִϸ��̼�
  public GameObject attackPrefabSprite; //���� ���� ����Ʈ ��������Ʈ
  public Transform attackPoint;   //����Ʈ ������ ��ġ
  public float attackDuration = 5f; //���� �� 5��
  private float attackTimer;        //���� Ÿ�̸�

  //�̸� ��ȯ ����
  public GameObject monsterPrefab; //��ȯ�� �̸� ������
  public Transform center;         //��ȯ �߽� ����Ʈ
  public int summonCount = 5;     //��ȯ�� �̸� ��
  public float summonRadius = 3f; //��ȯ ������
  bool isSummoning = false; //��ȯ������?
  bool hasSummoned = false; //��ȯ�ߴ°�?

  protected override void Start()
  {
    base.Start();

    currentState = EliteBoss1State.Idle;
    attackTimer = attackDuration; //Ÿ�̸� �ʱ�ȭ
  }
  private new void Update()
  {
    if (player == null || isDead || currentState == EliteBoss1State.Dead) return;
    float dis = Vector2.Distance(transform.position, player.transform.position);

    //�ʱ�ȭ
    attackTimer -= Time.deltaTime;

    switch(currentState)
    {
      case EliteBoss1State.Idle:
        if (dis <= visionRange)
          ChangeState(EliteBoss1State.Walk);
        break;
      case EliteBoss1State.Walk:
        Walk(); //������ �ȴ� �Լ�
        if(currentHp <= maxHp * 0.3f && !hasSummoned) //ü��30����+��ȯ�ѹ�����������
        {
          ChangeState(EliteBoss1State.Monster);
        }
        //���� �� �� ���� ����
        else if (attackTimer <= 0f)
        {
          ChangeState(EliteBoss1State.Attack);
        }
        break;
      case EliteBoss1State.Attack:
        if (!isAttacking)//���� ���� �ƴ� ����
        {
          //���� ����Ʈ �߻�
          Attack();
        }
        break;
      case EliteBoss1State.Monster:
        if (!isSummoning) //��ȯ���� �ƴҶ���
        {
          //�̸� ��ȯ
          SummonMonsters();
        }
        break;
      case EliteBoss1State.Dead:
        break;
    }
  }

  void ChangeState(EliteBoss1State newState)
  {
    if (currentState == newState) return;
    currentState = newState;

    switch (newState)
    {
      case EliteBoss1State.Idle:
        animator.SetBool("isWalking", false);
        break;
      case EliteBoss1State.Walk:
        animator.SetBool("isWalking", true);
        break;
      case EliteBoss1State.Attack:
        animator.SetTrigger("Attack");
        animator.SetBool("isWalking", false); //run�� �ȴ� ���� �ƴ϶��� ���
                                              //�Լ�ȣ��
        break;
      case EliteBoss1State.Monster:
        animator.SetTrigger("Monster");
        animator.SetBool("isWalking", false);
        //�Լ�ȣ��
        break;
      case EliteBoss1State.Dead:
        animator.SetTrigger("Dead");
        Dead(); //��ӹ޴� BossController�� Dead�Լ� ȣ��
        break;
    }
  }
  //Walk�Լ��� �������� ������� ����
  public void Walk()
  {
    dir = (player.transform.position - transform.position).normalized;    // �÷��̾ ���ϴ� ����
    transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);    // �̵�

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

  //���� �Լ�
  void Attack()
  {
    isAttacking = true; //���ݽ���

    if(attackPrefabAnim != null && attackPoint != null)
    {
      GameObject animObj = Instantiate(attackPrefabAnim, attackPoint.position, Quaternion.identity);
      // �÷��̾� �߽� ���� ���
      Vector2 targetPos = player.GetComponent<SpriteRenderer>().bounds.center;
      Vector2 firePos = attackPoint.position;
      Vector2 dir = (targetPos - firePos).normalized;

      // ȸ�� ����
      float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      animObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

      Animator anim = animObj.GetComponent<Animator>();
      if(anim != null)
      {
        anim.SetTrigger("Slash");
      }

      Destroy(animObj, 0.5f); //�ִϸ��̼� 0.5���� �ڵ� ����

      Invoke(nameof(SpawnSpriteAttack), 0.5f); //���� �ʰ� ���ÿ� ��������Ʈ �߻�
    }
  }

  //���� ���� ����Ʈ �߻�
  void SpawnSpriteAttack()
  {
    if(attackPrefabSprite != null && attackPoint != null)
    {
      GameObject spriteObj = Instantiate(attackPrefabSprite, attackPoint.position, Quaternion.identity);

      //�÷��̾� �߽� ��ġ�� ��Ȯ�� ���� ���
      Vector2 targetPos = player.GetComponent<SpriteRenderer>().bounds.center;
      Vector2 firePos = attackPoint.position;
      Vector2 dir = (targetPos - firePos).normalized;

      // ��������Ʈ ���� ���߱� (ȸ��)
      float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      spriteObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

      //�̵�
      spriteObj.GetComponent<Rigidbody2D>().velocity = dir * 4f; //�ӵ� ����

      attackTimer = attackDuration; //���� ��Ÿ�� ����
      isAttacking = false; //������ ������
      ChangeState(EliteBoss1State.Walk); //�ٽ� �ȱ�
    }
  }
  //�̸� ��ȯ �Լ�
  void SummonMonsters()
  {
    isSummoning = true; //��ȯ��
    hasSummoned = true; //��ȯ�ƴٰ� ǥ��

    if(monsterPrefab != null && center != null)
    {
      //�ݺ������� ��ȯ
      for(int i = 0; i < summonCount; i++)
      {
        Vector2 randomPos = Random.insideUnitCircle * summonRadius;
        Instantiate(monsterPrefab, center.position + (Vector3)randomPos, Quaternion.identity);
      }
    }
    attackTimer = attackDuration; //���ʵ��� �Ȱ� �ϱ� ���ؼ� attack��Ÿ�� ����
    ChangeState(EliteBoss1State.Walk); //��ȯ ������ �ٽ� �ȱ�
  }
  public void ApplyFSMOnHit()
  {
    //������ ó���� ��ӹ޾� �����Ƿ� ����� ���� ��ȯ��
    if (currentHp <= 0 && !isDead)
    {
      isDead = true;
      ChangeState(EliteBoss1State.Dead);
    }
    else if (currentState != EliteBoss1State.Attack && currentState != EliteBoss1State.Monster && currentState != EliteBoss1State.Dead && isAttacking)
    {
      ChangeState(EliteBoss1State.Walk);
    }
  }
}
