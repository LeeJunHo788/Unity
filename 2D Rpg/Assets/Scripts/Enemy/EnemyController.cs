using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
  protected Rigidbody2D rb;
  protected BoxCollider2D box;
  protected LayerMask groundLayer;
  protected AudioSource audioSource;
  Vector2 dir;            // 이동방향

  public bool onGround = false;       // 땅에 닿았는지 여부
  public bool onSlope;             // 경사면 여부
  protected float angle;              // 경사면과의 각도

  protected enum EnemyState { Idle, Move, Attack, KnockBack, Dead }   // 적의 상태
  protected EnemyState state;   // 상태 변수


  protected Animator anim;  // 적 애니메이터
  protected float deathTime;    // 적 사망 애니메이션의 길이(애니메이션의 이름을 반드시 "이름_Death"로 할 것)
  protected float knockBackTime;    // 적 넉백 애니메이션의 길이(애니메이션의 이름을 반드시 "이름_Hit"로 할 것)
  protected float attackTime;   // 공격 애니메이션의 클립 길이
  public bool isDead = false;   // 적 사망 상태
 

  protected float range;  // 탐색 범위
  protected float speed;   // 이동속도
  protected Collider2D[] cols;   // 탐색한 콜라이더

  protected bool canMove = true;        // false면 부동형
  protected bool isFlyingEnemy = false; // true면 공중 적, false면 지상 적
  protected bool canAttack = false;     // 공격 가능 몬스터 체크용 변수, 기본값은 false
  protected float attackRange; // 공격 범위
  public bool isAttack = false;     // 공격 중
  public bool isAttacking = false;  // 공격 모션 진행 중

  public bool isRecentKnockback = false;        // 돌진중 넉백 당한 여부
  protected bool isSturn = false;
  public bool isKnockBack = false;    // 넉백 상태 여부

  protected bool isAttackMovementAllowed = false; // 공격 중 움직임이 가능한 적 확인 기본값은 false

  protected Coroutine dashCoroutine; // 현재 돌진 중인 코루틴을 저장
  protected bool isDashing = false;

  
  protected Material defaultMat;     // 원래 머티리얼
  public Material hitFlashMat;    // 깜빡일 때 사용할 흰색 머티리얼
  private SpriteRenderer spriteRenderer;

  [Header("스탯")]
  public string mobName;   // 적 이름
  public float att;    // 적 공격력
  public float def;    // 적 방어력
  public float defIgn;  // 적 방어력무시
  public int hp;   // 적 최대체력
  public int currentHp;    // 적 현재 체력
  public Slider hpSlider;    // 체력 슬라이더
  public int exp;      // 적 처치시 경험치
  public int gold;      // 적 처치시 골드
  public float spawnWeight = 10f;  // 스폰 확률 기본 10

  [Header("드랍 아이템")]
  public GameObject[] dropItemPool;   // 처치시 드랍하는 아이템 



  protected GameObject player;    // 플레이어 객체
  protected PlayerController pc;  // 플레이어 컨트롤러 스크립트
  protected float playerAtt;    // 플레이어 객체의 공격력
  protected float playerDefIgn; // 플레이어 객체의 방어력 무시

  protected GameObject tp; // 텔레포터 객체
  protected TeleporterManager tpm; // 텔레포터 객체

  public GameObject demTextObj;   // 데미지 텍스트 오브젝트
  public AudioClip hitClip;

  public static event Action<int> OnEnemyDeath;   // 이벤트 선언

  protected virtual void Awake()
  {

  }

  protected virtual void Start()
  {
    rb = GetComponent<Rigidbody2D>();    // 리지드 바디
    box = GetComponent<BoxCollider2D>();  // 박스 콜라이더 가져오기
    groundLayer = LayerMask.GetMask("Ground"); // Ground 레이어만 감지하게 설정
    audioSource = GetComponent<AudioSource>();    // 오디오 소스 가져오기

    player = GameObject.FindGameObjectWithTag("Player");    // 플레이어 객체 가져오기
    pc = player.GetComponent<PlayerController>();
    playerAtt = pc.att;   // 플레이어의 공격력 가져오기
    playerDefIgn = pc.defIgn;   // 플레이어의 방어력 무시 가져오기

    
    spriteRenderer = GetComponent<SpriteRenderer>();    // 스프라이트 랜더러 가져오기
    defaultMat = spriteRenderer.material;               // 원래 마테리얼 저장


    tp = GameObject.FindGameObjectWithTag("Teleporter");     // 텔레포터 객체 가져오기
    tpm = tp.GetComponent<TeleporterManager>();


    mobName = this.gameObject.name;   // 현재 객체의 이름 가져오기
    hpSlider = gameObject.GetComponentInChildren<Slider>();   // 현재 객체의 체력 슬라이더 가져오기


    // 초기 체력, 체력바 설정
    currentHp = hp;
    hpSlider.maxValue = hp;
    hpSlider.value = currentHp;

    // 적의 애니메이터
    anim = GetComponent<Animator>();


    // 애니메이터의 컨트롤러에서 모든 애니메이션 클립 가져오기
    RuntimeAnimatorController controller = anim.runtimeAnimatorController;
    AnimationClip[] clips = controller.animationClips;

    foreach (AnimationClip clip in clips)
    {
      // 사망 애니메이션의 길이 가져오기
      if (clip.name == mobName + "_Death")
      {
        deathTime = clip.length;
        break;
      }

    }


   

    state = EnemyState.Idle;

  }

  protected virtual void Update()
  {
    if (anim == null)
    {
      anim = GetComponent<Animator>();
      return;
    }

    if(isRecentKnockback && !isSturn)
    {
      StartCoroutine(Sturn());
      return;
    }


    if (isAttack)
    {
      if (!isAttackMovementAllowed)
        rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
    }

    else
    {
      rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
      rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    if (!canMove)   // 부동형 적이면 위치 고정
    {
      rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
      rb.constraints |= RigidbodyConstraints2D.FreezePositionY;

    }

    if (isFlyingEnemy)
    {
      FlyToPlayer(); // 공중 적 전용 이동 함수
      return;
    }


    PlayAnimation();


    if (isKnockBack || isAttack)
      return;



    else
    {
      // 만약 현재체력이 0보다 낮아지면
      if (currentHp <= 0)
      {
        if (isDead == true)
          return;

        // 움직임을 멈추고 사망 애니메이션 재생
        rb.velocity = Vector2.zero;
        state = EnemyState.Dead;

        isDead = true;

        // 레이어를 변경하여 판정 제거
        gameObject.layer = LayerMask.NameToLayer("Dead");

        // 객체 비활성화 (나중에 삭제로 바꿀 것)
        Invoke("Death", deathTime);
        return;
      }

      // 적을 중심으로 일정 범위 내에 있는 콜라이더
      cols = Physics2D.OverlapCircleAll(transform.position, range);

      // 감지된 콜라이더 검사 실행
      foreach (Collider2D col in cols)
      {
        // 콜라이더의 태그가 Player라면
        if (col != null && col.CompareTag("Player"))
        {

          Vector2 targetPos = col.transform.position;   // 쫓아올 플레이어의 위치
          dir = (targetPos - (Vector2)transform.position).normalized; // 방향 설정
          float distanceToPlayer = Vector2.Distance(transform.position, targetPos); // 플레이어와의 거리 계산

          // 공격 가능한 적이 플레이어가 공격 범위 안에 있고, 공격 중이 아니라면 공격 실행
          if (distanceToPlayer <= attackRange && !isAttack && !isDead && canAttack)
          {
            TryAttack();
            return;
          }

          // x축, y축 차이
          float xDiff = Mathf.Abs(targetPos.x - transform.position.x);
          float yDiff = Mathf.Abs(targetPos.y - transform.position.y);

          // 적이 떨어지고 있는 경우
          if (!onGround)
          {
            // 아래로만 이동
            dir = new Vector2(0, -1);
            return;
          }


          // x축 차이가 일정 범위 이하라면
          else if (xDiff < 0.2f)
          {

            // 플레이어보다 아래에 있는 경우
            if (yDiff < 0.2f && transform.position.y - targetPos.y > 0)
            {
              rb.velocity = Vector2.zero; // 적 멈추기
              state = EnemyState.Idle; // 대기 상태
              return;
            }

            
          }

         
          // 플레이어보다 위에 있으면서 경사면이 아닌곳에서 이동 중인 경우
          if (yDiff > 1.0f && transform.position.y - targetPos.y > 0 && rb.velocity.x != 0 && !onSlope)
          {
            // 양 옆으로 레이캐스트를 쏘기 위한 방향 벡터
            Vector2 left = Vector2.left;
            Vector2 right = Vector2.right;
            float rayDistance = 3f; // 감지 거리 설정

            Debug.DrawRay(transform.position, left * rayDistance, Color.red);   // 왼쪽 레이
            Debug.DrawRay(transform.position, right * rayDistance, Color.red);  // 오른쪽 레이

            // 레이캐스트로 벽 감지
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, rayDistance, groundLayer);
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, rayDistance, groundLayer);


            bool wallLeft = hitLeft.collider != null && Mathf.Abs(Vector2.Angle(hitLeft.normal, Vector2.right)) < 10f;
            bool wallRight = hitRight.collider != null && Mathf.Abs(Vector2.Angle(hitRight.normal, Vector2.left)) < 10f;

            if (wallLeft && !wallRight)
            {
              // 왼쪽에 벽이 있고 오른쪽은 열려 있음 → 오른쪽으로 이동
              dir = Vector2.right;
            }
            else if (!wallLeft && wallRight)
            {
              // 오른쪽에 벽이 있고 왼쪽은 열려 있음 → 왼쪽으로 이동
              dir = Vector2.left;
            }
            else if ((!wallLeft && !wallRight) || (wallLeft && wallRight))
            {
              // 양쪽 다 벽이 있거나 없으면 오른쪽 으로 이동
              dir = (targetPos - (Vector2)transform.position).normalized;
              dir.y = 0; // 수평 이동만

              // 플레이어가 오른쪽에 있음
              if (targetPos.x > transform.position.x)
              {
                if(xDiff < 0.1f)
                {
                  dir.x = 1;
                  return;
                }

                dir.x = 1;

              }
              else
              {
                if (xDiff < 0.1f)
                {
                  dir.x = -1;
                  return;
                }
                dir.x = -1;
              }

            }

          }

          // 회전 설정
          if (dir.x > 0)
          {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            dir.x = 1;
            if (dir.y > 0)
            {
              dir.y = 0;
            }

            // 슬라이더는 회전효과 제거
            hpSlider.transform.localRotation = Quaternion.Euler(0, 0, 0);
          }

          else if (dir.x < 0)
          {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            dir.x = -1;
            if (dir.y > 0)
            {
              dir.y = 0;
            }

            // 슬라이더는 회전효과 제거
            hpSlider.transform.localRotation = Quaternion.Euler(0, 180, 0);

          }
          

          // 상태변경
          state = EnemyState.Move;

          return;

        }

      }

      // 플레이어가 범위에서 벗어나면 정지
      rb.velocity = Vector2.zero;


      // 상태 변환
      state = EnemyState.Idle;

    }

  }


  private void OnCollisionEnter2D(Collision2D col)
  {

    // 부딪힌 물체가 방패이거나 플레이어라면
    if (col.gameObject.CompareTag("Player") || (col.gameObject.CompareTag("Shield")))
    {

      // 부딪힌 방향
      Vector2 hitDir = (transform.position - player.transform.position);

      // 넉백 실행, 데미지는 0
      StartCoroutine(KnockBack(0, hitDir));
    }

    // 부딪힌 물체가 공격이라면
    else if (col.gameObject.CompareTag("Attack"))
    {
      // 부딪힌 방향
      Vector2 hitDir = (transform.position - player.transform.position);

      // 공격의 이름(배율때문에 필요)
      string attackName = col.gameObject.name;

      // 넉백 실행, 데미지 적용
      StartCoroutine(KnockBack(playerAtt * PlayerPrefs.GetFloat(attackName), hitDir));

    }


  }

  private void OnCollisionStay2D(Collision2D col)
  {
    // 부딪힌 물체가 땅이라면
    if (col.gameObject.CompareTag("Ground"))
    {
      foreach (ContactPoint2D contact in col.contacts)
      {
        Vector2 normal = contact.normal;

        // 충돌한 표면이 위쪽을 향할 경우 (법선 벡터의 y값이 일정 이상이면 바닥으로 간주)
        if (contact.normal.y > 0.5f)
        {
          onGround = true;
        }

        if (isKnockBack || currentHp <= 0) return; // 넉백 중이거나 사망시 이동 중단

        Vector2 slopeDir = new Vector2(-normal.y, normal.x).normalized; // 시계 방향으로 90도 회전

        // 적이 바라보는 방향에 따라 방향 보정
        if (transform.rotation.eulerAngles.y == 180)
        {
          slopeDir = -slopeDir;
        }

        float slopeX = Mathf.Abs(slopeDir.x);

        // 만약 x값이 너무 작으면 이동 생략 (0에 가까움)
        if (slopeX < 0.01f)
          return;

        float projectedSpeed = Vector2.Dot(dir, slopeDir);
        float ratio = speed / slopeX;


        Vector2 moveDir = slopeDir * projectedSpeed * ratio;

        // 속도 적용
        rb.velocity = moveDir;
        
      }

    }
  }



  void OnCollisionExit2D(Collision2D col)
  {
    // 떨어진 물체가 땅이라면
    if (col.gameObject.CompareTag("Ground"))
    {
      // 넉백 중이 아닐 때
      if (!isKnockBack)
      {
        onGround = false;
      }
    }
  }

  // 사망
  void Death()
  {
    // 객체 삭제 
    Destroy(gameObject);

    // 텔레포터가 준비중이 아닐 때만 경험치, 골드, 아이템 획득
    if (tpm.isPreparing == false)
    {
      ExpManager.instance.AddExp(exp);
      GoldManager.instance.GetGold(gold);

      DropItem();

    }

    OnEnemyDeath?.Invoke(hp);
  }


  public IEnumerator KnockBack(float dem, Vector2 dir)
  {

    // 돌진 중에 피격 시 돌진 강제 종료
    if (isDashing)
    {
      if (dashCoroutine != null)
      {
        StopCoroutine(dashCoroutine);
        dashCoroutine = null;
      }
      isDashing = false;
      isAttacking = false;
      isAttack = false;
      rb.velocity = Vector2.zero;

      isRecentKnockback = true;
    }

    audioSource.PlayOneShot(hitClip, 0.4f);  // 소리 재생

    int totalDem = Mathf.RoundToInt(dem - (dem * (def - (def * (playerDefIgn * 0.01f))) * 0.01f));

    // 체력 감소
    currentHp -= totalDem;

    // 체력 바 반영
    hpSlider.value = currentHp;

    ShowDamageText(totalDem);

    // 공격 중이거나 사망하거나 부동형이면 넉백 효과 취소
    if (isAttack || currentHp <= 0 || !canMove)
    { yield break; }


    state = EnemyState.KnockBack;
    isKnockBack = true;

    // 넉백 효과
    rb.velocity = Vector2.zero;
    // 공격이 왼쪽으로부터 왔을 때
    if (dir.x > 0)
    {
      // 공중 적이라면 옆으로만 밀림
      if (isFlyingEnemy)
        rb.AddForce(new Vector2(2, 0) * 1.2f, ForceMode2D.Impulse);
      else
        rb.AddForce(new Vector2(2, 1) * 1.2f, ForceMode2D.Impulse);
    }

    // 공격이 오른쪽으로부터 왔을 때
    else if (dir.x < 0)
    {
      // 공중 적이라면 옆으로만 밀림
      if (isFlyingEnemy)
        rb.AddForce(new Vector2(-2, 0) * 1.2f, ForceMode2D.Impulse);
      else
        rb.AddForce(new Vector2(-2, 1) * 1.2f, ForceMode2D.Impulse);
    }


    StartCoroutine(HitFlash());   // 피격시 흰색으로 깜빡이는 효과

    // 넉백이 끝날때 까지 대기
    yield return new WaitForSeconds(0.5f);


    isKnockBack = false;
    state = EnemyState.Idle;

    if(isAttack)
    {
      rb.velocity = Vector2.zero;
      yield return new WaitForSeconds(1);
      isAttack = false;
    }

  }

  // 피격 시각 효과
  IEnumerator HitFlash()
  {
    spriteRenderer.material = hitFlashMat;  // 머테리얼 변경
    yield return new WaitForSeconds(0.15f); // 잠깐 기다렸다가
    spriteRenderer.material = defaultMat; // 원래 색으로 복원
  }

  public void ForceDeath()
  {
    if (!isDead)  // 이미 죽은 적은 다시 죽이지 않음
    {
      currentHp = 0;

    }
  }

  void DropItem()
  {
    float rnd = UnityEngine.Random.value;

    // 아이템 드랍 확률 결정 
    if (rnd > pc.itemDrop)
    {
      return;
    }


    // 아이템별 확률 계산을 위해 총 가중치 계산
    float totalWeight = 0f;

    foreach (var obj in dropItemPool)
    {
      ItemManager item = obj.GetComponent<ItemManager>();

      if (item != null && item.itemData != null)
      {
        totalWeight += item.itemData.weight;
      }
    }


    // 랜덤 값을 뽑고 그에 해당하는 아이템 선택
    float randomValue = UnityEngine.Random.Range(0, totalWeight);
    float current = 0f;

    foreach (var obj in dropItemPool)
    {
      ItemManager item = obj.GetComponent<ItemManager>();
      if (item == null || item.itemData == null)
        continue;

      current += item.itemData.weight;
      if (randomValue <= current)
      {
        Instantiate(obj, transform.position, Quaternion.identity);
        break;
      }
    }

  }

  protected virtual void TryAttack()
  {
    if (isAttack || isDead || player == null)
      return;

    StartCoroutine(PerformAttack());
  }

  protected virtual IEnumerator PerformAttack()
  {
    // 넉백 중이라면 공격 불가
    if (isKnockBack)
      yield break;

    isAttack = true;
    isAttacking = true;
    rb.velocity = Vector2.zero;
    state = EnemyState.Attack;
    

    // 자식 클래스에서 공격 구현
    yield return null;

    yield return new WaitForSeconds(1.5f); // 공격 후 쿨타임
    isAttacking = false;

    yield return new WaitForSeconds(0.5f); // 공격 후 쿨타임
    isAttack = false;
    state = EnemyState.Idle;
  }

  // 공중 몹 이동 메서드
  protected virtual void FlyToPlayer()
  {
    if (player == null)
      return;

    

    PlayAnimation();

    // 만약 현재체력이 0보다 낮아지면
    if (currentHp <= 0)
    {
      if (isDead == true)
        return;

      // 움직임을 멈추고 사망 애니메이션 재생
      rb.velocity = Vector2.zero;
      state = EnemyState.Dead;

      isDead = true;

      // 레이어를 변경하여 판정 제거
      gameObject.layer = LayerMask.NameToLayer("Dead");

      // 객체 비활성화 (나중에 삭제로 바꿀 것)
      Invoke("Death", deathTime);
      return;
    }

    // 공격중이거나 피격중이면 이동 취소
    if (isKnockBack)
      return;

   

    float distance = Vector2.Distance(transform.position, player.transform.position);

    // 공격 가능한 적이 플레이어가 공격 범위 안에 있고, 공격 중이 아니라면 공격 실행
    if (distance <= attackRange && !isAttack && !isDead && canAttack)
    {
      TryAttack();
      return;
    }

    // 범위 내 플레이어 탐지
    if (distance <= range)
    {
      Vector2 dir = ((player.transform.position - transform.position) + new Vector3(0, 0.6f)).normalized;

      // 회전 방향 설정
      if (dir.x > 0)
      {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        hpSlider.transform.localRotation = Quaternion.Euler(0, 0, 0);
      }
      else
      {
        transform.rotation = Quaternion.Euler(0, 180, 0);
        hpSlider.transform.localRotation = Quaternion.Euler(0, 180, 0);
      }


      if (isAttack)
        return;

     
      // 속도 적용
      rb.velocity = dir * speed;

      
      

     if(!isAttack && !isAttacking)
      state = EnemyState.Move;

      
    }
    else
    {
      
      rb.velocity = Vector2.zero;
      state = EnemyState.Idle;
    }

  }

  // 돌진 공격 메서드
  protected virtual IEnumerator DashAttack(Vector2 direction, float dashPower, float chargeTime, float duration)
  {
    isDashing = true;

    rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
    rb.velocity = Vector2.zero;
    yield return new WaitForSeconds(chargeTime); // 돌진 전 준비 시간

    rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
    rb.AddForce(direction.normalized * dashPower, ForceMode2D.Impulse);

    yield return new WaitForSeconds(duration); // 돌진 유지 시간

    rb.velocity = Vector2.zero;
    isDashing = false;
  }

  protected IEnumerator Stay()
  {
    yield return new WaitForSeconds(1f);
  }

  void ShowDamageText(int damage)
  {

    // 적 위에 생성
    Vector3 spawnPos = transform.position + Vector3.up * 0.3f;

    GameObject textObj = Instantiate(demTextObj, spawnPos, Quaternion.identity);
    DemText damageTextScript = textObj.GetComponent<DemText>();
    damageTextScript.SetText(damage);
  }

  public void TakeDamage(float damage)
  {
    if (isDead) return;

    int totalDamage = Mathf.RoundToInt(damage - (damage * (def - (def * (playerDefIgn * 0.01f))) * 0.01f));

    // 체력 감소
    currentHp -= totalDamage;

    // 체력바 반영
    hpSlider.value = currentHp;

    // 데미지 텍스트 출력
    ShowDamageText(totalDamage);

    // 피격 시 깜빡이기
    StartCoroutine(HitFlash());

  }

  IEnumerator Sturn()
  {
    isSturn = true;
    yield return new WaitForSeconds(0.5f);

    state = EnemyState.Idle;
    rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
    rb.constraints |= RigidbodyConstraints2D.FreezePositionY;


    yield return new WaitForSeconds(1.0f);

    rb.constraints &= RigidbodyConstraints2D.FreezePositionX;
    rb.constraints &= RigidbodyConstraints2D.FreezePositionY;
    rb.constraints = RigidbodyConstraints2D.FreezeRotation;


    isSturn = false;
    isRecentKnockback = false;

  }


  void PlayAnimation()
  {
    switch (state)
    {
      case EnemyState.Idle:
        {
          anim.SetInteger("State", 0);
        }
        break;

      case EnemyState.Move:
        {
          anim.SetInteger("State", 1);
        }
        break;

      case EnemyState.Attack:
        {
          anim.SetInteger("State", 2);
        }
        break;

      case EnemyState.KnockBack:
        {
          anim.SetInteger("State", 3);
        }
        break;
      case EnemyState.Dead:
        {
          anim.SetInteger("State", 4);
          rb.gravityScale = 0;
        }
        break;
      default:
        break;
    }
  }

}
