using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrewKnife : Itembase
{
  public GameObject knife;
  PlayerController pc;
  GameObject Player;
  public float attackRate = 2.0f;    // 공격빈도
  bool isAttack = false;      // 공격체크
  public float knifeDmg;
  public int howUpgrade = 0;  // 강화정도
  public string ItemInfo;

  //효과음
  private AudioSource audioSource;

  private void Start()
  {
    Player = GameObject.Find("Player");
    pc = GameObject.Find("Player").GetComponent<PlayerController>();
    audioSource = GetComponent<AudioSource>();
    ItemInfo = "단검을 배치하고 집어던집니다.";
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
    // 강화정도에 따른 공격속도 및 공격력
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
    // 플레이어 위치에 투사체를 생성
    isAttack = true;

    //효과음 재생
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
