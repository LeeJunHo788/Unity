using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : EnemyController
{
  

  public bool IsDead => isDead; //Lock���� �б��������� ����

  public WaveController waveController; //UI

  protected override void Start()
  {
    base.Start(); //enemycontroller base ��������
    waveController = GameObject.Find("WaveUI").GetComponent<WaveController>();
  }

  protected override void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("PlayerAttack"))       // ���ݰ� �ε����� ���
    {
      //�ߺ� �ǰ� ������ ���� �߻�ü�� ���� �����Ѵ�
      Destroy(collision.gameObject);

      //���⼭���� �����̴� ü�±�� �����ڵ�
      BulletController bc = collision.GetComponent<BulletController>();   // ����� �������� ���� �Ҹ� ��Ʈ�ѷ� ��������
      float damage = pc.attackDem * bc.damageCoefficient;                 // �޴� ���ط� = �÷��̾� ���ݷ� * ������ ���

      HitEnemy(damage);
    }
  }

  public override void HitEnemy(float damage)
  {

    float finalDamage = Mathf.RoundToInt(damage - (damage * (defence - (defence * (pc.defIgnore * 0.01f))) * 0.01f));
    bool isCritical = Random.value < (pc.criticalChance / 100f);

    // ġ��Ÿ �� ���
    if (isCritical)
    {
      finalDamage *= pc.criticalDem;
    }


    // ������ �ؽ�Ʈ ����
    GameObject dmgText = Instantiate(damageTextPrefab, transform.position + Vector3.up, Quaternion.identity);
    dmgText.GetComponent<DamageTextController>().SetDamage(finalDamage, isCritical);   // ������, ġ��Ÿ ���� ����

    currentHp -= Mathf.RoundToInt(finalDamage);          //���� ����
    slider.value = currentHp; //�����̴�����

    if(currentHp < 0)
    {
      Dead();
    }

    //����FSM�� ���� ��ȯ ó��
    Stage1BossController stageBoss = this as Stage1BossController;
    if(stageBoss != null)
    {
      stageBoss.ApplyFSMOnHit();
    }
  }
  public override void Dead()
  {
    isDead = true; //�̵������
    currentHp = 0; //���̳ʽ��� �������°� ����
    rb.velocity = Vector2.zero; //�˹����
    rb.simulated = false; //���� ��Ȱ��ȭ
    animator.SetTrigger("Dead"); //Dead �ִϸ��̼� ���

    if (audioSource != null)
    {
      string prefabName = gameObject.name;
      if (prefabName.Contains("Stage3Boss") || prefabName.Contains("EliteBoss3"))
      {
        if (SFXManager.instance.boss3Kill != null)
        {
          audioSource.PlayOneShot(SFXManager.instance.boss3Kill);
        }
        else
        {
          Debug.LogWarning("[ȿ���� ����] boss3Kill Ŭ���� null");
        }
      }
      else if (prefabName.Contains("EliteBoss1_Cat"))
      {
        if (SFXManager.instance.catKill != null)
        {
          audioSource.PlayOneShot(SFXManager.instance.catKill);
        }
        else
        {
          Debug.LogWarning("[ȿ���� ����] catKill Ŭ���� null");
        }
      }
      else
      {
        if (SFXManager.instance.enemyKill != null)
        {
          audioSource.PlayOneShot(SFXManager.instance.enemyKill); // �⺻ �� ȿ����
        }
        else
        {
          Debug.LogWarning("[ȿ���� ����] enemyKill Ŭ���� null");
        }
      }
    }
    else
    {
      Debug.LogWarning("[ȿ���� ����] audioSource�� null");
    }

    OnDeath?.Invoke();
  }
  //�� ��ü �������� �̺�Ʈ�� �Լ�
  public void EnemyDieAnimationEvent()
  {
    DropExp();      // ����ġ ������Ʈ ���

    pc.killCount++;       // óġ Ƚ�� �߰�
    pc.KillCountUpdate(); // óġ Ƚ�� ǥ�� ������Ʈ

    Destroy(gameObject); //�� ����

    EnemyController.OnBossDeath?.Invoke();

  }
  // ����ġ ������Ʈ ���
  void DropExp()
  {
    Instantiate(expObj, transform.position, Quaternion.Euler(0, 0, 0));
  }

}
