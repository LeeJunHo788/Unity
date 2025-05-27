using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;

public class SpawnManager : MonoBehaviour
{
  [Header("적")]
  public GameObject[] basicEnemies; // 일반 적 프리팹
  public GameObject[] dashEnemies; // 돌진 적 프리팹
  public GameObject[] longDistanceEnemies; // 돌진 적 프리팹
  public GameObject[] eliteMonster;   // 엘리트
  public GameObject[] bossPrefab;         // 보스 프리팹

  [Header("이벤트 오브젝트")]
  public GameObject[] eventObjects;

  public float spawnInterval; // 적 스폰 간격
  public float waveTime; // 웨이브 지속 시간
  public int currentWave; // 현재 웨이브

  public int maxWave;
  private GameObject currentBoss;       // 현재 소환된 보스
  private bool isBossWave = false;      // 보스 웨이브 여부
  private bool bossDead = false;        // 보스 사망 체크

  public float startSpawnInterval;  // 웨이브 시작 시점의 스폰 간격
  public float minSpawnInterval = 0.6f; // 가장 빠르게 되는 최소값
  private float spawnDecreaseInterval = 4f; // 몇 초마다 줄일지
  private float lastDecreaseTime; // 마지막으로 감소한 시점
  private float spawnDecreaseAmount = 0.3f; // 얼마나 줄일지

  public Text waveTimer; // 웨이브 타이머 UI
  public TextMeshProUGUI waveNumText; // 웨이브 텍스트 UI
  private float waveStartTime; // 웨이브 시작 시간
  private bool isPreparing;    // 준비 중 여부

  GameObject player;
  PlayerController pc;

  public WaveController waveController; //연출용 트랜지션
  public GameObject magnetObj;
  public float magnetSpawnChance;

  private void Awake()
  {
    // 1웨이브 설정
    currentWave = 0;
    spawnInterval = 3f;
    waveTime = 15f;
    minSpawnInterval = 0.6f;
    magnetSpawnChance = 0.01f;
    spawnDecreaseInterval = 3f;

    if (SceneManager.GetActiveScene().name == "Stage1")
      maxWave = 31;
    else
      maxWave = int.MaxValue;

  }

