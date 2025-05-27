using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum Boss3State
{
  Idle,
  Walk,
  Explosion,
  Heal,
  Dead
}
public class Stage3BossController : BossController
{
  //FSM ���� ����
  public Boss3State currentState = Boss3State.Idle;
  private Boss3State lastState;

  public float visionRange = 10f; //�þ�

  //Explosion���� ����
  public GameObject warningEffectPrefab;
  public GameObject explosionEffectPrefab;
  public float explosionCooldown = 10f; //10�ʿ� �� ���� ����
  private float explosionTimer;         //���� Ÿ�̸�
  float explosionDuration = 2f;         //�����߿� ��������
  float explosionStateTimer = 0f;
  
  public Transform[] spawnPoints; //Explosion ���� ��ġ �ĺ���
  

  //���� ���� ����
  public float healCooldown = 15f; //15�ʿ� �� ���� ����
  private float healTimer;         //��Ÿ�̸�
  public Transform healPoint;     //�� ����Ʈ ��ġ 
  public GameObject healEffectPrefab; //�� ����Ʈ ������(�����)

  protected override void Start()
  {
    base.Start();
    currentState = Boss3State.Idle;

    //Ÿ�̸� �ʱ�ȭ
    healTimer = healCooldown; 
    explosionTimer = explosionCooldown; 
  }
  private new void Update()
  {
    if (player == null || isDead || currentState == Boss3State.Dead) return;

    healTimer -= Time.deltaTime;
    explosionTimer -= Time.deltaTime;

    float dis = Vector2.Distance(transform.position, player.transform.position);

    switch (currentState)
    {
      case Boss3State.Idle:
      case Boss3State.Walk:
        Walk();
        //��������
        if (healTimer <= 0f)
        {
          ChangeState(Boss3State.Heal); // 15�� ����ϸ� ���� ����
          healTimer = healCooldown;     // ���� �� ��Ÿ�� �ʱ�ȭ
        }
        //��������
        if(explosionTimer <= 0f)
        {
          ChangeState(Boss3State.Explosion);
          //ü�� 300 ���Ϸ� �������� ��Ÿ�� 6�ʷ� ����, �� ���� ����
          explosionTimer = (currentHp <= 300f) ? 6f : explosionCooldown;
          explosionStateTimer = explosionDuration; //������ ��������
        }

        //�÷��̾�� ������ Walk����
        if(dis <= visionRange)
        {
          ChangeState(Boss3State.Walk);
        }
        else
        {
          ChangeState(Boss3State.Idle);
        }
          break;
      case Boss3State.Heal:
        //�Ϸ�� �̺�Ʈ�Լ��� ȣ���Ұű⶧���� ����
        break;
      case Boss3State.Explosion:
        explosionStateTimer -= Time.deltaTime;
        if(explosionStateTimer <= 0f)
        {
          ChangeState(Boss3State.Idle);
        }
        break;
      case Boss3State.Dead:
          break;
    }
  }

  void ChangeState(Boss3State newState)
  {
    if (currentState == newState) return;
    currentState = newState;

    switch(newState)
    {
      case Boss3State.Idle:
        animator.SetBool("isWalking", false);
        break;
      case Boss3State.Walk:
        animator.SetBool("isWalking", true);
        break;
      case Boss3State.Explosion:
        animator.SetTrigger("Explosion");
        TriggerExplosion();
        break;
      case Boss3State.Heal:
        animator.SetTrigger("Heal");
        break;
      case Boss3State.Dead:
        animator.SetTrigger("Dead");
        Dead(); //��ӹ޴� BossController�� Dead�Լ� ȣ��
        break;
    }
  }

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
  //Heal�ִϸ��̼ǿ��� �̺�Ʈ�Լ� ȣ���
  public void HealComplete()
  {
    float healAmount = 300f;
    currentHp = Mathf.Min(currentHp + healAmount, maxHp);

    if(healPoint != null && healEffectPrefab != null)
    {
      GameObject healEffect = Instantiate(healEffectPrefab, healPoint.position, Quaternion.identity, healPoint);
      Destroy(healEffect, 1.5f); //����
    }

    //�� �ϴ� ���� ��������� �ٲ�
    StartCoroutine(TurnPurple());

    ChangeState(Boss3State.Idle);
  }

  private IEnumerator TurnPurple()
  {
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    Color originalColor = sr.color;

    //������
    sr.color  = new Color(0.6f, 0.3f, 0.9f, 0.9f);

    yield return new WaitForSeconds(1.5f); //����Ʈ ���ӽð��� ����

    sr.color = originalColor;
  }

  //Explosion ���� (��� + ����)
  public void TriggerExplosion()
  {
    Debug.Log("���� ����");
    // SpawnPoint �ߺ� ���� 3�� ���� ����
    List<int> indexPool = new List<int>();
    for (int i = 0; i < spawnPoints.Length; i++) indexPool.Add(i);

    int spawnCount = Mathf.Min(3, spawnPoints.Length); // Ȥ�� 3�� �̸��� ��� ���
    List<Transform> selectedPoints = new List<Transform>();

    for (int i = 0; i < spawnCount; i++)
    {
      int rndIdx = Random.Range(0, indexPool.Count);
      int selected = indexPool[rndIdx];
      selectedPoints.Add(spawnPoints[selected]);
      indexPool.RemoveAt(rndIdx);
    }

    // �� ��ġ�� ���, 1�� �� ����
    foreach (Transform point in selectedPoints)
    {
      Vector3 spawnPos = point.position;
      GameObject warning = Instantiate(warningEffectPrefab, spawnPos, Quaternion.identity); // ���
      Destroy(warning, 1.5f); //�غ� �������� 1.5�ʵ� �ڵ�����

      StartCoroutine(DelayedExplosion(spawnPos));
    }
  }

  IEnumerator DelayedExplosion(Vector3 pos)
  {
    yield return new WaitForSeconds(1f);
    Instantiate(explosionEffectPrefab, pos, Quaternion.identity);
    ChangeState(Boss3State.Idle);
  }

  public void ApplyFSMOnHit()
  {
    //������ ó���� ��ӹ޾� �����Ƿ� ����� ���� ��ȯ��
    if (currentHp <= 0 && !isDead)
    {
      isDead = true;
      ChangeState(Boss3State.Dead);
    }
    else if (currentState != Boss3State.Explosion && currentState != Boss3State.Heal && currentState != Boss3State.Dead)
    {
      currentState = Boss3State.Walk;
    }
  }
}
