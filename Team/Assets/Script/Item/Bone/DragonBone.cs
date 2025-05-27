using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBone : Itembase
{
  public GameObject Fus;
  bool isFus = false;             // �����üũ
  public float coolDown = 7.0f;  // ��ٿ�
  public float dmg;
  public string ItemInfo;
  public int howUpgrade;
  public PlayerController pc;
  public GameObject Player;

  private AudioSource audioSource;


    private void Start()
  {
    Player = GameObject.Find("Player");
    ItemInfo = "�߿� �ܿ��� �������� �����Դϴ�. ���� �����ϸ� ���ĳ��ϴ�.";
    pc = GameObject.Find("Player").GetComponent<PlayerController>();
    audioSource = GetComponent<AudioSource>();
  }

  private void Update()
  {
    howUpgrade = Level;

    if (!isFus)
    {
      push();
    }

    switch (howUpgrade)
    {
      case 0:
        coolDown = 7.0f * (1 - (0.1f * pc.attackSpeed));
        dmg = 1.5f * pc.attackDem;
        break;
      case 1:
        coolDown = 6.0f * (1 - (0.1f * pc.attackSpeed));
        dmg = 2.0f * pc.attackDem;
        break;
      case 2:
        coolDown = 5.0f * (1 - (0.1f * pc.attackSpeed));
        dmg = 3.0f * pc.attackDem;
        break;
      case 3:
        coolDown = 4.0f * (1 - (0.1f * pc.attackSpeed));
        dmg = 4.0f * pc.attackDem;
        break;
      case 4:
        coolDown = 3.0f * (1 - (0.1f * pc.attackSpeed));
        dmg = 5.5f * pc.attackDem;
        break;
      case 5:
        coolDown = 1.0f * (1 - (0.1f * pc.attackSpeed));
        dmg = 7.0f * pc.attackDem;
        break;
    }
  }

  // ���� ���������� �о�� ����� ������
  public void push()
  {
    Collider2D hit = Physics2D.OverlapCircle(Player.transform.position, 4.0f);

    if (hit.CompareTag("Enemy"))  // �±� Ȯ��
    {
      StartCoroutine(Ro());
    }
    if (hit.CompareTag("Boss"))
    {
      StartCoroutine(Ro());
    }
    if (hit.CompareTag("GameController"))
    {
      StartCoroutine(Ro());
    }

  }

  IEnumerator Ro()
  {
    isFus = true;

    //ȿ���� ���
    if(audioSource != null && SFXManager.instance != null && SFXManager.instance.bone != null)
    {
      audioSource.PlayOneShot(SFXManager.instance.bone);
    }

    Fus.SetActive(true);
    yield return new WaitForSeconds(2.0f);
    Fus.SetActive(false);
    yield return new WaitForSeconds(coolDown);
    isFus = false;
  }


}