  private void Start()
  {
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();

    waveTimer = GameObject.Find("Spawntimer").GetComponent<Text>();
    waveNumText = GameObject.Find("WaveNum").GetComponent<TextMeshProUGUI>();


    waveStartTime = Time.time; // 웨이브 시작 시간 초기화
    StartCoroutine(SpawnWave()); // 웨이브 스폰 코루틴 시작 

    EnemyController.OnDeath += () => TrySpawnMagnet();
  }
  private void Update()
  {
    UpdateWaveTimer(); // 웨이브 타이머 업데이트
  }
  private IEnumerator SpawnWave()
  {
   
    startSpawnInterval = spawnInterval; // 웨이브 시작 시점 간격 저장

    while (currentWave != maxWave)
    {
      lastDecreaseTime = Time.time;
      spawnInterval = startSpawnInterval;


      // 씬에 존재하는 모든 적들 강제 사망
      GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
      foreach (var enemy in enemies)
      {
        EnemyController ec = enemy.GetComponent<EnemyController>();
        ec.ForceDie();

      }

      GameObject boss = GameObject.FindGameObjectWithTag("Boss");
      if (boss != null)
      {
        EnemyController bc = boss.GetComponent<EnemyController>();
        bc.ForceDie();

      }
      GameObject eventObj = GameObject.FindGameObjectWithTag("EventObject");

      isPreparing = true;
      waveStartTime = Time.time;
      currentWave++; // 웨이브 증가
      AudioManager.instance?.BossBGM(currentWave);//보스전용브금재생위해씬넘버넘겨주기


      foreach (var ev in EventSystemObject.activeEvents.ToArray())
      {
        ev.ForceEndEvent();
      }

      //준비 UI 호출
      if (currentWave % 10 == 0)
      {
        waveController.PlayBossIntro();
      }
      else if (currentWave == 1)
      {
        waveController.ShowReadyPanel();
      }

      else if (currentWave == maxWave)
      {
        waveController.ShowGameClearUI();
        //스테이지 클리어 처리 확인
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.clearedStage1 = true; //스테이지1클리어등록(무한모드해금조건)
        Debug.Log("해금완룡");
        yield break;
      }

      float prepareDuration = 5f; // 준비 시간 설정
      float prepareEndTime = Time.time + prepareDuration;

      // 준비 시간 동안 대기
      while (Time.time < prepareEndTime)
      {
        yield return null;  // 매 프레임 대기 (타이머 갱신용)
      }

      if (currentWave % 3 == 0 && currentWave != 30)
      {
        SpawnRandomEventObject();
      }


      isPreparing = false;

      waveStartTime = Time.time; // 웨이브 시작 시간 초기화

      

      // 5웨이브마다 난이도 조절
      if (currentWave % 5 == 0)
      {
        startSpawnInterval = Mathf.Max(1f, spawnInterval - 0.5f); // 최소 1초까지 감소
        waveTime = Mathf.Min(60f, waveTime + 10f); // 최대 60초까지 증가
        minSpawnInterval = Mathf.Max(0.03f, minSpawnInterval - 0.1f);
      }

      isBossWave = (currentWave % 10 == 0);
      bossDead = false;

      pc.AddGold(); // 웨이브 종료 시 플레이어 소지금 추가 메서드 호출



      //시작 UI 호출
      if (isBossWave)
      {
        waveNumText.text = "BOSS!";
        yield return StartCoroutine(SpawnBossWave(currentWave));
      }

      else
      {
        if (currentWave % 5 == 0 && currentWave % 10 != 0)
        {
          int index = 0;
          if (currentWave == 15) index = 1;
          else if (currentWave == 25) index = 2;
          Instantiate(eliteMonster[index], RandomPositionAroundPlayer(player.transform, 10f), Quaternion.identity);
        }
        waveNumText.text = $"Wave : {currentWave}";
        waveController.ShowGameStartUI();
        float waveEndTime = Time.time + waveTime;   // 현재 웨이브 종료 시간 설정
        while (Time.time < waveEndTime)
        {
          // 5초마다 spawnInterval 감소
          if (Time.time - lastDecreaseTime >= spawnDecreaseInterval)
          {
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnDecreaseAmount);
            lastDecreaseTime = Time.time; // 감소 시점 갱신
          }

          SpawnEnemyAtRandomPoint();
          yield return new WaitForSeconds(spawnInterval);
        }
      }
    }
  }

  private void SpawnEnemyAtRandomPoint()
  {
    if (player == null) return;

    Vector3 spawnPos = RandomPositionAroundPlayer(player.transform, 12f); // 반경 10f 안에서 랜덤 위치

    float rand = Random.Range(0f, 1f); // 0.0 ~ 1.0 사이 무작위 값

    GameObject[] chosenArray = null;

    // 적 종류 선택
    if (rand < 0.5f) // 일반 적 50%
    {
      chosenArray = basicEnemies;
    }
    else if (rand < 0.8f) // 돌진 적 30%
    {
      chosenArray = dashEnemies;
    }
    else // 원거리 적 20%
    {
      chosenArray = longDistanceEnemies;
    }

    // 강화형 등장 확률 계산
    float reinforcedChance = 0f;
    if (currentWave >= 5 && currentWave < 26)
    {
      reinforcedChance = (currentWave - 4) * 0.05f; // 5웨이브 5%
    }
    else if (currentWave >= 26)
    {
      reinforcedChance = 1f; // 100%
    }
    else
    {
      reinforcedChance = 0f; // 1~4웨이브는 강화형 없음
    }

    bool isReinforced = Random.value < reinforcedChance;

    int indexToSpawn = isReinforced ? 1 : 0;
    indexToSpawn = Mathf.Clamp(indexToSpawn, 0, chosenArray.Length - 1); // 배열 길이 체크

    GameObject prefabToSpawn;

    prefabToSpawn = chosenArray[indexToSpawn];
    Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
  }

  private void UpdateWaveTimer()
  {
    if (waveTimer != null)
    {
      float timeLeft;

      if (isPreparing)
      {
        // 준비 중이면 준비 시간 타이머 표시
        timeLeft = 5f - (Time.time - waveStartTime);
        waveTimer.text = "Time Left: " + Mathf.Ceil(timeLeft).ToString();
      }
      else
      {
        // 보스 웨이브라면
        if (isBossWave)
        {
          timeLeft = 15f + waveTime - (Time.time - waveStartTime); // 남은 시간 계산
        }

        // 웨이브 중이면 웨이브 타이머 표시
        else
        {
          timeLeft = waveTime - (Time.time - waveStartTime); // 남은 시간 계산
        }
        waveTimer.text = "Time Left: " + Mathf.Ceil(timeLeft).ToString();
      }
    }
  }

  private IEnumerator SpawnBossWave(int waveNum)
  {
    //  배열 인덱스를 계산
    int bossIndex = (waveNum / 10) - 1;
    // 보스 생성
    currentBoss = Instantiate(bossPrefab[bossIndex], GetRandomSpawnPoint(), Quaternion.identity);

    // 보스가 EnemyController를 상속 받았다고 가정
    EnemyController.OnBossDeath += () => bossDead = true; // 보스 사망 

    // 웨이브 타이머 대신 BOSS! 출력
    waveTimer.text = "BOSS!";

    float waveEndTime = Time.time + waveTime;

    // 보스가 죽을 때까지 대기
    while (!bossDead && Time.time < waveEndTime)
    {

      yield return null;
    }

    if (!bossDead && currentBoss != null)
    {
      EnemyController ec = currentBoss.GetComponent<EnemyController>();
      if (ec != null)
      {
        ec.ForceDie(); // 강제 사망 처리
      }
    }

  }

  private Vector3 GetRandomSpawnPoint()
  {
    return RandomPositionAroundPlayer(player.transform, 12f); // 보스도 플레이어 주변에서 생성되도록
  }


  public void TrySpawnMagnet()
  {
    float rand = Random.value; // 0.0 ~ 1.0 랜덤값
    if (rand < magnetSpawnChance && magnetObj != null)
    {
      GameObject magnet = Instantiate(magnetObj, RandomPositionAroundPlayer(player.transform, 15f), Quaternion.identity);
      magnet.name = magnetObj.name;
    }
  }

  Vector3 RandomPositionAroundPlayer(Transform player, float radius)
  {
    float angle = Random.Range(0f, 360f);

    float radian = angle * Mathf.Deg2Rad;

    float x = Mathf.Cos(radian) * radius;
    float y = Mathf.Sin(radian) * radius;

    Vector3 randomPos = player.position + new Vector3(x, y, 0f);

    return randomPos;
  }

  private void SpawnRandomEventObject()
  {
    if (eventObjects.Length == 0 || player == null) return;

    int index = Random.Range(0, eventObjects.Length);
    GameObject eventPrefab = eventObjects[index];

    Vector3 spawnPos = RandomPositionAroundPlayer(player.transform, 5f); // 원하는 반경
    Instantiate(eventPrefab, spawnPos, Quaternion.identity);
  }



}