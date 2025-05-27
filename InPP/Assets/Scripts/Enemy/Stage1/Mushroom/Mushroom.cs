using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Mushroom : EnemyController
{
  
  public GameObject attack;


  protected override void Awake()
  {
    range = 70f; // Ž�� ����
    speed = 1.8f; // �̵��ӵ�
    att = 5;     // ���ݷ�
    def = 5;     // ����
    hp = 10;      // ü��
    exp = 15;     // ����ġ
    gold = 5;     // ��
    defIgn = 15;   // ���� ����

    canAttack = true;
    attackRange = 5.0f;
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
      hpSlider.transform.localRotation = Quaternion.Euler(0, 180, 0);

      StartCoroutine(Attack(pos));

    }
    

    yield return new WaitForSeconds(attackTime); // ���� �� ��Ÿ��
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

    yield return new WaitForSeconds(0.8f); // 0.8�� ��ٸ���

    if (currentHp <= 0)
    {
      isAttack = false;
      isAttacking = false;
      anim.SetInteger("State", 4);
      yield break;
    }

    GameObject clone = Instantiate(attack, pos, Quaternion.identity);


    Destroy(clone, 3f);
  }
}
