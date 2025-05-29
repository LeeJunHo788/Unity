using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  public GameObject[] Wave1Enemy;  // 1웨이브
  public GameObject[] Wave2Enemy;  // 2웨이브
  public GameObject[] Wave3Enemy;  // 3웨이브


  GameObject[] enemy;   // 현재 웨이브 스폰될 적
  GameObject[][] enemyPool;


  bool isSpawning;  // 소환중 여부
  public float minTime;    // 스폰 최소 쿨타임
  public float maxTime;    // 스폰 최대 쿨타임

  Collider2D[] cols; // 감지한 콜라이더를 담는 배열
  public float range = 30;   // 플레이어 탐지 범위
  public float stopSpawnDistance = 5f;  // 스폰 중지 범위
  bool isDetect = false;      // 범위 내 플레이어 감지 여부
  float distanceToPlayer;

  GameObject tp;    // 텔레포터 오브젝트
  TeleporterManager tpManager; // 텔레포터매니저 

  int waveNum;   // 웨이브 번호

  void Awake()
  {
    
  }

  private void Start()
  {

    waveNum = 1;

    // 배열 초기화
    enemyPool = new GameObject[][] { Wave1Enemy, Wave2Enemy, Wave3Enemy };

    // 웨이브에 맞는 적 스폰 배열 설정
    enemy = enemyPool[waveNum -1];

    isDetect = false;
    isSpawning = false;

    minTime = 7;
    maxTime = 15;


    tp = GameObject.Find("Teleporter");
    tpManager = tp.GetComponent<TeleporterManager>();

  }

  private void Update()
  {
    // 중심으로 일정 범위 내에 있는 콜라이더
    cols = Physics2D.OverlapCircleAll(transform.position, range);

    // 감지된 콜라이더 검사 실행
    foreach (Collider2D col in cols)
    {
      // 콜라이더의 태그가 Player라면
      if (col != null && col.CompareTag("Player"))
      {
        distanceToPlayer = Vector2.Distance(transform.position, col.transform.position);

        // 너무 가까우면 감지해도 스폰하지 않도록 표시
        if (distanceToPlayer >= stopSpawnDistance)
        {
          isDetect = true;
        }
        break;
      }
    }

    // 플레이어가 범위 내에 있으면 스포너 활성화
    if (isDetect)
    {
      StartCoroutine(SpawnEnemy());
    }

  }

  IEnumerator SpawnEnemy()
  {

    // 스포너 주위에 플레이어가 있을 때만 스포너 활성화
    while (isDetect && !isSpawning)
    {

      // 준비 중이거나 보스가 소환되었으면 스폰 중단
      if (tpManager.isPreparing || tpManager.isBossSpawn || distanceToPlayer <= stopSpawnDistance)
      {
        yield break;
      }

      isSpawning = true;

      // 랜덤시간 대기 후
      float spawnTime = Random.Range(minTime, maxTime);
      yield return new WaitForSeconds(spawnTime);


      // 텔레포터가 준비중이라면 소환 불가능
      if (tpManager.isPreparing == true || distanceToPlayer <= stopSpawnDistance)
      {
        isSpawning = false;
        yield break;
      }

      // 시간 지난 후에도 다시 체크
      if (tpManager.isPreparing || tpManager.isBossSpawn || distanceToPlayer <= stopSpawnDistance)
      {
        isSpawning = false;
        yield break;
      }


      // 몬스터 소환
      GameObject selectedEnemy = GetRandomEnemy(enemy);
      GameObject spawnEnemy = Instantiate(selectedEnemy, transform.position, Quaternion.identity);
      spawnEnemy.name = selectedEnemy.name;   // 이름 뒤에 Clone 제거
      isSpawning = false;
    }
  }

  GameObject GetRandomEnemy(GameObject[] enemyArray)
  {
    float totalWeight = 0f;

    foreach (GameObject obj in enemyArray)
    {
      EnemyController ec = obj.GetComponent<EnemyController>();
      if (ec != null)
      {
        totalWeight += ec.spawnWeight;
      }
    }

    float randomValue = Random.Range(0f, totalWeight);
    float currentWeight = 0f;

    foreach (GameObject obj in enemyArray)
    {
      EnemyController ec = obj.GetComponent<EnemyController>();
      if (ec != null)
      {
        currentWeight += ec.spawnWeight;
        if (randomValue <= currentWeight)
        {
          return obj;
        }
      }
    }

    return enemyArray[0]; // fallback
  }



  // 탐색 범위를 보여주는 메서드
  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, stopSpawnDistance);
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, range);
  }

  // 웨이브 진행시 소환 풀 업데이트
  public void UpdateEnemyPool(int wave)
  {
    waveNum = wave; // 현재 텔레포터의 웨이브 번호와 일치시킴

    if (waveNum < 4)
    {
      enemy = enemyPool[waveNum - 1];

    }
  }

  // 오브젝트 활성화시 호출되는 함수
  private void OnEnable()
  {
    TeleporterManager.GoNextWave += UpdateEnemyPool;
  }

  // 오브젝트 비활성화시 호출되는 함수
  private void OnDisable()
  {
    TeleporterManager.GoNextWave -= UpdateEnemyPool;
  }

}
