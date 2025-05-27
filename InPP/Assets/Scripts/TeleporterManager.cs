using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class TeleporterManager : MonoBehaviour
{
  GameObject player;
  PlayerController pc;

  public GameObject[] BossPool;                // 현재 스테이지 보스 배열
  GameObject currentBoss;                 // 현재 소환된 보스

  public GameObject gate;

  Slider slider;    // 텔레포터의 진행률을 나타낼 슬라이더
  Text waveText;    // 웨이브를 나타낼 텍스트
  Image background; // 슬라이더와 텍스트 뒤에 깔리는 배경

  public float[] maxChargeVals; // 웨이브 별 채워야하는 최대 적 Hp
  float maxChargeVal; // 웨이브 별 채워야하는 최대 적 Hp
  int currentChargeVal; // 현재 채워진 수치

  int waveNum;    // 현재 진행중인 웨이브
  public string nextSceneName;    // 다음 씬 이름

  public bool isStageStart;         // 스테이지 시작 후 텔레포트 활성화용 변수
  public bool isPreparing;       // 준비 중 여부
  public bool isBossSpawn;      // 보스 소환 여부
  bool isBossStarting = false;         // 보스 소환 후 딜레이 대기 중
  bool isBossCleared = false;  // 보스 클리어 처리 여부

  public static TeleporterManager Instance;

  public static event Action<int> GoNextWave; // 웨이브 변경 이벤트

  void Awake()
  {
    
  }


  void Start()
  {
    StartCoroutine(Starting());
  }

  IEnumerator Starting()
  {
    // 플레이어가 생성될 때까지 기다림
    while (GameObject.FindWithTag("Player") == null)
    {
      yield return null; // 다음 프레임까지 대기
    }

    player = GameObject.FindWithTag("Player");

    pc = player.GetComponent<PlayerController>();

    slider = GetComponentInChildren<Slider>();  // 슬라이더 가져오기
    waveText = GetComponentInChildren<Text>();  // 텍스트 가져오기
    background = GetComponentInChildren<Image>();  // 배경 이미지 가져오기
    gate = GameObject.Find("Gate");     // 가로막는 게이트 가져오기
    if (gate != null)
    {
      gate.SetActive(false);
    }


    // 초기 설정
    waveNum = 0;            // 웨이브 넘버 0

    currentChargeVal = 0;   // 수치 0
    maxChargeVal = maxChargeVals[0];
    isStageStart = true;
    isPreparing = true;
    isBossSpawn = false;

    waveText.text = "Wave : " + waveNum.ToString(); // 웨이브 숫자 표시


    TeleporterUpdate(currentChargeVal); // 초기화 (여기서 웨이브 넘버 ++)

  }

  void Update()
  {
    if (pc == null || pc.isPaused)
      return; // 아직 초기화가 안 된 경우 아무 것도 안함

    
    if(isStageStart)
    {
      ActiveTeleporter();
      return;
      
    }


    //다음 스테이지 메서드
    if (isBossSpawn && currentBoss == null)      // 보스 클리어시
    {
      if (!isBossCleared)
      {
        KillAllEnemies();
        isBossCleared = true;   // 플래그 설정
      
      }


      if (gate != null)
      {
        // 게이트 비활성화
        gate.SetActive(false);
      }

      isPreparing = true;

      slider.gameObject.SetActive(false);   // 슬라이더 비활성화

      bool isdetected = false;  // 현재 감지 상태 계산
      float range = 2.0f;       // 탐지 범위

      Collider2D[] cols;

      cols = Physics2D.OverlapCircleAll(transform.position, range);  // 중심으로 일정 범위 내에 있는 콜라이더

      // 감지된 콜라이더 검사 실행
      foreach (Collider2D col in cols)
      {
        // 콜라이더의 태그가 Player라면
        if (col != null && col.CompareTag("Player"))
        {
          isdetected = true;
          break;
        }
      }

      if (isdetected)    // 범위내에 플레이어가 있으면
      {
        background.gameObject.SetActive(true);   // 배경 활성화
        background.rectTransform.localPosition = new Vector2(1.5f, 500);    // 배경 위치 옮기기
        background.rectTransform.sizeDelta = new Vector2(1500, 300);    // 배경 키우기

        // 텍스트 업데이트
        waveText.fontSize = 90;
        waveText.text = "다음 스테이지로 넘어갑니다.\nEnter : 확인";
        waveText.gameObject.SetActive(true);

        GoNextStage();
      }

      else
      {
        background.gameObject.SetActive(false);   // 배경 비활성화
        waveText.gameObject.SetActive(false);   // 텍스트 비활성화
      }
    }

    // 4웨이브일 때 플레이어가 다가오면 보스 소환을 물어봄
    if (waveNum == 4 && !isBossSpawn && !isBossStarting)
    {
      DetectPlayerForBoss();
    }

  }


  void OnEnable()
  {
    EnemyController.OnEnemyDeath += TeleporterUpdate;    // 이벤트 구독
  }

  void OnDisable()
  {
    EnemyController.OnEnemyDeath -= TeleporterUpdate;    // 구독 해제
  }

  void TeleporterUpdate(int hp)
  {
    // 준비 중이 아니면서 보스가 소환되기 전에 죽은 적 체력만큼 텔레포터 진행도 추가
    if (isPreparing == false && !isBossSpawn)
    { currentChargeVal += hp; }

    // 슬라이더 반영
    slider.maxValue = maxChargeVal;
    slider.value = currentChargeVal;

    // 텔레포터가 다 채워지면
    if (currentChargeVal >= maxChargeVal)
    {
      StartCoroutine(NextWave());

    }
  }

  IEnumerator NextWave()
  {
    isPreparing = true;

    background.rectTransform.sizeDelta = new Vector2(1200, 300);    // 배경 키우기
    background.rectTransform.localPosition = new Vector2(1.5f, 440);    // 배경 위치 옮기기
    waveText.gameObject.SetActive(true);                           // 웨이브 텍스트 활성화

    // 텍스트 변경
    waveText.text = "준비중...";

    // 이펙트 추가 코드 (가능하면)


    waveNum++;     // 웨이브 넘버 추가
    currentChargeVal = 0;   // 진행률 초기화 
    if(waveNum < 4)
    {
      maxChargeVal = maxChargeVals[waveNum - 1];
    }


    KillAllEnemies();     // 모든 적 제거

    // 3웨이브 클리어시
    if (waveNum >= 4)
    {
      isPreparing = true;
      slider.gameObject.SetActive(false); 
      yield break;
    }

    // 5초 후 재가동
    yield return new WaitForSeconds(5);

    GoNextWave?.Invoke(waveNum);

    waveText.text = "Wave : " + waveNum.ToString();   // 텍스트 업데이트

    slider.gameObject.SetActive(true);

    if (!isBossSpawn)    // 보스를 잡은 상태가 아니라면
    {
      isPreparing = false;      // 텔레포터 활성화

    }


    // 재 가동 후 3초 후에 텍스트 비활성화 후 배경 크기 줄이기
    yield return new WaitForSeconds(3);
    background.rectTransform.sizeDelta = new Vector2(1200, 145);    // 배경 줄이기
    background.rectTransform.localPosition = new Vector2(1.5f, 355);    // 배경 위치 옮기기
    waveText.gameObject.SetActive(false);                           // 웨이브 텍스트 비활성화

  }

  // 씬에 있는 모든 적 제거
  void KillAllEnemies()
  {
    // 씬 내 모든 EnemyController 컴포넌트를 가진 오브젝트 찾기
    EnemyController[] enemies = FindObjectsOfType<EnemyController>();

    // 모든 적 제거
    foreach (EnemyController enemy in enemies)
    {
      enemy.ForceDeath();  // 강제 사망 실행
    }

  }

  void DetectPlayerForBoss()
  {
    bool isDetected = false;
    float range = 2.0f;

    Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range);
    foreach (var col in cols)
    {
      if (col != null && col.CompareTag("Player"))
      {
        isDetected = true;
        break;
      }
    }

    if (isDetected)
    {
      background.gameObject.SetActive(true);
      background.rectTransform.localPosition = new Vector2(1.5f, 500);
      background.rectTransform.sizeDelta = new Vector2(1500, 300);

      waveText.fontSize = 90;
      waveText.text = "보스를 소환합니다.\nEnter : 확인";
      waveText.gameObject.SetActive(true);

      // 엔터를 누르면 3초 후 보스 소환
      if (Input.GetKeyDown(KeyCode.Return))
      {
        isBossStarting = true;
        waveText.text = "준비중...";
        StartCoroutine(DelayedBossSpawn());
      }
    }
    else
    {
      background.gameObject.SetActive(false);
      waveText.gameObject.SetActive(false);
    }
  }

  IEnumerator DelayedBossSpawn()
  {
    yield return new WaitForSeconds(5f);

    background.gameObject.SetActive(false);
    waveText.gameObject.SetActive(false);

    SpawnBoss();      // 보스 소환
    isPreparing = false;
  }

  void SpawnBoss()
  {

    // 몬스터 소환
    int index = UnityEngine.Random.Range(0, BossPool.Length);

    currentBoss = Instantiate(BossPool[index], transform.position + new Vector3(0, 1), Quaternion.identity);
    currentBoss.name = BossPool[index].name;  // 이름 뒤에 Clone 제거

    isBossSpawn = true;

  }

  // 다음 스테이지로 가는 메서드
  void GoNextStage()
  {
    // 엔터를 누르면
    if(Input.GetKeyDown(KeyCode.Return))
    {
      // 만약 nextSceneName이 비어있지 않다면 해당 씬으로 이동
      if (!string.IsNullOrEmpty(nextSceneName))
      {
        int currentStage = GameManager.Instance.StageNumber;

        // 페이더 찾기
        ScreenFader fader = FindObjectOfType<ScreenFader>();
        string sceneToLoad = (currentStage == 3) ? "EndingScene" : "LoadingScene";

        if (currentStage == 3)
        {
          // SaveManager에서 게임 클리어 메서드 호출
          FindObjectOfType<SaveManager>()?.GameClear();


        }

        else
        {
          PlayerPrefs.SetString("NextScene", nextSceneName); // 다음 씬 이름 임시 저장
        }

        // 페이드 후 엔딩씬으로 전환
        if (fader != null)
        {
          fader.FadeAndLoadScene(sceneToLoad);
        }

        else
        {
          // 페이더 없으면 바로 전환
          SceneManager.LoadScene(sceneToLoad);
        }

      }
      else
      {
        Debug.LogWarning("씬 없음");
      }
    }

  }

  // 스테이지 시작시 텔레포터 가동을 묻는 메서드
  void ActiveTeleporter()
  {
    slider.gameObject.SetActive(false);   // 슬라이더 비활성화

    bool isdetected = false;  // 현재 감지 상태 계산
    float range = 2.0f;       // 탐지 범위

    Collider2D[] cols;

    cols = Physics2D.OverlapCircleAll(transform.position, range);  // 중심으로 일정 범위 내에 있는 콜라이더

    // 감지된 콜라이더 검사 실행
    foreach (Collider2D col in cols)
    {
      // 콜라이더의 태그가 Player라면
      if (col != null && col.CompareTag("Player"))
      {
        isdetected = true;
        break;
      }
    }

    if (isdetected)    // 범위내에 플레이어가 있으면
    {
      background.gameObject.SetActive(true);   // 배경 활성화
      background.rectTransform.localPosition = new Vector2(1.5f, 500);    // 배경 위치 옮기기
      background.rectTransform.sizeDelta = new Vector2(1500, 300);    // 배경 키우기

      // 텍스트 업데이트
      waveText.fontSize = 100;
      waveText.text = "텔레포터를 가동합니다.\nEnter : Y";
      waveText.gameObject.SetActive(true);

      // 엔터를 누르면
      if (Input.GetKeyDown(KeyCode.Return))
      {
        if (gate != null)
        {
          gate.SetActive(true);
        }

        isStageStart = false;
        StartCoroutine(NextWave());
      }
      
    }

    else
    {
      background.gameObject.SetActive(false);   // 배경 비활성화
      waveText.gameObject.SetActive(false);   // 텍스트 비활성화
    }
  }

  // 탐색 범위를 보여주는 메서드
  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, 3.0f);
  }

}
