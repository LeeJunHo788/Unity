using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
  public System.Action OnHit;
  protected bool isInvincible = false; //피격시 무적여부

  [Header("할당 오브젝트")]
  public Material hitMat;                 // 피격 마테리얼
  public Slider hpSlider;                //플레이어 체력바
  public GameObject mainUI;             // 메인UI
  public LayerMask itemLayer;          // 획득 아이템 레이어



  [Header("능력치")]
  public float currentHp;     // 현재 체력
  public float maxHp;       // 최대 체력
  public float attackDem;      // 공격력
  public float defence;     // 방어력
  public float defIgnore;   // 방어무시
  public float attackSpeed; // 공격 속도
  public float moveSpeed;   // 이동속도
  public float knockBackForce;    // 넉백힘
  public float criticalChance;    // 치명타 확률
  public float criticalDem;       // 치명타 피해량
  public float getRange;        // 획득 사거리
  public float goldAcq;         // 골드 획득량
  public float expAcq;          // 경험치 획득량
  public float invinvibleTime; //무적시간

  [Header("기타 수치")]
  public int killCount;     // 처치한 적 수
  public int stageGold;     // 스테이지에 누적된 골드
  public int gold;        // 소지금


  float maxExp;        // 최대 경험치
  float currentExp;    // 현재 경험치
  public int level;           // 레벨


  List<Transform> pullingItems = new List<Transform>();   // 당겨지고 있는 아이템 배열

  Animator animator;            //플레이어 애니메이터 추가
  Material originMat;           // 기본 마테리얼
  GameObject gameManager;       //게임매니저
  GameManager gm;               //게임매니저 스크립트
  SpriteRenderer sp;          //스프라이트렌더러
  Rigidbody2D rb;
  LevelSystem ls;
  LevelControl lc;
  public Slider expSlider;              // 경험치 슬라이더
  public TextMeshProUGUI killCountText; // 처치 수 텍스트
  public TextMeshProUGUI goldText;      // 소지금 텍스트
  public TextMeshProUGUI levelText;      // 레벨 텍스트

  //효과음
  private AudioSource audioSource;


  private void Awake()
  {
    // 스탯 설정
    maxHp = 100f;
    currentHp = maxHp;
    attackDem = 10;
    defence = 30;
    defIgnore = 0;
    attackSpeed = 1f;
    moveSpeed = 5;
    knockBackForce = 5f;
    criticalChance = 5;
    criticalDem = 2;
    getRange = 2f;
    goldAcq = 1f;
    expAcq = 1f;
    invinvibleTime = 0.5f;

    // 초기 경험치, 레벨 설정
    level = 1;
    maxExp = 30;
    currentExp = 0;

    // 
    killCount = 0;
    stageGold = 0;
  }

  private IEnumerator Start()
  {
    animator = GetComponent<Animator>();
    originMat = GetComponent<SpriteRenderer>().material;
    sp = GetComponent<SpriteRenderer>();
    rb = GetComponent<Rigidbody2D>();
    audioSource = GetComponent<AudioSource>();
    StatInit();



    while (mainUI == null || gameManager == null)
    {
      if (mainUI == null)
        mainUI = GameObject.Find("MainUI");

      if (gameManager == null)
      {
        gameManager = GameObject.Find("GameManager");
        if (gameManager != null)
        {
          gm = gameManager.GetComponent<GameManager>();
        }
      }

      yield return null; // 둘 중 하나라도 못 찾으면 기다림
    }



     InitializeMainUI(); // 둘 다 찾은 다음에 초기화!!
    ls = GameObject.Find("LevelSystem").GetComponent<LevelSystem>();
    lc = GameObject.Find("LevelControl").GetComponent<LevelControl>();
  }

  /*
  private void Start()
  {
    
    while(gameManager == null)
    {
      animator = GetComponent<Animator>();
      originMat = GetComponent<SpriteRenderer>().material;
      sp = GetComponent<SpriteRenderer>();        // 스프라이트렌더러 가져오기"
      gameManager = GameObject.Find("GameManager");   // 게임 매니저 오브젝트 찾기
      gm = gameManager.GetComponent<GameManager>();  // 게임 매니저 스크립트 가져오기

    }

      InitializeMainUI();
    

  }
  */

  private void Update()
  {
    if (currentHp == 0)
      return;

    Move();

    PullObject();

    GetExp();

    //임시 메서드
    StatReset();

    HpUp();
  }

  void HpUp()
  {
    if (Input.GetKey(KeyCode.Space))
    {
      currentHp = maxHp;
      hpSlider.value = currentHp;
    }

  }

  void Move()
  {
    Vector2 dir = Vector2.zero;   // 이동 방향

    if (Input.GetKey(KeyCode.LeftArrow))
    {
      dir += Vector2.left;
    }

    if (Input.GetKey(KeyCode.RightArrow))
    {
      dir += Vector2.right;
    }

    if (Input.GetKey(KeyCode.UpArrow))
    {
      dir += Vector2.up;
    }

    if (Input.GetKey(KeyCode.DownArrow))
    {
      dir += Vector2.down;
    }

    // 방향 전환 처리
    if (dir.x < 0)
    {
      sp.flipX = true;   // 왼쪽으로 이동 시 왼쪽을 바라봄
    }
    else if (dir.x > 0)
    {
      sp.flipX = false;  // 오른쪽으로 이동 시 오른쪽을 바라봄
    }

    // Rigidbody의 속도를 직접 설정해서 이동
    rb.velocity = dir.normalized * moveSpeed;

    // 이동에 따른 애니메이션 변경
    if (animator != null)
    {
      animator.SetInteger("State", dir != Vector2.zero ? 1 : 0);
    }
  }

  //공격 받았을때 체력바 깎이게 하기
  public void TakeDamage(float damage, float defIgn)
  {
    currentHp -= Mathf.RoundToInt(damage - (damage * (defence - (defence * (defIgn * 0.01f))) * 0.01f));

    //체력 범위 제한
    currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    hpSlider.value = currentHp;

    if(currentHp == 0)
    {
      StartCoroutine(Dead());
      return;
    }

    StartCoroutine(MaterialShift());

  }

  IEnumerator MaterialShift()
  {
    GetComponent<SpriteRenderer>().material = hitMat;   // 마테리얼 변경(빨간색 깜빡임 효과)
    yield return new WaitForSeconds(0.15f); // 0.15초
    GetComponent<SpriteRenderer>().material = originMat;  // 마테리얼 복귀
  }

  // 무적시간 메서드
  IEnumerator Invincible()
  {
    isInvincible = true;    // 무적 활성화

    yield return new WaitForSeconds(invinvibleTime);    // 무적시간 후 무적 비활성화
    isInvincible = false;
  }


  private void OnTriggerStay2D(Collider2D collision)
  {

    // 적과 피격 시
    if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
    {
      if (isInvincible) return;       // 무적중 이라면 피격 무효

      EnemyController ec = collision.GetComponent<EnemyController>(); // 스크립트 가져오기
      float damage = ec.attackDem;    // 데미지 가져오기
      float enemyDefIgnore = ec.defIgnore;  // 방어력 무시 가져오기
      OnHit?.Invoke();

      TakeDamage(damage, enemyDefIgnore);    // 데미지 적용
      StartCoroutine(Invincible());   // 무적 활성화

    }

    // 아이템 접촉시
    if (collision.CompareTag("Item"))
    {
      gm.GetItem();   // 아이템 획득 메서드 호출
      Destroy(collision.gameObject);
    }

  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    // 적 공격에 피격 시
    if (collision.CompareTag("EnemyAttack"))
    {
      if (isInvincible) return;       // 무적중 이라면 피격 무효

      EnemyAttack ea = collision.GetComponent<EnemyAttack>();
      float damage = ea.attDamage;
      float enemyDefIgnore = ea.DefIgnore;

      OnHit?.Invoke();

      TakeDamage(damage, enemyDefIgnore);    // 데미지 적용(수치 추후 수정)

      if (ea.destroyOnHit == true)   // 피격시 사라지는 오브젝트라면 피격시 삭제
        Destroy(collision.gameObject);

      StartCoroutine(Invincible());   // 무적 활성화
    }

    else if (collision.CompareTag("Shop"))
    {
      return;
    }

  }


  void GetExp()
  {
    // 탐지 반경 안의 경험치 오브젝트들을 모두 가져옴
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f, itemLayer);

    foreach (Collider2D hit in hits)
    {
      if (hit.CompareTag("Exp"))  // 태그 확인
      {
        //효과음 재생 먼저
        if(audioSource != null && SFXManager.instance != null && SFXManager.instance.item != null)
        {
          audioSource.PlayOneShot(SFXManager.instance.item);
        }

        ExpPlus(5f * expAcq);     // 경험치 획득
        Destroy(hit.gameObject); // 경험치 오브젝트 제거
      }
    }
  }


  // 경험치 획득 메서드
  void ExpPlus(float expVal)
  {
    currentExp += expVal;

    if (currentExp >= maxExp)
    {
      float overVal = currentExp - maxExp;  // 초과분 저장
      level++;  // 레벨업
      levelText.text = $"Lv : {level}";

      currentExp = 0;   // 현재 경험치 초기화
      maxExp += 10;     // 경험치 통 증가

      currentExp += overVal;                // 초과분 적용

      expSlider.maxValue = maxExp;

      gm.GetItem();   // 아이템 획득 메서드 호출
    }


    expSlider.value = currentExp;

  }

  // 오브젝트 당겨오는 메서드
  void PullObject()
  {
    // 획득 범위 내 아이템레이어를 가진 오브젝트 찾기
    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, getRange, itemLayer);

    // 반복문으로 하나하나 당겨오기
    foreach (Collider2D collider in colliders)
    {
      if (!pullingItems.Contains(collider.transform))  // 이미 당기고 있는 아이템은 제외
      {
        pullingItems.Add(collider.transform);          // 리스트 추가
        StartCoroutine(PullObjectCoroutine(collider.transform));
      }
    }

  }

  // 오브젝트 당겨오는 코루틴
  IEnumerator PullObjectCoroutine(Transform item)
  {
    float timer = 0f;
    float acceleration = 50f;    // 가속도
    float currentVelocity = 0f; // 현재 속도
    float maxVelocity = 10f;    // 최대 속도

    while (item != null)
    {
      timer += Time.deltaTime;
      currentVelocity = Mathf.Min(acceleration * timer, maxVelocity);   // 최대속도 제한

      Vector2 dir = (transform.position - item.position).normalized;
      item.position += (Vector3)(dir * currentVelocity * Time.deltaTime);

      yield return null;
    }

    pullingItems.Remove(item);    // 당겨오기 끝나면 제거

  }

  public void KillCountUpdate()
  {
    killCountText.text = $"Kill : {killCount}";
  }

  // 웨이브 종료시 킬 카운트 만큼 골드 추가
  public void AddGold()
  {
    int finalGold = killCount;

    switch (ls.L_State)
    {
      case LevelSystem.LevelState.Normal:
        break;
      case LevelSystem.LevelState.Hard:
        finalGold = Mathf.RoundToInt(finalGold * 1.5f);
        break;
      case LevelSystem.LevelState.VeryHard:
        finalGold = Mathf.RoundToInt(finalGold * 2f);
        break;
      default:
        break;
    }

    if (lc != null && lc.isWeather)
      finalGold *= 2;


    stageGold += finalGold;
    PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + killCount);
    killCount = 0;

    KillCountUpdate();

    goldText.text = $"Gold : {stageGold}";
  }

  IEnumerator Dead()
  {
    animator.SetTrigger("Dead");

    yield return new WaitForSeconds(1.5f);
    Time.timeScale = 0f;

    GameObject waveUI = GameObject.Find("WaveUI");
    WaveController wc = waveUI.GetComponent<WaveController>();

    wc.ShowGameOverUI();
  }

  public void InitializeMainUI()
  {
    while (mainUI == null)
    {
      mainUI = GameObject.Find("MainUI");
    }

    if (mainUI != null)
    {
      // 메인 UI에서 슬라이더, 텍스트 할당
      goldText = mainUI.transform.Find("GoldText").GetComponent<TextMeshProUGUI>();
      killCountText = mainUI.transform.Find("KillCountText").GetComponent<TextMeshProUGUI>();
      expSlider = mainUI.transform.Find("ExpSlider").GetComponent<Slider>();
      levelText = mainUI.transform.Find("LevelText").GetComponent<TextMeshProUGUI>();

      //체력바 초기 설정
      hpSlider.maxValue = maxHp;
      hpSlider.minValue = 0;
      hpSlider.value = currentHp;

      // 경험치바 초기 설정
      expSlider.maxValue = maxExp;
      expSlider.minValue = 0;
      expSlider.value = currentExp;

      levelText.text = $"Lv : {level}";

    }
  }

  // 스탯 초기화(스테이지 진입 or 스테이지 선택 씬 진입)
  public void StatInit()
  {
    maxHp = 40f + PlayerPrefs.GetFloat("maxHp");
    currentHp = maxHp;
    attackDem = 10 + PlayerPrefs.GetFloat("attackDem");
    defence = 10 + PlayerPrefs.GetFloat("defence");
    defIgnore = 10;
    attackSpeed = 1f;
    moveSpeed = 5;
    knockBackForce = 5f;
    criticalChance = 10;
    criticalDem = 2;
    getRange = 2f + PlayerPrefs.GetFloat("getRange");
    goldAcq = 1f + PlayerPrefs.GetFloat("goldAcq");
    expAcq = 1f + PlayerPrefs.GetFloat("expAcq");
    invinvibleTime = 0.5f;
    gold = PlayerPrefs.GetInt("Gold") + 100000;


    if (currentHp <= 0)
      currentHp = maxHp;
  }

  // 임시 스탯 공장 초기화 메서드
  public void StatReset()
  {
    if(Input.GetKeyDown(KeyCode.P))
    {
      // 스탯 초기화
      maxHp = 40f;
      currentHp = maxHp;
      attackDem = 10;
      defence = 10;
      defIgnore = 10;
      attackSpeed = 1f;
      moveSpeed = 5;
      knockBackForce = 5f;
      criticalChance = 10;
      criticalDem = 2;
      getRange = 2f;
      goldAcq = 1f;
      expAcq = 1f;
      invinvibleTime = 0.5f;


      // 플레이어 프리팹스 초기화 산 스탯 초기화
      PlayerPrefs.DeleteAll();
    }
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    StatInit();
    Debug.Log("스탯 불러오기");
  }

  public void AttractExp()
  {
    GameObject[] exps = GameObject.FindGameObjectsWithTag("Exp");

    foreach (GameObject exp in exps)
    {
      StartCoroutine(PullExpToPlayer(exp.transform));
    }
  }

  IEnumerator PullExpToPlayer(Transform exp)
  {
    float speed = 0f;
    float acceleration = 20f;

    while (exp != null)
    {
      speed += acceleration * Time.deltaTime;
      Vector3 direction = (this.transform.position - exp.position).normalized;
      exp.position += direction * speed * Time.deltaTime;

      if (Vector3.Distance(exp.position, this.transform.position) < 0.1f)
      {
        break;
      }
      yield return null;
    }
  }
}

