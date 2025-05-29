using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
  protected Rigidbody2D rb;    // 리지드 바디
  protected BoxCollider2D box;    // 박스 콜라이더
  protected AudioSource audioSource;    // 효과음 재생용 오디오 소스

  Vector2 rayStart;               // 레이가 시작하는 지점
  float rayLength;       // 레이 길이
  Vector2 rightBottom;    // 박스 콜라이더의 우측 하단을 담을 변수
  Vector2 leftBottom;     // 박스 콜라이더의 좌측 하단을 담을 변수


  protected enum PlayerState { Idle, Move, Attack, Jump, Fall, BlockIdle, BlockHit, KnockBack, Dead } // 플레이어의 상태
  protected PlayerState state;     // 상태 변수

  protected float Xvel = 0;   // 가로 이동 속도
  protected float Yvel = 0;   // 세로 이동 속도
  protected float jumpForce;    // 점프력

  protected bool onSlope;             // 경사면 여부
  protected float angle;              // 경사면과의 각도
  protected float comboTimer = 0f;    // 콤보 시간 체크

  public bool onGround = false;   // 바닥에 닿았는지 여부
  protected Animator anim;    // 애니메이터

  protected int jumpCount = 0;    // 현재 점프 횟수
  protected int maxJump;   // 최대 점프 횟수

  public int comboStep = 0;   // 현재 콤보 단계
  public float comboResetTime;   // 콤보 초기화 시간
  public bool isAttack = false;   // 공격 실행중 여부

  public bool isBlock = false;    // 방어 실행중 여부
  public bool blocking = false;    // 방어 성공 여부


  protected AnimationClip attackAnim;    // 공격 애니메이션
  public float attackTime;    // 공격 애니메이션의 길이
  public float attackSpeed;

  protected AnimationClip blockingAnim;    // 방어 애니메이션



  protected AnimationClip knockBackAnim;   // 넉백 애니메이션
  protected float knockBackTime;    // 플레이어의 넉백 시간
  protected float knockBackForce = 4.0f;    // 넉백 힘
  public bool isKnockBack = false;    // 넉백 중 여부
  protected bool isInvincible = false;          // 무적 중 여부
  float invincibleTime = 1.0f;        // 무적시간

  public bool inShop = false;         // 상점 입장 여부
  public bool isPaused = false;       // 일시정지 여부


  protected AnimationClip deathAnim;
  protected float deathTime;
  protected bool isDead = false;   // 사망 상태 확인

  [Header("스탯")]
  public float speed;   // 가로 이동 속력
  public float att;    // 플레이어 공격력
  public float def;    // 플레이어 방어력
  public int maxHp;   // 플레이어 최대 체력
  public int currentHp;    // 플레이어 현재 체력
  public float defIgn;  // 플레이어 방어력 무시   
  public float goldAcq; // 플레이어 골드 획득량
  public float itemDrop; // 플레이어 아이템 획득 확률
  public float expAcq;  // 경험치 획득량
  public int level;    // 플레이어 레벨
  public float potionCoolTime;         // 포션 쿨타임


  public int potion;    // 포션 갯수
  public string playerName;   // 플레이어 캐릭터의 이름

  public bool isPotionCoolTime = false; // 포션 쿨타임 진행상황


  protected Material defaultMat;     // 원래 머티리얼
  protected SpriteRenderer spriteRenderer;
  public Material hitFlashMat;      // 피격시 머테리얼


  public GameObject gameOverCanvas;     // 게임 오버 캔버스


  public PhysicsMaterial2D lowFrictionMaterial;  // 이동 가능할 때 사용할 물리 머티리얼
  public PhysicsMaterial2D highFrictionMaterial; // 멈출 때 사용할 물리 머티리얼

  [Header("스킬 UI")]
  public Slider psvSkillSlider;
  public Image psvSkillIcon;


  public Slider skill1Slider;
  public Image skill1Icon;
  public GameObject skill1LevelText;

  public Slider skill2Slider;
  public Image skill2Icon;
  public GameObject skill2LevelText;

  [Header("효과음")]
  public AudioClip attackClip;      // 기본 공격
  public AudioClip psvSkillClip;    // 패시브 스킬
  public AudioClip skill1Clip;      // 1번 스킬
  public AudioClip skill2Clip;      // 2번 스킬
  public AudioClip potionClip;      // 포션 사용
  public AudioClip hitClip;         // 피격


  protected bool isPsvSkillCooling = false;
  protected bool isSkill1Cooling = false;
  protected bool isSkill2Cooling = false;

  protected float psvSkillCooldown;   // 페시브 스킬 쿨타임
  protected float skill1Cooldown;   // Q 스킬 쿨타임
  protected float skill2Cooldown;   // W 스킬 쿨타임

  private static PlayerController instance;

  public static event Action<int, int> HpChange; // 체력 변경 이벤트
  public static event Action<int> PotionChange;  // 포션 사용 이벤트


  protected virtual void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject); // 씬 변경 시 오브젝트 유지
    }
    else
    {
      Destroy(gameObject); // 중복 방지
    }

    level = 1;    // 초기 레벨
  }


  protected virtual void Start()
  {
    rb = GetComponent<Rigidbody2D>();   // 리지드바디 불러오기
    box = GetComponent<BoxCollider2D>();  // 박스 콜라이더 가져오기
    audioSource = GetComponent<AudioSource>();  // 오디오 소스 가져오기

    rayStart = rightBottom;
    rayLength = 0.4f;   // 레이의 길이설정

    currentHp = maxHp; // 초기 체력


    HpChange?.Invoke(maxHp, currentHp);    // 체력 업데이트
    PotionChange?.Invoke(potion);           // 포션 업데이트

    potionCoolTime = 10f;                  // 포션 쿨타임

    state = PlayerState.Idle;


    if (skill1Slider != null) skill1Slider.value = 1f;
    if (skill2Slider != null) skill2Slider.value = 1f;

    skill1Slider.gameObject.SetActive(false);
    skill2Slider.gameObject.SetActive(false);

    skill1Icon.color = Color.gray;
    skill2Icon.color = Color.gray;


    spriteRenderer = GetComponent<SpriteRenderer>();
    defaultMat = spriteRenderer.material;    // 원래 머테리얼 저장

  }

  protected virtual void Update()
  {
    if (inShop || isPaused) return;

    // 애니메이션 실행
    PlayAnimation();

    // 레이 따라오기
    rightBottom = (Vector2)box.bounds.center + new Vector2(box.bounds.extents.x - 0.01f, -box.bounds.extents.y);
    leftBottom = (Vector2)box.bounds.center + new Vector2(-box.bounds.extents.x + 0.01f, -box.bounds.extents.y);
    if (transform.rotation.eulerAngles.y == 0)
    {
      rayStart = rightBottom;
    }
    else
    {
      rayStart = leftBottom;
    }

    // 만약 현재체력이 0보다 낮아지면
    if (currentHp <= 0)
    {
      if (isDead)
        return;

      // 움직임을 멈추고 사망 애니메이션 재생
      rb.gravityScale = 0;
      rb.velocity = Vector2.zero;
      state = PlayerState.Dead;


      // 레이어를 변경하여 판정 제거
      gameObject.layer = LayerMask.NameToLayer("Dead");

      // 객체 비활성화 (나중에 삭제로 바꿀 것)
      Invoke("Death", deathTime);

      isDead = true;

      return;
    }

    StartCoroutine(UsePotion());

    HandleSkillInput();

    if (blocking || isAttack)
    {
      box.sharedMaterial = highFrictionMaterial;
    }

    // 이동 적용
    // 넉백중이면 이동 적용 X
    if (isKnockBack || blocking) return;

    else
    {
      // 이동 처리 함수
      Move();

      // 점프 처리 함수
      Jump();

      // 땅 감지
      onGround = IsOnGround();

      if (!onGround)
      {
        box.sharedMaterial = lowFrictionMaterial; // 공중일 때는 항상 낮은 마찰력 적용
      }


      // 땅에 붙어 있을 때 공격시 가로축 움직임 정지
      if ((isAttack || isBlock) && onGround)
      {
        rb.velocity = new Vector2(0, rb.velocity.y);
      }

      else
      {
        rb.velocity = new Vector2(Xvel, rb.velocity.y);
      }

    }

    // 속도값이 0이면 기본 상태
    if (rb.velocity == new Vector2(0, 0) && state != PlayerState.BlockIdle && state != PlayerState.Attack)
    {
      state = PlayerState.Idle;
    }

    // 공격
    Attack();

    if (comboStep > 0)
    {
      comboTimer -= Time.deltaTime;

      if (comboTimer <= 0)
      {
        // 일정시간이 지나면 콤보 초기화
        comboStep = 0;
      }
    }


    if (blocking == true)
      return;




  }

  /* ========================================================================================= */

  private void Move()
  {
    Debug.DrawRay(rayStart, Vector2.down * rayLength, Color.red);

    LayerMask groundLayer = 1 << 6;

    // 박스 콜라이더 왼쪽 또는 오른쪽 아래에서 아래 방향으로 Ray를 쏨
    RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, rayLength, groundLayer);

    if (hit.collider != null) // 바닥과 충돌했을 경우
    {
      angle = Vector2.Angle(hit.normal, Vector2.up); // 바닥과 수직(Up) 벡터의 각도 계산

      if (angle > 5f) // 각도가 5도 이상이면 경사면 처리  
      {
        onSlope = true;
      }

      else
      {
        onSlope = false;
      }
    }

    // 공격, 방어 중이면 이동 불가
    if (!(isAttack || isBlock))
    {
      // 방향키를 누르면 이동
      float moveInput = 0;

      if (Input.GetKey(KeyCode.LeftArrow))
      {
        moveInput = -1;   // 왼쪽 이동
      }

      else if (Input.GetKey(KeyCode.RightArrow))
      {
        moveInput = 1;    // 오른쪽 이동
      }

      // 공격 중이 아니라면 캐릭터 회전 (공격 중에는 회전 불가)
      if (moveInput == -1)
      {
        transform.rotation = Quaternion.Euler(0, 180, 0);
        rayStart = leftBottom; // 레이가 시작되는 지점도 회전
      }

      else if (moveInput == 1)
      {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rayStart = rightBottom; // 레이가 시작되는 지점도 회전
      }


      // 상태 변경
      if ((moveInput == 1 || moveInput == -1))
      {
        state = PlayerState.Move;
      }

      if (blocking)
      {
        box.sharedMaterial = highFrictionMaterial;
      }
      else if (!onGround)
      {
        box.sharedMaterial = lowFrictionMaterial;
      }


      else if (onSlope && onGround) // 경사면 위에 서 있을 때
      {
        if (moveInput == 0)
        {
          // 방향키를 떼었을 때 높은 마찰력 적용 (멈춤)
          box.sharedMaterial = highFrictionMaterial;
        }
        else
        {
          // 방향키를 누르면 낮은 마찰력 적용 (움직일 수 있도록)
          box.sharedMaterial = lowFrictionMaterial;
        }
      }
      else
      {
        // 평지에서는 기본적으로 낮은 마찰력 유지
        box.sharedMaterial = lowFrictionMaterial;
      }

      // X축 이동값 = 방향 * 속도
      Xvel = moveInput * speed;
      Yvel = rb.velocity.y;

      rb.velocity = new Vector2(Xvel, Yvel);

    }
  }

  private bool IsOnGround()
  {
    LayerMask groundLayer = 1 << 6;

    // 오른쪽 아래에서 레이 발사
    RaycastHit2D rightHit = Physics2D.Raycast(rightBottom, Vector2.down, rayLength, groundLayer);
    Debug.DrawRay(rightBottom, Vector2.down * rayLength, Color.green); // 디버그용 레이

    // 왼쪽 아래에서 레이 발사
    RaycastHit2D leftHit = Physics2D.Raycast(leftBottom, Vector2.down, rayLength, groundLayer);
    Debug.DrawRay(leftBottom, Vector2.down * rayLength, Color.blue); // 디버그용 레이

    if (rightHit.collider != null || leftHit.collider != null)

    if ((rightHit.collider != null || leftHit.collider != null) && rb.velocity.y <= 5f) // 바닥과 충돌했을 경우
    {

      jumpCount = 0;
      return true;

    }

    return false; // 바닥이 감지되지 않으면 false
  }


  private void Jump()
  {
    // 넉백 중에 실행 불가
    if (isKnockBack) return;

    // 땅에 붙어있으면서 스페이스를 누르면 점프
    // 공격중이면 실행 불가
    if (jumpCount < maxJump && Input.GetKeyDown(KeyCode.Space) && !isAttack)
    {
      rb.velocity = new Vector2(rb.velocity.x, 0);
      rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
      jumpCount++;

    }

    // 상태 처리 (Jump or Fall)
    // 캐릭터의 Y축 이동속도가 양수라면 = 올라가고 있으면 점프상태
    if (rb.velocity.y > 0 && onGround == false
        && state != PlayerState.Attack)   // 공격 입력시 상태 변경 미반영
    {
      state = PlayerState.Jump;
    }

    else if (rb.velocity.y <= 0 && onGround == false
             && state != PlayerState.Attack)
    {
      state = PlayerState.Fall;
    }


  }

  // 공격 실행은 자식 스크립트에서
  protected virtual void Attack()
  {
    
  }


  // 공격 피격
  public void TakeDem(float dem, float defIgn, Vector2 hitDir)
  {
    // 무적이라면 피격 무시
    if (isInvincible) return;

    StartCoroutine(KnockBack(dem, defIgn, hitDir));
    StartCoroutine(InvincibleOn());

  }

  // 몸박피격
  private void Hit(Collision2D collision)
  {
    // 플레이어와 부딪힌 물체가 적이라면
    if (collision.gameObject.CompareTag("Enemy"))
    {
      // 무적이라면 피격 무시
      if (isInvincible) return;

      // 적의 스크립트 가져오기
      EnemyController ec = collision.gameObject.GetComponent<EnemyController>();

      // 적의 공격력 가져오기
      float dem = ec.att;
      float defIgn = ec.defIgn;

      // 적이 플레이어를 공격한 방향
      Vector2 hitDir = (transform.position - collision.transform.position);

      // 적이 사망 상태가 아니라면 넉백 처리
      if (!ec.isDead)
      {
        StartCoroutine(KnockBack(dem, defIgn, hitDir));
        StartCoroutine(InvincibleOn());
      }
    }

    // 플레이어와 부딪힌 물체가 공격이라면
    else if (collision.gameObject.CompareTag("EnemyAttack") || collision.gameObject.CompareTag("EnemyBullet"))
    {
      // 무적이라면 피격 무시
      if (isInvincible) return;

      // 적의 스크립트 가져오기
      EnemyAttackController eatc = collision.gameObject.GetComponent<EnemyAttackController>();

      // 적의 공격력 가져오기
      float dem = eatc.att;
      float defIgn = eatc.defIgn;

      // 적이 플레이어를 공격한 방향
      Vector2 hitDir = (transform.position - collision.transform.position);

      StartCoroutine(KnockBack(dem, defIgn, hitDir));
      StartCoroutine(InvincibleOn());
    }

    // 플레이어와 부딪힌 물체가 적이라면
    else if (collision.gameObject.CompareTag("Boss"))
    {
      // 무적이라면 피격 무시
      if (isInvincible) return;

      // 적의 스크립트 가져오기
      BossController bc = collision.gameObject.GetComponent<BossController>();

      // 적의 공격력 가져오기
      float dem = bc.att;
      float defIgn = bc.defIgn;

      // 적이 플레이어를 공격한 방향
      Vector2 hitDir = (transform.position - collision.transform.position);

      // 적이 사망 상태가 아니라면 넉백 처리
      if (!bc.isDead)
      {
        StartCoroutine(KnockBack(dem, defIgn, hitDir));
        StartCoroutine(InvincibleOn());
      }
    }
  }

  // 넉백
  IEnumerator KnockBack(float dem, float defIgn, Vector2 dir)
  {
    state = PlayerState.KnockBack;
    isBlock = false;
    isKnockBack = true;

    // 체력 감소
    currentHp -= Mathf.RoundToInt(dem - (dem * (def - (def * (defIgn * 0.01f))) * 0.01f));
    audioSource.PlayOneShot(hitClip);  // 소리 재생

    if (currentHp < 0)
    {
      currentHp = 0;
    }



    HpChange?.Invoke(maxHp, currentHp);

    // 넉백 효과
    rb.velocity = new Vector2(0, 0);

    // 공격이 왼쪽으로부터 왔을 때
    if (dir.x > 0)
    {
      rb.AddForce(new Vector2(1, 1) * knockBackForce, ForceMode2D.Impulse);
    }

    // 공격이 오른쪽으로부터 왔을 때
    else if (dir.x < 0)
    {
      rb.AddForce(new Vector2(-1, 1) * knockBackForce, ForceMode2D.Impulse);
    }

    StartCoroutine(HitFlash());

    // 넉백이 끝날때 까지 대기
    yield return new WaitForSeconds(knockBackTime);


    state = PlayerState.Idle;
    isKnockBack = false;

  }

  // 이벤트 호출을 위한 메서드
  protected void HpChangeMethod()
  {
    HpChange.Invoke(maxHp, currentHp);
  }

  // 무적시간
  IEnumerator InvincibleOn()
  {
    isInvincible = true;

    // 무적시간 후 무적 종료 
    yield return new WaitForSeconds(invincibleTime);
    isInvincible = false;
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    // 적이라면 피격
    Hit(collision);

    // 땅이라면
    if (collision.gameObject.CompareTag("Ground"))
    {
      // 점프카운트 초기화
      if (onGround)
      {
        jumpCount = 0;
      }
    }


  }

  // 피격 시각 효과
  IEnumerator HitFlash()
  {
    spriteRenderer.material = hitFlashMat;  // 머테리얼 변경
    yield return new WaitForSeconds(0.15f); // 잠깐 기다렸다가
    spriteRenderer.material = defaultMat; // 원래 색으로 복원
  }

  void OnCollisionStay2D(Collision2D collision)
  {
    Hit(collision);
  }



  void Death()
  {
    // 객체 비활성화
    gameObject.SetActive(false);

    GameObject goCanvas = Instantiate(gameOverCanvas);

    Time.timeScale = 0f;  // 게임 정지

  }

  protected virtual void HandleSkillInput()
  {
    if (Input.GetKeyDown(KeyCode.Q) && !isSkill1Cooling)
    {
      StartCoroutine(SkillCooldown(skill1Slider, skill1Cooldown, 1));
    }

    if (Input.GetKeyDown(KeyCode.W) && !isSkill2Cooling)
    {
      StartCoroutine(SkillCooldown(skill2Slider, skill2Cooldown, 2));
    }

    if (Input.GetKeyDown(KeyCode.C) && !isSkill2Cooling)
    {
      StartCoroutine(SkillCooldown(psvSkillSlider, psvSkillCooldown, 3));
    }
  }

  protected IEnumerator SkillCooldown(Slider slider, float cooldown, int skillNum)
  {
    Image icon = skill1Icon;

    if (skillNum == 1)
    {
      isSkill1Cooling = true;
      skill1Slider.gameObject.SetActive(true);
      icon = skill1Icon;
    }

    if (skillNum == 2)

    {
      isSkill2Cooling = true;
      skill2Slider.gameObject.SetActive(true);
      icon = skill2Icon;
    }

    if (skillNum == 3)

    {
      isPsvSkillCooling = true;
      psvSkillSlider.gameObject.SetActive(true);
      icon = psvSkillIcon;
    }

    if (icon != null) icon.color = Color.gray;

    float timer = 0f;


    while (timer < cooldown)
    {
      timer += Time.deltaTime;
      if (slider != null)
        slider.value = 1 - (timer / cooldown);
      yield return null;
    }

    if (slider != null)
      slider.value = 1f;

    

    if (skillNum == 1)
    {
      isSkill1Cooling = false;
      skill1Slider.gameObject.SetActive(false);
      if (icon != null) icon.color = Color.white;
    }

    if (skillNum == 2)

    {
      isSkill2Cooling = false;
      skill2Slider.gameObject.SetActive(false);
      if (icon != null) icon.color = Color.white;
    }

    if (skillNum == 3)

    {
      isPsvSkillCooling = false;
      psvSkillSlider.gameObject.SetActive(false);
      if (icon != null) icon.color = Color.white;
    }
  }


  // 오브젝트 활성화시 호출되는 함수
  protected virtual void OnEnable()
  {
    ItemManager.ItemHpChange += ItemHpChange;
    ItemManager.ItemAttChange += ItemAttChange;
    ItemManager.ItemDefChange += ItemDefChange;
    ItemManager.ItemSpdChange += ItemSpdChange;
    ItemManager.ItemAttSpdChange += ItemAttSpdChange;
    ItemManager.ItemGoldAcqChange += ItemGoldAcqChange;
    ItemManager.ItemItemDropChange += ItemItemDropChange;
    ItemManager.ItemDefIgnChange += ItemDefIgnChange;
    ItemManager.ItemJumpCountChange += ItemJumpCountChange;
  }

  // 오브젝트 비활성화시 호출되는 함수
  protected virtual void OnDisable()
  {
    ItemManager.ItemHpChange -= ItemHpChange;
    ItemManager.ItemAttChange -= ItemAttChange;
    ItemManager.ItemDefChange -= ItemDefChange;
    ItemManager.ItemSpdChange -= ItemSpdChange;
    ItemManager.ItemAttSpdChange -= ItemAttSpdChange;
    ItemManager.ItemGoldAcqChange -= ItemGoldAcqChange;
    ItemManager.ItemItemDropChange -= ItemItemDropChange;
    ItemManager.ItemDefIgnChange -= ItemDefIgnChange;
    ItemManager.ItemJumpCountChange -= ItemJumpCountChange;
  }

  // 아이템에 의한 체력 변경
  void ItemHpChange(float maxHpMulVal, float maxHpPlusVal, float currentHpMulVal, float currentHpPlusVal)
  {
    maxHp = Mathf.RoundToInt((maxHp * maxHpMulVal) + maxHpPlusVal);

    currentHp = Mathf.RoundToInt((currentHp * currentHpMulVal) + currentHpPlusVal);


    if (currentHp > maxHp)
    {
      currentHp = maxHp;
    }

    // UI에게 전달
    HpChange?.Invoke(maxHp, currentHp);
  }


  // 아이템에 의한 공격력 변경
  void ItemAttChange(float attMulVal, float attPlusVal)
  {
    att = (attMulVal * att) + attPlusVal;

  }

  // 아이템에 의한 방어력 변경
  void ItemDefChange(float defMulVal, float defPlusVal)
  {
    def = (defMulVal * def) + defPlusVal;
    if (def > 99.9f)
      def = 99.9f;
  }

  // 아이템에 의한 이동속도 변경
  void ItemSpdChange(float spdMulVal, float spdPlusVal)
  {
    speed = (spdMulVal * speed) + spdPlusVal;

    if (speed > 10)
      speed = 10;

  }

  // 아이템에 의한 공격속도 변경
  void ItemAttSpdChange(float attSpdPlusVal)
  {
    float newVal = anim.GetFloat("AttackSpeed") + attSpdPlusVal;

    anim.SetFloat("AttackSpeed", newVal);

    if(anim.GetFloat("AttackSpeed") > 3)
      anim.SetFloat("AttackSpeed", 3);


  }

  // 아이템에 의한 골드 획득량 변경
  void ItemGoldAcqChange(float goldAcqPlusVal)
  {
    goldAcq += goldAcqPlusVal;

  }

  // 아이템에 의한 아이템 획득률 변경
  void ItemItemDropChange(float itemDropPlusVal)
  {
    itemDrop += itemDropPlusVal;

  }

  // 아이템에 의한 방어력 무시 변경
  void ItemDefIgnChange(float defIgnPlusVal)
  {
    defIgn += defIgnPlusVal;
    
    if(defIgn > 100)
      defIgn = 100;

  }
  
  // 아이템에 의한 점프 횟수 증가
  void ItemJumpCountChange(int jumpCountPlusVal)
  {
    maxJump += jumpCountPlusVal;

  }


  IEnumerator UsePotion()
  {
    // H버튼으로 포션사용
    if (Input.GetKeyDown(KeyCode.H))
    {
      // 최대체력일때는 포션 사용 불가
      if (potion > 0 && currentHp < maxHp && isPotionCoolTime == false)
      {
        isPotionCoolTime = true;
        potion--;
        currentHp += Mathf.RoundToInt(maxHp * 0.4f);  // 최대체력의 40% 회복

        audioSource.PlayOneShot(potionClip);  // 소리 재생

        if (currentHp > maxHp)
        {
          currentHp = maxHp;
        }

        HpChange?.Invoke(maxHp, currentHp);
        PotionChange.Invoke(potion);

        yield return new WaitForSeconds(potionCoolTime);
        isPotionCoolTime = false;
      }

    }
  }

  public void GetPotion()
  {
    PotionChange.Invoke(potion);
  }

  


  // 플레이어의 상태에 따른 애니메이션 실행 함수
  void PlayAnimation()
  {
    switch (state)
    {
      case PlayerState.Idle:
        {
          // 공격, 방어, 넉백 중이라면 끝나고 애니메이션 재생
          if (!(isAttack || isBlock || isKnockBack))
          {
            anim.SetInteger("State", 0);
          }

        }
        break;
      case PlayerState.Move:
        {
          anim.SetInteger("State", 1);

        }
        break;
      case PlayerState.Attack:
        {
          anim.SetInteger("State", 2);
        }
        break;
      case PlayerState.Jump:
        {
          // 공격, 방어, 넉백 중이라면 끝나고 애니메이션 재생
          if (!(isAttack || isBlock || isKnockBack))
          { anim.SetInteger("State", 3); }
        }
        break;
      case PlayerState.Fall:
        {
          // 공격, 방어, 넉백 중이라면 끝나고 애니메이션 재생
          if (!(isAttack || isBlock || isKnockBack))
          { anim.SetInteger("State", 4); }
        }
        break;
      case PlayerState.BlockIdle:
        {
          anim.SetInteger("State", 5);
        }
        break;
      case PlayerState.BlockHit:
        {
          anim.SetTrigger("Block");
          anim.SetInteger("State", 6);
        }
        break;

      case PlayerState.KnockBack:
        {
          anim.SetTrigger("KnockBack");
          anim.SetInteger("State", 7);
        }
        break;

      case PlayerState.Dead:
        {
          anim.SetTrigger("Dead");
         anim.SetInteger("State", 8);
        }
        break;

      default:
        break;
    }
  }

  

}
