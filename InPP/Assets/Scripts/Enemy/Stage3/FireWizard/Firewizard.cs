using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firewizard : EnemyController
{
  public GameObject attack;


  protected override void Awake()
  {
    range = 35.0f; // Ž�� ����
    speed = 0f; // �̵��ӵ�
    att = 5;     // ���ݷ�
    def = 5;     // ����
    hp = 25;      // ü��
    exp = 20;     // ����ġ
    gold = 7;     // ��
    defIgn = 15;   // ���� ����
    spawnWeight = 5f;

    canAttack = true;
    attackRange = 15.0f;
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
      transform.rotation = Quaternion.Euler(0, 180, 0);
      Vector2 pos = transform.position + new Vector3(0.2f, -0.2f);
      // �����̴��� ȸ��ȿ�� ����
      hpSlider.transform.localRotation = Quaternion.Euler(0, 0, 0);
      StartCoroutine(Attack(pos));

    }

    else if (hitDir.x <= 0)
    {
      transform.rotation = Quaternion.Euler(0, 180, 0);
      Vector2 pos = transform.position + new Vector3(-0.2f, -0.2f);
      // �����̴��� ȸ��ȿ�� ����
      hpSlider.transform.localRotation = Quaternion.Euler(0, 0, 0);

      StartCoroutine(Attack(pos));

    }

    state = EnemyState.Idle;

    yield return new WaitForSeconds(attackTime); // ���� �� ��Ÿ��
    isAttacking = false;


    yield return new WaitForSeconds(2f); // ���� �� ��Ÿ��

    isAttack = false;
  }

  IEnumerator Attack(Vector2 pos)
  {

    yield return new WaitForSeconds(0.9f); // 0.8�� ��ٸ���

    if (currentHp <= 0)
    {
      isAttack = false;
      isAttacking = false;
      anim.SetInteger("State", 4);
      yield break;
    }

    GameObject clone = Instantiate(attack, pos, Quaternion.identity);


    Destroy(clone, 10f);
  }
}
