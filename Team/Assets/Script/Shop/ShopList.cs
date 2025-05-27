using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopList : MonoBehaviour
{
  GameObject ShopUI;
  PlayerController Player;

  //ȿ���� ����
  private AudioSource audioSource;

  [Header("�������ݱ��Ű��� ����")]
  int maxHpUp;     // ü�°�ȭ����
  int AtkUp;       // ���ݷ°�ȭ����
  int DefUp;       // ���°�ȭ����
  int getUp;       // ȹ���Ÿ���ȭ����
  int goldUp;      // ���ȹ�氭ȭ����
  int expUp;       // ����ġȹ�氭ȭ����

  public int gold; // �ӽ�


  public Text chatText;   // ����â �ϴ� ����
  public Text text01;     // ��ǰ 1 ���� �� ����
  public Text text02;     // ��ǰ 2 ���� �� ����
  public Text text03;     // ��ǰ 3 ���� �� ����
  public Text text04;     // ��ǰ 4 ���� �� ����
  public Text text05;     // ��ǰ 5 ���� �� ����
  public Text text06;     // ��ǰ 6 ���� �� ����
  public Text playermoney;


  public int price01;
  public int price02;
  public int price03;
  public int price04;
  public int price05;
  public int price06;

  private void Start()
  {
    Player = GameObject.Find("Player").GetComponent<PlayerController>();
    ShopUI = GameObject.Find("ShopUI");
    audioSource = GetComponent<AudioSource>(); //������ҽ� �޾ƿ���
  }

  private void Update()
  {
    text01.text = "ü�� ��ȭ\n���� : " + price01;     // ���� ������ ǥ��
    text02.text = "���ݷ� ��ȭ\n���� : " + price02;     // ���� ������ ǥ��
    text03.text = "���� ��ȭ\n���� : " + price03;     // ���� ������ ǥ��
    text04.text = "������ȹ��\n��ȭ ���� : " + price04;     // ���� ������ ǥ��
    text05.text = "���ȹ��\n��ȭ ���� : " + price05;     // ���� ������ ǥ��
    text06.text = "����ġȹ��\n��ȭ ���� : " + price06;     // ���� ������ ǥ��
    playermoney.text = "" + Player.gold + " Gold";                    // �÷��̾� ������
    maxHpUp = PlayerPrefs.GetInt("maxHpUp");
    AtkUp = PlayerPrefs.GetInt("AtkUp");
    DefUp = PlayerPrefs.GetInt("DefUp");
    getUp = PlayerPrefs.GetInt("getUp");
    goldUp = PlayerPrefs.GetInt("goldUp");
    expUp = PlayerPrefs.GetInt("expUp");
    // ���ݼ����� ����ġ��
    switch (maxHpUp)
    {
      case 0:
        price01 = 500;  // �ӽ� �ʱⰡ�� 500
        break;
      case 1:
        price01 = 1000;
        break;
      case 2:
        price01 = 1500;
        break;
      case 3:
        price01 = 2000;
        break;
      case 4:
        price01 = 2500;
        break;
      default:
        price01 = 3000;
        break;

    }
    switch (AtkUp)
    {
      case 0:
        price02 = 500;  // �ӽ� �ʱⰡ�� 500
        break;
      case 1:
        price02 = 1000;
        break;
      case 2:
        price02 = 1500;
        break;
      case 3:
        price02 = 2000;
        break;
      case 4:
        price02 = 2500;
        break;
      default:
        price02 = 3000;
        break;
    }
    switch (DefUp)
    {
      case 0:
        price03 = 500;  // �ӽ� �ʱⰡ�� 500
        break;
      case 1:
        price03 = 1000;
        break;
      case 2:
        price03 = 1500;
        break;
      case 3:
        price03 = 2000;
        break;
      case 4:
        price03 = 2500;
        break;
      default:
        price03 = 3000;
        break;
    }
    switch (getUp)
    {
      case 0:
        price04 = 500;  // �ӽ� �ʱⰡ�� 500
        break;
      case 1:
        price04 = 1000;
        break;
      case 2:
        price04 = 1500;
        break;
      case 3:
        price04 = 2000;
        break;
      case 4:
        price04 = 2500;
        break;
      default:
        price04 = 3000;
        break;
    }
    switch (goldUp)
    {
      case 0:
        price05 = 500;  // �ӽ� �ʱⰡ�� 500
        break;
      case 1:
        price05 = 1000;
        break;
      case 2:
        price05 = 1500;
        break;
      case 3:
        price05 = 2000;
        break;
      case 4:
        price05 = 2500;
        break;
      default:
        price05 = 3000;
        break;
    }
    switch (expUp)
    {
      case 0:
        price06 = 500;  // �ӽ� �ʱⰡ�� 500
        break;
      case 1:
        price06 = 1000;
        break;
      case 2:
        price06 = 1500;
        break;
      case 3:
        price06 = 2000;
        break;
      case 4:
        price06 = 2500;
        break;
      default:
        price06 = 3000;
        break;
    }

    if (Input.GetKeyDown(KeyCode.Escape))
    {
      ExitShop();
    }
  }

  // ü�� ���       
  public void HpSell()
  {
    if (Player.gold >= price01)
    {
      // ���� ��ȭ�� ����
      maxHpUp++;
      PlayerPrefs.SetInt("maxHpUp", maxHpUp);
      // �ӽ÷� 50���
      //Player.maxHp += 50;
      Player.gold -= price01;
      PlayerPrefs.SetFloat("maxHp", 10f * (float)maxHpUp);    // ������ ����

      PlayBuySound(); //ȿ�������
      chatText.text = "ü�°�ȭ�� �����߽��ϴ�.\n���� ��ȭ�� :" + maxHpUp;
    }
    else
    {
      chatText.text = "��尡 �����մϴ�";
    }
  }

  // ���ݷ� ���
  public void AtkSell()
  {
    if (Player.gold >= price02)
    {
      // ���������� �߰�

      // ���� ��ȭ�� ����
      AtkUp++;
      PlayerPrefs.SetInt("AtkUp", AtkUp);
      // �ӽð�ȭ
      //Player.attackDem += 10;
      Player.gold -= price02;
      PlayerPrefs.SetFloat("attackDem", 10f * (float)AtkUp);    // ������ ����

      PlayBuySound(); //ȿ�������
      chatText.text = "���ݷ°�ȭ�� �����߽��ϴ�.\n���� ��ȭ�� :" + AtkUp;
    }
    else
    {
      chatText.text = "��尡 �����մϴ�";
    }
  }

  // ���� ���
  public void DefSell()
  {

    if (Player.gold >= price03)
    {
      // ���������� �߰�

      // ���� ��ȭ�� ����
      DefUp++;
      PlayerPrefs.SetInt("DefUp", DefUp);
      //Player.defence += 5;
      Player.gold -= price03;
      PlayerPrefs.SetFloat("defence", 5 * (float)DefUp);    // ������ ����

      PlayBuySound(); //ȿ�������
      chatText.text = "���°�ȭ�� �����߽��ϴ�.\n���� ��ȭ�� :" + DefUp;
    }
    else
    {
      chatText.text = "��尡 �����մϴ�";
    }
  }

  public void GetSell()
  {
    if (Player.gold >= price04)
    {
      // ���� ��ȭ�� ����
      getUp++;
      PlayerPrefs.SetInt("getUp", getUp);
      //Player.getRange += 1f;
      Player.gold -= price04;
      PlayerPrefs.SetFloat("getRange", 1f * (float)getUp);    // ������ ����

      PlayBuySound(); //ȿ�������
      chatText.text = "������ȹ�氭ȭ�� �����߽��ϴ�.\n���� ��ȭ�� :" + getUp;
    }
    else
    {
      chatText.text = "��尡 �����մϴ�";
    }
  }
  public void GoldSell()
  {
    // �ֿ佺��04 ���

    if (Player.gold >= price05)
    {
      // ���������� �߰�

      // ���� ��ȭ�� ����
      goldUp++;
      PlayerPrefs.SetInt("goldUp", goldUp);
      //Player.goldAcq++;
      Player.gold -= price05;
      PlayerPrefs.SetFloat("goldAcq", 1f * (float)goldUp);    // ������ ����

      PlayBuySound(); //ȿ�������
      chatText.text = "���ȹ�淮��ȭ�� �����߽��ϴ�.\n���� ��ȭ�� :" + goldUp;
    }
    else
    {
      chatText.text = "��尡 �����մϴ�";
    }
  }
  public void ExpSell()
  {
    // �ֿ佺��04 ���

    if (Player.gold >= price06)
    {
      // ���������� �߰�

      // ���� ��ȭ�� ����
      expUp++;
      PlayerPrefs.SetInt("expUp", expUp);
      //Player.expAcq++;
      Player.gold -= price06;
      PlayerPrefs.SetFloat("expAcq", 1f * (float)expUp);    // ������ ����

      PlayBuySound(); //ȿ�������
      chatText.text = "����ġ��ȭ�� �����߽��ϴ�.\n���� ��ȭ�� :" + expUp;
    }
    else
    {
      chatText.text = "��尡 �����մϴ�";
    }
  }



  public void ExitShop()
  {
    ShopUI.SetActive(false);
  }

  private void PlayBuySound()
  {
    if (audioSource != null && SFXManager.instance != null && SFXManager.instance.shopBuy != null)
    {
      audioSource.PlayOneShot(SFXManager.instance.shopBuy);
    }
  }
}
