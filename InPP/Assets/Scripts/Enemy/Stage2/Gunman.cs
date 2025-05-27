using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gunman : EnemyController
{

  public GameObject attack;


  protected override void Awake()
  {
    range = 35.0f; // Ž�� ����
    speed = 2.1f; // �̵��ӵ�
    att = 5;     // ���ݷ�
    def = 5;     // ����
    hp = 17;      // ü��
    exp = 15;     // ����ġ
    gold = 5;     // ��
    defIgn = 15;   // ���� ����

    canAttack = true;
    attackRange = 8.0f;
  }

  protected override void Start()
  {
    currentHp = hp;
    base.Start();

    // �ִϸ������� ��Ʈ�ѷ����� ��� �ִϸ��̼� Ŭ�� ��������
    RuntimeAnimatorController controller = anim.runtimeAnimatorController;
    AnimationClip[] clips = controller.animationClips;

    foreach (AnimationClip clip in clips)
    {
      // ���� �ִϸ��̼��� ���� ��������
      if (clip.name == mobName + "_Attack")
      {
        attackTime = clip.length;
        break;
      }
    }

  }

  protected override void Update()
  {
    base.Update();
  }

  protected override IEnumerator PerformAttack()
  {

    anim.SetTrigger("Attack");


    isAttack = true;
    isAttacking = true;
    rb.linearVelocity = Vector2.zero;
    state = EnemyState.Attack;

    Vector2 hitDir = (player.transform.position - transform.position);

    if (hitDir.x > 0)
    {
      transform.rotation = Quaternion.Euler(0, 0, 0);
      Vector2 pos = transform.position + new Vector3(0.4f, 0.2f);
      // �����̴��� ȸ��ȿ�� ����
      hpSlider.transform.localRotation = Quaternion.Euler(0, 0, 0);

      StartCoroutine(Attack(pos));

    }

    else if (hitDir.x <= 0)
    {
      transform.rotation = Quaternion.Euler(0, 180, 0);
      Vector2 pos = transform.position + new Vector3(-0.4f, 0.2f);
      // �����̴��� ȸ��ȿ�� ����
      hpSlider.transform.localRotation = Quaternion.Euler(0, 180, 0);

      StartCoroutine(Attack(pos));

    }


    yield return new WaitForSeconds(attackTime / 0.6f); // ���� �� ��Ÿ��
    isAttacking = false;

    // ������ ���� �ڿ��� �ٽ� �� �� üũ
    if (currentHp <= 0)
    {
      isAttacking = false;
      isAttack = false;
      yield break;
    }

    state = EnemyState.Idle;
    yield return new WaitForSeconds(1f); // ���� �� ��Ÿ��

    isAttack = false;
  }

  IEnumerator Attack(Vector2 pos)
  {

    Vector2 savedPlayerPos = player.transform.position;   // �÷��̾� ��ġ ����

    yield return new WaitForSeconds(0.85f); // ��� �غ�

    if (currentHp <= 0)
    {
      isAttack = false;
      isAttacking = false;
      anim.SetInteger("State", 4);
      yield break;
    }

    GameObject clone1 = Instantiate(attack, pos, Quaternion.identity);
    GameObject clone2 = Instantiate(attack, pos, Quaternion.identity);
    GameObject clone3 = Instantiate(attack, pos, Quaternion.identity);

    
    BulletController bullet1 = clone1.GetComponent<BulletController>();
    BulletController bullet2 = clone2.GetComponent<BulletController>();
    BulletController bullet3 = clone3.GetComponent<BulletController>();

    
    Vector2 baseDir = (savedPlayerPos + Vector2.up * 0.5f - pos).normalized;   // 1�� �Ѿ��� �÷��̾� ������

    // ������ ���� ���� 
    float angleOffset = 15f; // ���ϴ� ���� ����
    float rad = angleOffset * Mathf.Deg2Rad;

    Vector2 dirLeft = RotateVector(baseDir, rad);   // �������� ����
    Vector2 dirRight = RotateVector(baseDir, -rad); // ���������� ����


    bullet1.SetDirection(baseDir);   // ���
    bullet2.SetDirection(dirLeft);   // ����
    bullet3.SetDirection(dirRight);  // ������
    
    Destroy(clone1, 5f);
    Destroy(clone2, 5f);
    Destroy(clone3, 5f);
  }

  // ���� ��ȯ �޼���
  Vector2 RotateVector(Vector2 v, float radians)
  {
    float cos = Mathf.Cos(radians);
    float sin = Mathf.Sin(radians);
    return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
  }
}