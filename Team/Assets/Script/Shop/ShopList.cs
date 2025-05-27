using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopList : MonoBehaviour
{
  GameObject ShopUI;
  PlayerController Player;

  //효과음 변수
  private AudioSource audioSource;

  [Header("상점스텟구매관련 스택")]
  int maxHpUp;     // 체력강화스택
  int AtkUp;       // 공격력강화스택
  int DefUp;       // 방어력강화스택
  int getUp;       // 획득사거리강화스택
  int goldUp;      // 골드획득강화스택
  int expUp;       // 경험치획득강화스택

  public int gold; // 임시


  public Text chatText;   // 상점창 하단 설명문
  public Text text01;     // 상품 1 가격 및 설명
  public Text text02;     // 상품 2 가격 및 설명
  public Text text03;     // 상품 3 가격 및 설명
  public Text text04;     // 상품 4 가격 및 설명
  public Text text05;     // 상품 5 가격 및 설명
  public Text text06;     // 상품 6 가격 및 설명
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
    audioSource = GetComponent<AudioSource>(); //오디오소스 받아오기
  }

  private void Update()
  {
    text01.text = "체력 강화\n가격 : " + price01;     // 현재 상점가 표기
    text02.text = "공격력 강화\n가격 : " + price02;     // 현재 상점가 표기
    text03.text = "방어력 강화\n가격 : " + price03;     // 현재 상점가 표기
    text04.text = "아이템획득\n강화 가격 : " + price04;     // 현재 상점가 표기
    text05.text = "골드획득\n강화 가격 : " + price05;     // 현재 상점가 표기
    text06.text = "경험치획득\n강화 가격 : " + price06;     // 현재 상점가 표기
    playermoney.text = "" + Player.gold + " Gold";                    // 플레이어 소지금
    maxHpUp = PlayerPrefs.GetInt("maxHpUp");
    AtkUp = PlayerPrefs.GetInt("AtkUp");
    DefUp = PlayerPrefs.GetInt("DefUp");
    getUp = PlayerPrefs.GetInt("getUp");
    goldUp = PlayerPrefs.GetInt("goldUp");
    expUp = PlayerPrefs.GetInt("expUp");
    // 가격설정용 스위치문
    switch (maxHpUp)
    {
      case 0:
        price01 = 500;  // 임시 초기가격 500
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
        price02 = 500;  // 임시 초기가격 500
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
        price03 = 500;  // 임시 초기가격 500
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
        price04 = 500;  // 임시 초기가격 500
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
        price05 = 500;  // 임시 초기가격 500
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
        price06 = 500;  // 임시 초기가격 500
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

  // 체력 상승       
  public void HpSell()
  {
    if (Player.gold >= price01)
    {
      // 스텟 강화도 증가
      maxHpUp++;
      PlayerPrefs.SetInt("maxHpUp", maxHpUp);
      // 임시로 50상승
      //Player.maxHp += 50;
      Player.gold -= price01;
      PlayerPrefs.SetFloat("maxHp", 10f * (float)maxHpUp);    // 데이터 저장

      PlayBuySound(); //효과음재생
      chatText.text = "체력강화를 구매했습니다.\n현재 강화도 :" + maxHpUp;
    }
    else
    {
      chatText.text = "골드가 부족합니다";
    }
  }

  // 공격력 상승
  public void AtkSell()
  {
    if (Player.gold >= price02)
    {
      // 스텟증가문 추가

      // 스텟 강화도 증가
      AtkUp++;
      PlayerPrefs.SetInt("AtkUp", AtkUp);
      // 임시강화
      //Player.attackDem += 10;
      Player.gold -= price02;
      PlayerPrefs.SetFloat("attackDem", 10f * (float)AtkUp);    // 데이터 저장

      PlayBuySound(); //효과음재생
      chatText.text = "공격력강화를 구매했습니다.\n현재 강화도 :" + AtkUp;
    }
    else
    {
      chatText.text = "골드가 부족합니다";
    }
  }

  // 방어력 상승
  public void DefSell()
  {

    if (Player.gold >= price03)
    {
      // 스텟증가문 추가

      // 스텟 강화도 증가
      DefUp++;
      PlayerPrefs.SetInt("DefUp", DefUp);
      //Player.defence += 5;
      Player.gold -= price03;
      PlayerPrefs.SetFloat("defence", 5 * (float)DefUp);    // 데이터 저장

      PlayBuySound(); //효과음재생
      chatText.text = "방어력강화를 구매했습니다.\n현재 강화도 :" + DefUp;
    }
    else
    {
      chatText.text = "골드가 부족합니다";
    }
  }

  public void GetSell()
  {
    if (Player.gold >= price04)
    {
      // 스텟 강화도 증가
      getUp++;
      PlayerPrefs.SetInt("getUp", getUp);
      //Player.getRange += 1f;
      Player.gold -= price04;
      PlayerPrefs.SetFloat("getRange", 1f * (float)getUp);    // 데이터 저장

      PlayBuySound(); //효과음재생
      chatText.text = "아이템획득강화를 구매했습니다.\n현재 강화도 :" + getUp;
    }
    else
    {
      chatText.text = "골드가 부족합니다";
    }
  }
  public void GoldSell()
  {
    // 주요스텟04 상승

    if (Player.gold >= price05)
    {
      // 스텟증가문 추가

      // 스텟 강화도 증가
      goldUp++;
      PlayerPrefs.SetInt("goldUp", goldUp);
      //Player.goldAcq++;
      Player.gold -= price05;
      PlayerPrefs.SetFloat("goldAcq", 1f * (float)goldUp);    // 데이터 저장

      PlayBuySound(); //효과음재생
      chatText.text = "골드획득량강화를 구매했습니다.\n현재 강화도 :" + goldUp;
    }
    else
    {
      chatText.text = "골드가 부족합니다";
    }
  }
  public void ExpSell()
  {
    // 주요스텟04 상승

    if (Player.gold >= price06)
    {
      // 스텟증가문 추가

      // 스텟 강화도 증가
      expUp++;
      PlayerPrefs.SetInt("expUp", expUp);
      //Player.expAcq++;
      Player.gold -= price06;
      PlayerPrefs.SetFloat("expAcq", 1f * (float)expUp);    // 데이터 저장

      PlayBuySound(); //효과음재생
      chatText.text = "경험치강화를 구매했습니다.\n현재 강화도 :" + expUp;
    }
    else
    {
      chatText.text = "골드가 부족합니다";
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
