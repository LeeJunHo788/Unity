using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 투사체 클론 Axe에 적용시킬 스크립트
public class Axe : Itembase
{
  // 투사체 프리팹
  public GameObject Threw;    // 투사체프리팹
  public float attackRate = 2.0f;    // 공격빈도
  bool isAttack = false;      // 공격체크
  public float axeDmg;
  public int howUpgrade = 0;  // 강화정도
  public string ItemInfo;
  public PlayerController player;

  // 플레이어 위치
  Vector2 playerpos;

  //효과음
  private AudioSource audioSource;

  private void Start()
  {
    // 플레이어 위치 받아오기
    playerpos = GameObject.Find("Player").transform.position;
    player = GameObject.Find("Player").GetComponent<PlayerController>();
    audioSource = GetComponent<AudioSource>();
    ItemInfo = "회전하는 도끼를 머리 위로 던집니다.";
  }

  private void Update()
  {
    // 플레이어 위치 업데이트
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
    // 강화정도에 따른 공격속도 및 공격력
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
    // 플레이어 위치에 투사체를 생성
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
