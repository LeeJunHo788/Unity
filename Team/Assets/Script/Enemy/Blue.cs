using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blue : EnemyController
{
  //������ �� ���� ����
  Vector2 dashDir;          //���� ���� ����
  bool isDashing = false;   //���� ���� ���ΰ�?
  float dashSpeed = 10f;     //���� ���ǵ�
  float dashTime = 0.5f;    //���� ���� �ð�
  float dashTimer = 0f;     // ���� Ÿ�̸�(�����ð� ���� ��������)
  float dashCooldown = 1f;  // ���� ��Ÿ��
  

  float prepareTime = 0.5f;   //���� �� �غ�ð�
  float prepareTimer = 0f;    //���� Ÿ�̸�
  bool isPreparing = false;   //���� �� �غ����
  bool canDash = true;        // ���� ���� ����

  public Animator dashAnimator;
  protected virtual void Awake()
  {
    sm = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

    maxHp = 10f + sm.currentWave;
    currentHp = maxHp;
    moveSpeed = 2f;
    attackDem = 5f + sm.currentWave;
    defence = 10f + sm.currentWave;
    defIgnore = Mathf.Min(100f, 10f + sm.currentWave); // �ִ� 100���� ����
  }
  protected override void Start()
  {
    base.Start();
  }

  protected override void Update()
  {
    if (isDead) return; //�׾����� ������ X

    if(!isDashing && !isPreparing)
      base.Update();

    //��� ���̰ų� �غ����� �ƴ϶��
    if (!isDashing && !isPreparing && canDash)
    {
      //�÷��̾� ��ġ ã��
      float distance = Vector2.Distance(transform.position, player.transform.position);

      //��������
      if (distance < 4f)
      {

        //���� �� �غ� ����
        dashDir = (player.transform.position - transform.position).normalized;
        isPreparing = true;
        prepareTimer = prepareTime;


        //��� idle���·� ��ȯ
        dashAnimator.SetBool("isWalking", false);
      }
    }
    //���� �غ���
    else if (!isDashing && isPreparing)
    {
      prepareTimer -= Time.deltaTime;

      if (prepareTimer <= 0f)
      {
        //���� ����
        isPreparing = false;
        isDashing = true;
        dashTimer = dashTime;
        StartCoroutine(DashColldown());

        //�ٽ� �ȴ� ���·� ��ȯ
        dashAnimator.SetBool("isWalking", true);

      }
    }
    //���� ��
    else if (isDashing)
    {
      rb.velocity = Vector2.zero; //���� �� �⺻ �̵� ����
      //���� �߿��� ������� �� ���
      rb.velocity = dashDir * dashSpeed;
      dashTimer -= Time.deltaTime;


      if (dashTimer <= 0f)
      {
        rb.velocity = Vector2.zero;
        isDashing = false; //���� ���� (�ٽ� Enemy Controller��� ������)
      }
    }
  }
  //����� ���� �ߴ�
  public override void Dead()
  {
    isDashing = false;
    isPreparing = false;

    base.Dead(); //EnemyController�� ���ó��
  }

  IEnumerator DashColldown()
  {
    canDash = false;
    yield return new WaitForSeconds(dashCooldown);
    canDash = true;
  }

  protected override void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("PlayerAttack"))       // ���ݰ� �ε����� ���
    {
      if(!isPreparing)    // ���� �غ��߿��� �˹� ��Ȱ��ȭ
      {
        Vector2 dir = (collision.transform.position - player.transform.position).normalized;    // �о ����

        rb.AddForce(dir * pc.knockBackForce, ForceMode2D.Impulse);             // �˹�
        StartCoroutine(StopEnemy(rb));                            // 0.2���� ����

      }

      //���⼭���� �����̴� ü�±�� �����ڵ�
      BulletController bc = collision.GetComponent<BulletController>();   // ����� �������� ���� �Ҹ� ��Ʈ�ѷ� ��������

      if (bc == null || bc.hasHit) return;  // �̹� �ǰ��� �Ѿ��̶�� �ǰ�ȿ�� ����

      bc.hasHit = true;  // �ǰ� ó�� ǥ��

      //�߻�ü ����
      Destroy(collision.gameObject);

      float damage = pc.attackDem * bc.damageCoefficient;                 // �޴� ���ط� = �÷��̾� ���ݷ� * ������ ���

      // ���� ������
      float finalDamage = Mathf.RoundToInt(damage - (damage * (defence - (defence * (pc.defIgnore * 0.01f))) * 0.01f));

      bool isCritical = Random.value < (pc.criticalChance / 100f);        // ġ��Ÿ Ȯ�� ����

      // ġ��Ÿ���
      if (isCritical)
      {
        finalDamage *= pc.criticalDem;     // ������ ����
      }


      currentHp -= Mathf.RoundToInt(finalDamage);          //���� ����
      slider.value = currentHp; //�����̴�����
      StartCoroutine(MaterialShift());

      if (currentHp <= 0)
      {
        Dead();
      }

      // ������ �ؽ�Ʈ ����
      GameObject dmgText = Instantiate(damageTextPrefab, transform.position + Vector3.up, Quaternion.identity);
      dmgText.GetComponent<DamageTextController>().SetDamage(finalDamage, isCritical);   // ������, ġ��Ÿ ���� ����
    }
  }


}
   
