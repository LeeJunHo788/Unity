using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����ü Ŭ�� Axe�� �����ų ��ũ��Ʈ
public class Axe : Itembase
{
  // ����ü ������
  public GameObject Threw;    // ����ü������
  public float attackRate = 2.0f;    // ���ݺ�
  bool isAttack = false;      // ����üũ
  public float axeDmg;
  public int howUpgrade = 0;  // ��ȭ����
  public string ItemInfo;
  public PlayerController player;

  // �÷��̾� ��ġ
  Vector2 playerpos;

  //ȿ����
  private AudioSource audioSource;

  private void Start()
  {
    // �÷��̾� ��ġ �޾ƿ���
    playerpos = GameObject.Find("Player").transform.position;
    player = GameObject.Find("Player").GetComponent<PlayerController>();
    audioSource = GetComponent<AudioSource>();
    ItemInfo = "ȸ���ϴ� ������ �Ӹ� ���� �����ϴ�.";
  }

  private void Update()
  {
    // �÷��̾� ��ġ ������Ʈ
    playerpos = GameObject.Find("Player").transform.position;

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
        axeDmg = 0.25f;
        attackRate = 2.0f * (1 - (0.1f * player.attackSpeed));
        break;
      case 1:
        axeDmg = 0.5f;
        attackRate = 1.8f * (1 - (0.1f * player.attackSpeed));
        break;
      case 2:
        axeDmg = 0.75f;
        attackRate = 1.6f * (1 - (0.1f * player.attackSpeed));
        break;
      case 3:
        axeDmg = 1.0f;
        attackRate = 1.4f * (1 - (0.1f * player.attackSpeed));
        break;
      case 4:
        axeDmg = 1.25f;
        attackRate = 1.2f * (1 - (0.1f * player.attackSpeed));
        break;
      case 5:
        axeDmg = 2.0f;
        attackRate = 0.5f * (1 - (0.1f * player.attackSpeed));
        break;
    }
  }


  IEnumerator Attack()
  {
    // �÷��̾� ��ġ�� ����ü�� ����
    isAttack = true;
    if (audioSource != null && SFXManager.instance != null && SFXManager.instance.axeThrew != null)
    {
      audioSource.PlayOneShot(SFXManager.instance.axeThrew);
    }
    for (int i = 0; i <= howUpgrade; i++)
    {
      GameObject clone = Instantiate(Threw, playerpos, Threw.transform.rotation);
      clone.GetComponent<Rigidbody2D>().velocity = new Vector2(2 - i, 8) * 1.5f;
    }
    yield return new WaitForSeconds(attackRate);
    isAttack = false;
  }
}
