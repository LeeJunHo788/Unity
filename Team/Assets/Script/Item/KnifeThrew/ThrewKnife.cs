using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrewKnife : Itembase
{
  public GameObject knife;
  PlayerController pc;
  GameObject Player;
  public float attackRate = 2.0f;    // ���ݺ�
  bool isAttack = false;      // ����üũ
  public float knifeDmg;
  public int howUpgrade = 0;  // ��ȭ����
  public string ItemInfo;

  //ȿ����
  private AudioSource audioSource;

  private void Start()
  {
    Player = GameObject.Find("Player");
    pc = GameObject.Find("Player").GetComponent<PlayerController>();
    audioSource = GetComponent<AudioSource>();
    ItemInfo = "�ܰ��� ��ġ�ϰ� ��������ϴ�.";
    howUpgrade = 0;
    knifeDmg = 1.0f;
  }

  private void Update()
  {
    howUpgrade = Level;

    if (!isAttack)
    {
      StartCoroutine(Attack());
    }
    else
    {
      return;
    }
    // ��ȭ������ ���� ���ݼӵ� �� ���ݷ�
    switch (howUpgrade)
    {
      case 0:
        knifeDmg = 1.0f;
        attackRate = 2.0f * (1 - (0.1f * pc.attackSpeed));
        break;
      case 1:
        knifeDmg = 1.25f;
        attackRate = 1.8f * (1 - (0.1f * pc.attackSpeed));
        break;
      case 2:
        knifeDmg = 1.5f;
        attackRate = 1.6f * (1 - (0.1f * pc.attackSpeed));
        break;
      case 3:
        knifeDmg = 1.75f;
        attackRate = 1.4f * (1 - (0.1f * pc.attackSpeed));
        break;
      case 4:
        knifeDmg = 2.0f;
        attackRate = 1.2f * (1 - (0.1f * pc.attackSpeed));
        break;
      case 5:
        knifeDmg = 3.0f;
        attackRate = 1.0f * (1 - (0.1f * pc.attackSpeed));
        break;
    }

  }

  IEnumerator Attack()
  {
    // �÷��̾� ��ġ�� ����ü�� ����
    isAttack = true;

    //ȿ���� ���
    if(audioSource != null && SFXManager.instance != null && SFXManager.instance.knifeThrew != null)
    {
      audioSource.PlayOneShot(SFXManager.instance.knifeThrew);
    }

    for (int i = 0; i <= howUpgrade; i++)
    {
      GameObject clone = Instantiate(knife, new Vector3(Player.transform.position.x, Player.transform.position.y - (i * 0.2f), Player.transform.position.z), knife.transform.rotation);
    }
    yield return new WaitForSeconds(attackRate);
    isAttack = false;
  }
}
