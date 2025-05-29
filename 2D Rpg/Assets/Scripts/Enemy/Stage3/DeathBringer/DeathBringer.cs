using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeathBringer : BossController
{
  public int phase = 1;
  

  public PhysicsMaterial2D lowFrictionMaterial;  // 이동 가능할 때 사용할 물리 머티리얼
  public PhysicsMaterial2D highFrictionMaterial; // 멈출 때 사용할 물리 머티리얼

  // 공격 프리팹
  public GameObject Attack1;
  public GameObject Attack2;

  // 소환물 오브젝트
  public GameObject angel;
  public GameObject firewizard;
  public GameObject haloghost;

  GameObject[] enemySpawners; // 에너미 스포너 배열
  Vector2[] randomPositions = new Vector2[4]; // 랜덤하게 뽑은 4개의 위치

  // 몬스터 중복 소환 방지 변수
  bool hasPhase2Started = false;
  bool hasPhase3Started = false;




  protected override void Awake()
  {
    // 1페이즈 스탯
    range = 10.0f; // 탐색 범위
    attackRange = 4.0f; // 공격범위
    speed = 3.0f; // 이동속도
    att = 10;     // 공격력
    def = 10;     // 방어력
    hp = 1000;      // 체력
    exp = 20;     // 경험치
    gold = 30;     // 돈
    defIgn = 20;   // 방어력 무시
    returnDistance = 200f;
  }

  protected override void Start()
  {
    // 에너미 스포너 저장
    enemySpawners = FindObjectsOfType<GameObject>()
        .Where(obj => obj.name.Contains("EnemySpawner")) // 이름에 EnemySpawner가 포함된 오브젝트만 선택
        .ToArray(); // 배열로 변환


    // 랜덤 4개 선택
    enemySpawners = enemySpawners.OrderBy(obj => Random.value).ToArray();

    randomPositions[0] = enemySpawners[0].transform.position; // 1번 위치
    randomPositions[1] = enemySpawners[1].transform.position; // 2번 위치
    randomPositions[2] = enemySpawners[2].transform.position; // 3번 위치
    randomPositions[3] = enemySpawners[3].transform.position; // 4번 위치

    currentHp = hp;

    anim = gameObject.GetComponent<Animator>();

    base.Start();

  }

  protected override void Update()
  {

    // 체력에 따라 페이즈 변경
    if (!hasPhase2Started && currentHp <= hp * 0.7f && phase == 1)   // 2페이즈
    {
      hasPhase2Started = true; // 더 이상 실행 안 되도록 설정
      
      anim.SetTrigger("Upgrade");
      StartCoroutine(PhaseChange2());
    }


    if (!hasPhase3Started && currentHp <= hp * 0.3f && phase == 2)   // 2페이즈
    {
      hasPhase3Started = true;
      anim.SetTrigger("Upgrade");
      StartCoroutine(PhaseChange3());
    }


    if (isUpgrade)
      return;


    

    base.Update();
  }

  protected override IEnumerator PerformAttack()
  {
    isAttack = true;
    isAttacking = true;
    rb.velocity = Vector2.zero;
    box.sharedMaterial = highFrictionMaterial;

    int randomAttack = Random.Range(0, 12); // 0~11 랜덤

    if (randomAttack  < 5)
    {
      anim.SetTrigger("Attack1");
      Vector2 hitDir = (player.transform.position - transform.position);

      if (hitDir.x > 0)
      {
        Vector2 pos = transform.position + new Vector3(2.3f, 1.8f);

        StartCoroutine(Attack_1(pos));

      }

      else if (hitDir.x <= 0)
      {
        Vector2 pos = transform.position + new Vector3(-2.4f, 1.8f);
        StartCoroutine(Attack_1(pos));

      }

    }

    else if (randomAttack > 9)
    {
      StartCoroutine(SpawnEnemy3());
    }

    else
    {
      StartCoroutine(Attack_2(player.transform.position + Vector3.up * 1));
    }


    yield return new WaitForSeconds(1f); // 공격 후 쿨타임
    isAttacking = false;

    yield return new WaitForSeconds(0.5f); // 공격 후 쿨타임

    box.sharedMaterial = lowFrictionMaterial;
    isAttack = false;
    state = EnemyState.Idle;
  }


  // 1번 공격
  IEnumerator Attack_1(Vector2 pos)
  {

    yield return new WaitForSeconds(0.5f); // 0.3초 기다리기

    if (currentHp <= 0)
    {
      isAttack = false;
      isAttacking = false;
      anim.SetInteger("State", 4);
      yield break;
    }

    GameObject clone = Instantiate(Attack1, pos, Quaternion.identity);

    Vector2 hitDir = (player.transform.position - transform.position);
    if (hitDir.x > 0)
    {
      clone.transform.rotation = isDefaultLeft ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }

    else if (hitDir.x <= 0)
    {
      clone.transform.rotation = isDefaultLeft ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }

      Destroy(clone, 0.6f);
  }

  // 2번 공격
  IEnumerator Attack_2(Vector2 startPos)
  {
    anim.SetTrigger("Attack2");

    yield return new WaitForSeconds(0.7f);


    GameObject attackObj = Instantiate(Attack2, startPos, Quaternion.identity);

    float followDuration = 1.7f; // 따라다니는 시간
    float timer = 0f;

    // 플레이어를 따라다니는 동안
    while (timer < followDuration)
    {
      if (player != null)
      {
        // 플레이어 위치를 따라감
        attackObj.transform.position = player.transform.position + Vector3.up * 2.5f;
      }

      timer += Time.deltaTime;
      yield return null;
    }

    // 따라다니는 거 멈추고 그 자리에 고정
    Vector2 stopPos = attackObj.transform.position;
    attackObj.transform.position = stopPos;

    // 기다림 후 공격 판정
    yield return new WaitForSeconds(0.5f);

    // 사각형 범위 설정
    Vector2 boxSize = new Vector2(2.3f, 3.5f); // 범위 크기
    Collider2D hit = Physics2D.OverlapBox(stopPos, boxSize, 0f, LayerMask.GetMask("Player"));

    if (hit != null)
    {
      // 데미지 주는 코드 호출 (예: hit.GetComponent<PlayerController>().TakeDamage(att); )
      PlayerController pc = hit.GetComponent<PlayerController>();
      if (pc != null)
      {
        pc.TakeDem(45,30, player.transform.position - transform.position);  // 데미지
      }
    }


    // 1.2초 후 삭제
    yield return new WaitForSeconds(0.4f);
    Destroy(attackObj);
  }

  


  IEnumerator PhaseChange2()
  {
    isUpgrade = true;
    box.sharedMaterial = highFrictionMaterial;
    anim.SetTrigger("Upgrade");


    yield return new WaitForSeconds(4);

    yield return StartCoroutine(SpawnEnemy1());

    phase = 2;
    isUpgrade = false;
    box.sharedMaterial = lowFrictionMaterial;
  }

  IEnumerator SpawnEnemy1()
  {
    // 양 옆 위치
    Vector2 pos1 = transform.position + (Vector3.right * 3.5f);
    Vector2 pos2 = transform.position + (Vector3.left * 3.5f);

    // 대각선 위쪽
    Vector2 pos3 = transform.position + new Vector3(2, 5.5f);
    Vector2 pos4 = transform.position + new Vector3(-2, 5.5f);

    // 양옆에 마법사 소환
    GameObject clone1 = Instantiate(firewizard, pos1, Quaternion.identity);
    GameObject clone2 = Instantiate(firewizard, pos2, Quaternion.identity);
    clone1.name = firewizard.name;
    clone2.name = firewizard.name;

    yield return new WaitForSeconds(4);

    anim.SetTrigger("Upgrade");
    // 스포너에 천사 소환
    GameObject clone5 = Instantiate(angel, randomPositions[0], Quaternion.identity);
    GameObject clone6 = Instantiate(angel, randomPositions[1], Quaternion.identity);
    clone5.name = angel.name;
    clone6.name = angel.name;

    yield return new WaitForSeconds(3);

    // 대각선에 고스트 소환
    GameObject clone3 = Instantiate(haloghost, pos3, Quaternion.identity);
    GameObject clone4 = Instantiate(haloghost, pos4, Quaternion.identity);
    clone3.name = haloghost.name;
    clone4.name = haloghost.name;

    yield return new WaitForSeconds(3);
  }


  IEnumerator PhaseChange3()
  {
    isUpgrade = true;
    box.sharedMaterial = highFrictionMaterial;
    anim.SetTrigger("Upgrade");


    yield return new WaitForSeconds(4);
    yield return StartCoroutine(SpawnEnemy2());
    phase = 2;
    isUpgrade = false;
    box.sharedMaterial = lowFrictionMaterial;
  }

  IEnumerator SpawnEnemy2()
  {
    // 양 옆 위치
    Vector2 pos1 = transform.position + (Vector3.right * 3.5f);
    Vector2 pos2 = transform.position + (Vector3.left * 3.5f);


    // 양옆에 마법사 소환
    GameObject clone1 = Instantiate(firewizard, pos1, Quaternion.identity);
    GameObject clone2 = Instantiate(firewizard, pos2, Quaternion.identity);
    clone1.name = firewizard.name;
    clone2.name = firewizard.name;

    yield return new WaitForSeconds(2.5f);

    anim.SetTrigger("Upgrade");
    // 스포너에 천사 소환
    GameObject clone3 = Instantiate(angel, randomPositions[0], Quaternion.identity);
    GameObject clone4 = Instantiate(angel, randomPositions[1], Quaternion.identity);
    GameObject clone5 = Instantiate(angel, randomPositions[2], Quaternion.identity);
    GameObject clone6 = Instantiate(angel, randomPositions[3], Quaternion.identity);

    clone3.name = angel.name;
    clone4.name = angel.name;
    clone5.name = angel.name;
    clone6.name = angel.name;

    yield return new WaitForSeconds(2);
  }

  // 마법사 또는 유령 소환
  IEnumerator SpawnEnemy3()
  {

    int randomAttack = Random.Range(0, 2); // 0~1 랜덤

    yield return new WaitForSeconds(3);


    if (randomAttack == 0)
    {
      // 양 옆 위치
      Vector2 pos1 = transform.position + (Vector3.right * 2);
      Vector2 pos2 = transform.position + (Vector3.left * 2);

      // 양옆에 마법사 소환
      GameObject clone1 = Instantiate(firewizard, pos1, Quaternion.identity);
      GameObject clone2 = Instantiate(firewizard, pos2, Quaternion.identity);
      clone1.name = firewizard.name;
      clone2.name = firewizard.name;

    }

    else
    {
      // 대각선 위쪽
      Vector2 pos3 = transform.position + new Vector3(2, 4);
      Vector2 pos4 = transform.position + new Vector3(-2, 4);

      // 대각선에 고스트 소환
      GameObject clone3 = Instantiate(haloghost, pos3, Quaternion.identity);
      GameObject clone4 = Instantiate(haloghost, pos4, Quaternion.identity);
      clone3.name = haloghost.name;
      clone4.name = haloghost.name;
    }


    yield return new WaitForSeconds(2);

  }
}
