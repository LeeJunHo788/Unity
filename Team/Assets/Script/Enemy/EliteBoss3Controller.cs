using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EliteBoss3State
{
  Idle,
  Walk,
  Run,
  Attack,
  Dead
}
public class EliteBoss3Controller : BossController
{
  //FSM ���� ����
  public EliteBoss3State currentState = EliteBoss3State.Idle;
  private EliteBoss3State lastState;

  public float visionRange = 50f; //�þ� -> �ȱ����
  public float attackDuration = 18f; //20�� �� �ߺ����� ����
  private float attackTimer;   //���� Ÿ�̸�
  public GameObject attackEffectPrefab; //��������Ʈ
  public Transform effectSpawnPoint;    //���� �߾� ��ġ

  //Run������ ���� ������
  public float runCooldown = 15f; //15�ʴ����� ����
  private float runTimer; //Ÿ�̸�
  private bool isRunning = false;

  private bool isShield = false; //�ǵ彺ų ��°�?

  public Transform shieldPoint;         //�ǵ� ����Ʈ ��ġ
  public GameObject shieldEffectPrefab; //�ǵ� ����Ʈ ������(�����)

  protected override void Start()
  {
    base.Start();
    currentState = EliteBoss3State.Idle;
    animator.ResetTrigger("Run");
    animator.ResetTrigger("Attack");
    animator.ResetTrigger("Shield");

    currentState = EliteBoss3State.Idle;
    runTimer = runCooldown;
    attackTimer = attackDuration;
  }

  private new void Update()
  {
    if (player == null || isDead || currentState == EliteBoss3State.Dead) return;
    float dis = Vector2.Distance(transform.position, player.transform.position);

    runTimer -= Time.deltaTime;
    attackTimer -= Time.deltaTime;

    if (!canAttack && attackTimer <= 0f)
    {
      canAttack = true;
    }

    switch (currentState)
    {
      case EliteBoss3State.Idle:
      case EliteBoss3State.Walk:
        Walk(); //������ �ȴ� �Լ�

        //�ǵ�
        if (!isShield && currentHp <= maxHp * 0.7f) //ü���� 70������
        {
          StartCoroutine(ShieldBuff());
          isShield = true;
          return;
        }
        //ȸ���� ���� ����
        if (runTimer <= 0f)
        {
          runTimer = runCooldown;
          ChangeState(EliteBoss3State.Run);
          return;
        }
        //��������
        if (!isAttacking && canAttack && attackTimer <= 0f && currentState != EliteBoss3State.Attack)
        {
          isAttacking = true;
          canAttack = false;
          attackTimer = attackDuration; //���⼭ �� �� �ʱ�ȭ
          ChangeState(EliteBoss3State.Attack);
          return;
        }
        //���� ���� �Ǵ�
        if (dis <= visionRange)
          ChangeState(EliteBoss3State.Walk);
        else
          ChangeState(EliteBoss3State.Idle);
        break;
      case EliteBoss3State.Run:
        break;
      case EliteBoss3State.Attack:
        break;
      case EliteBoss3State.Dead:
        break;
    }
  }
  void ChangeState(EliteBoss3State newState)
  {
    if (currentState == newState) return;
    currentState = newState;

      switch (newState)
      {
        case EliteBoss3State.Idle:
          animator.SetBool("isWalking", false);
          break;
        case EliteBoss3State.Walk:
          animator.SetBool("isWalking", true);
          break;
        case EliteBoss3State.Run:
          animator.SetTrigger("Run");
          animator.SetBool("isWalking", false); //run�� �ȴ� ���� �ƴ϶��� ���
          //�Լ�ȣ��
          break;
        case EliteBoss3State.Attack:
          animator.SetTrigger("Attack");
          animator.SetBool("isWalking", false);
        //�Լ�ȣ��
        break;
        case EliteBoss3State.Dead:
          animator.SetTrigger("Dead");
         animator.SetBool("isWalking", false);
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
  
  //�̺�Ʈ�Լ��� ó��
  public void StartRun()
  {
    if (isRunning) return; //�̹� �������̸� �ߺ�����X

    isRunning = true;
    StartCoroutine(RunRoutine());
  }

  IEnumerator RunRoutine()
  {
    moveSpeed *= 1.5f; //���� �ӵ� ����

    float runTime = 8f;
    float timer = 0f;

    while(timer < runTime)
    {
      Walk(); //�̵��Լ�ȣ��
      timer += Time.deltaTime;
      yield return null;
    }

    moveSpeed /= 1.5f;
    isRunning = false;
    runTimer = runCooldown; //�ʱ�ȭ
    
    ChangeState(EliteBoss3State.Walk);
    
  }

  IEnumerator ShieldBuff()
  {
    animator.SetTrigger("Shield"); //�ִϸ��̼� ���
    //���� ����Ʈ ����
    GameObject shield = Instantiate(shieldEffectPrefab, shieldPoint.position, Quaternion.identity, shieldPoint);
    Destroy(shield, 1.5f); //1.5�ʵ� ����

    defence *= 5;         //���� ����
    yield return new WaitForSeconds(30f); //30�� ���� ����
    defence /= 5;         //���� ������� ���ƿ�

  }
  public void EndAttack()
  {
    Debug.Log("EndAttack����");
    Debug.Log($"[��Ÿ�� �ʱ�ȭ] ���� attackTimer: {attackTimer}");

    isAttacking = false;
    attackTimer = attackDuration; //���⼭ ��Ÿ�� �ʱ�ȭ
    Debug.Log($"[��Ÿ�� �ʱ�ȭ �Ϸ�] ���ο� attackTimer: {attackTimer}");
    ChangeState(EliteBoss3State.Walk);

    Debug.Log("");
  }

  public void EnableAttackEffect()
  {
    GameObject effect = Instantiate(attackEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
    var explosion = effect.GetComponent<ExplosionDamage>();
    explosion.useExpansion = false; //Ȥ�� �𸣴� �ڵ�����ε� false����
    explosion.duration = 8f; //8�ʰ�����
    effect.transform.SetParent(transform); //���� ����ٴϵ��� 
  }

  public void ApplyFSMOnHit()
  {
    //������ ó���� ��ӹ޾� �����Ƿ� ����� ���� ��ȯ��
    if (currentHp <= 0 && !isDead)
    {
      isDead = true;
      ChangeState(EliteBoss3State.Dead);

      //�������� Ŭ���� ó�� Ȯ�� ���߿� �����Ҷ� �߰����� 
      //GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    else if (currentState != EliteBoss3State.Run && currentState != EliteBoss3State.Attack && currentState != EliteBoss3State.Dead && isAttacking)
    {
      ChangeState(EliteBoss3State.Walk);
    }
  }
}
