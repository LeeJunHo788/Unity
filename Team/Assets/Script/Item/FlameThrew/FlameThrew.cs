using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrew : Itembase
{
  public GameObject flame;
  GameObject Player;
  PlayerController pc;
  public int howUpgrade;
  public float attackTime = 2.0f;
  public string ItemInfo;
  bool isAttack = false;

  private AudioSource audioSource;

  private void Start()
  {
    Player = GameObject.Find("Player");
    pc = GameObject.Find("Player").GetComponent<PlayerController>();
    audioSource = GetComponent<AudioSource>();
    ItemInfo = "적을 감지하면 불태웁니다.";
  }

  private void Update()
  {
    howUpgrade = Level;

    switch (howUpgrade)
    {
      case 0:
        attackTime = 2.0f * (1 - (0.1f * pc.attackSpeed));
        break;
      case 1:
        attackTime = 1.8f * (1 - (0.1f * pc.attackSpeed));
        break;
      case 2:
        attackTime = 1.6f * (1 - (0.1f * pc.attackSpeed));
        break;
      case 3:
        attackTime = 1.4f * (1 - (0.1f * pc.attackSpeed));
        break;
      case 4:
        attackTime = 1.2f * (1 - (0.1f * pc.attackSpeed));
        break;
      case 5:
        attackTime = 1.0f * (1 - (0.1f * pc.attackSpeed));
        break;
    }
    if (!isAttack)
    {
      tracking();
    }
  }

  public void tracking()
  {
    StartCoroutine(Attack());
  }

  IEnumerator Attack()
  {
    isAttack = true;

    //효과음재생
    if (audioSource != null && SFXManager.instance != null && SFXManager.instance.flame != null)
    {
      audioSource.PlayOneShot(SFXManager.instance.flame);
    }

    Collider2D[] hits = Physics2D.OverlapCircleAll(Player.transform.position, 5f);

    foreach (Collider2D hit in hits)
    {
      if (hit.CompareTag("Enemy"))
      {
        GameObject flameOn = Instantiate(flame, hit.transform.position, Quaternion.Euler(0, 0, 0));
      }

      if (hit.CompareTag("Boss"))
      {
        GameObject flameOn = Instantiate(flame, hit.transform.position, Quaternion.Euler(0, 0, 0));
      }
    }

    yield return new WaitForSeconds(attackTime);
    isAttack = false;
  }
}
