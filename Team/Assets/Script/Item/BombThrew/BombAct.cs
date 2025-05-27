using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAct : MonoBehaviour
{
  public EnemyController Enemy;
  public PlayerController Pc;
  public BossController Bc;
  public CircleCollider2D circleCollider;
  public BombThrew Bomb;
  public float Dmg;
  public string BombName;
  public float arcHeight = 2f; // 포물선 높이

  public GameObject miniBomb;

  private AudioSource audioSource;

  // Start is called before the first frame update
  void Start()
  {
    Pc = GameObject.Find("Player").GetComponent<PlayerController>();
    Bomb = GameObject.Find("BombThrew").GetComponent<BombThrew>();
    circleCollider = GetComponent<CircleCollider2D>();
    audioSource = GetComponent<AudioSource>();
    StartCoroutine(des());
  }

  // Update is called once per frame
  void Update()
  {
    // 강화수치에 따른 공격력 배율증가
    // 폭탄 크기에 따라 차분
    if (BombName == "S")
    {
      switch (Bomb.howUpgrade)
      {
        case 0:
          Dmg = Pc.attackDem * 0.5f;
          break;
        case 1:
          Dmg = Pc.attackDem * 0.75f;
          break;
        case 2:
          Dmg = Pc.attackDem * 1.0f;
          break;
        case 3:
          Dmg = Pc.attackDem * 1.25f;
          break;
        case 4:
          Dmg = Pc.attackDem * 1.5f;
          break;
        case 5:
          Dmg = Pc.attackDem * 2f;
          break;
      }
    }
    else if (BombName == "M")
    {
      switch (Bomb.howUpgrade)
      {
        case 0:
          Dmg = Pc.attackDem * 1.0f;
          break;
        case 1:
          Dmg = Pc.attackDem * 2f;
          break;
        case 2:
          Dmg = Pc.attackDem * 3f;
          break;
        case 3:
          Dmg = Pc.attackDem * 4f;
          break;
        case 4:
          Dmg = Pc.attackDem * 7f;
          break;
        case 5:
          Dmg = Pc.attackDem * 10f;
          break;
      }
    }
    else if (BombName == "L")
    {
      switch (Bomb.howUpgrade)
      {
        case 0:
          Dmg = Pc.attackDem * 2f;
          break;
        case 1:
          Dmg = Pc.attackDem * 4f;
          break;
        case 2:
          Dmg = Pc.attackDem * 6f;
          break;
        case 3:
          Dmg = Pc.attackDem * 8f;
          break;
        case 4:
          Dmg = Pc.attackDem * 15f;
          break;
        case 5:
          Dmg = Pc.attackDem * 20f;
          break;
      }
    }
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.collider.CompareTag("Enemy"))
    {
      collision.collider.transform.gameObject.GetComponent<EnemyController>().HitEnemy(Dmg);
    }
    if (collision.collider.CompareTag("Boss"))
    {
      collision.collider.transform.gameObject.GetComponent<EnemyController>().HitEnemy(Dmg);
    }
  }

  IEnumerator des()
  {
    yield return new WaitForSeconds(1.0f);

    //폭발 사운드 재생
    if(audioSource != null && SFXManager.instance != null && SFXManager.instance.bombThrew != null)
    {
      audioSource.PlayOneShot(SFXManager.instance.bombThrew);
    }

    // 대형폭발은 작은 폭탄을 강화수치만큼 생성한다
    if (BombName == "L")
    {
      Vector2 center = transform.position;

      for (int i = 0; i < 5; i++)
      {
        // 랜덤한 원 안의 위치
        Vector2 randomPoint = center + Random.insideUnitCircle * circleCollider.radius;

        // A 오브젝트 생성 후, 랜덤 위치로 이동
        GameObject aObj = Instantiate(miniBomb, center, Quaternion.identity);
        aObj.transform.position = randomPoint; // 즉시 위치 이동
      }
    }
    Destroy(gameObject);
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Enemy"))
    {
      collision.transform.gameObject.GetComponent<EnemyController>().HitEnemy(Dmg);
    }
    if (collision.CompareTag("Boss"))
    {
      collision.transform.gameObject.GetComponent<EnemyController>().HitEnemy(Dmg);
    }
  }
}
