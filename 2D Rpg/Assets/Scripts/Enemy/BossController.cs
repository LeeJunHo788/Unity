using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
  protected Rigidbody2D rb;
  protected BoxCollider2D box;
  protected AudioSource audioSource;
  protected Vector2 dir;            // 이동방향

  public bool isDefaultLeft = false;
  protected bool onGround = false;       // 땅에 닿았는지 여부
  protected bool isUpgrade = false;

  public float returnDistance = 5f; //  복귀 거리
  private bool isReturning = false; // 복귀 중인지 확인

  protected enum EnemyState { Idle, Move, Attack, KnockBack, Dead }   // 적의 상태
  protected EnemyState state;   // 상태 변수


  protected Animator anim;  // 적 애니메이터
  protected float deathTime;    // 적 사망 애니메이션의 길이(애니메이션의 이름을 반드시 "이름_Death"로 할 것)
  public bool isDead = false;   // 적 사망 상태

  protected float range;  // 탐색 범위
  protected float speed;   // 이동속도
  protected Collider2D[] cols;   // 탐색한 콜라이더


  protected float attackRange; // 공격 범위
  public bool isAttack;     // 공격 중(공격 대기시간을 위한 변수)
  protected bool isAttacking;  // 공격 모션 실행중
  protected float accumulatedDamage = 0f;   // 넉백 처리를 위한 누적 데미지

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

  public GameObject[] dropItemPool;   // 처치시 드랍하는 아이템 

  public bool isKnockBack = false;    // 넉백 상태 여부


  protected GameObject player;    // 플레이어 객체
  protected PlayerController pc;  // 플레이어 컨트롤러 스크립트
  protected float playerAtt;    // 플레이어 객체의 공격력
  protected float playerDefIgn; // 플레이어 객체의 방어력 무시

  protected GameObject tp; // 텔레포터 객체
  protected TeleporterManager tpm; // 텔레포터 객체

  public GameObject demTextObj;   // 데미지 텍스트 오브젝트
  public AudioClip hitClip;



  protected virtual void Awake()
  {

  }

  protected virtual void Start()
  {

    rb = GetComponent<Rigidbody2D>();
    box = GetComponent<BoxCollider2D>();
    audioSource = GetComponent<AudioSource>();

    player = GameObject.FindGameObjectWithTag("Player");    // 플레이어 객체 가져오기
    pc = player.GetComponent<PlayerController>();
    playerAtt = pc.att;   // 플레이어의 공격력 가져오기
    playerDefIgn = pc.defIgn;   // 플레이어의 방어력 무시 가져오기


    tp = GameObject.FindGameObjectWithTag("Teleporter");     // 텔레포터 객체 가져오기
    tpm = tp.GetComponent<TeleporterManager>();

    spriteRenderer = GetComponent<SpriteRenderer>();
    defaultMat = spriteRenderer.material;


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


    PlayAnimation();


    if (isKnockBack)
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

        // 객체 삭제
        Invoke("Death", deathTime + 1f);
        return;
      }

      if (isReturning)
      {
        Vector2 returnDir = ((Vector2)tp.transform.position - (Vector2)transform.position).normalized;

        // 보스가 텔레포터 위치에 거의 도착했다면
        if (Vector2.Distance(transform.position, tp.transform.position) < 1.6f)
        {
          isReturning = false;
          rb.velocity = Vector2.zero;
          state = EnemyState.Idle;

          currentHp += Mathf.RoundToInt(hp * 0.1f);
          if(currentHp > hp)
          {
            currentHp = hp;
          }
          

          return;
        }

        // 텔레포터 방향으로 이동
        rb.velocity = returnDir * speed;

        // 회전
        if (returnDir.x > 0)
        {
          transform.rotation = isDefaultLeft ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
          hpSlider.transform.localRotation = isDefaultLeft ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        }
        else if (returnDir.x < 0)
        {
          transform.rotation = isDefaultLeft ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
          hpSlider.transform.localRotation = isDefaultLeft ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        }

        state = EnemyState.Move;
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
          Vector2 targetPos = col.transform.position; // 쫓아올 플레이어의 위치
          dir = (targetPos - (Vector2)transform.position).normalized;     // 방향 설정
          float distanceToPlayer = Vector2.Distance(transform.position, targetPos); // 플레이어와의 거리 계산

          // 플레이어가 공격 범위 안에 있고, 공격 중이 아니라면 공격 실행
          if (distanceToPlayer <= attackRange && !isAttack && !isDead)
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
            // y축 차이가 일정 범위 이상이면서 플레이어보다 위에 있는 경우
            if (yDiff > 0.2f && transform.position.y - targetPos.y > 0)
            {
              // 진행방향 유지
              return;
            }

            rb.velocity = Vector2.zero; // 적 멈추기
            state = EnemyState.Idle; // 대기 상태
            return;
          }

          // 플레이어보다 위에 있으면서 경사면이 아닌곳에서 이동 중인 경우
          if (yDiff > 3.0f && transform.position.y - targetPos.y > 0 && rb.velocity.x != 0)
          {
            // 진행방향 유지
            return;
          }

          // 회전 설정
          if (dir.x > 0)
          {
            // 오른쪽으로 이동 중일 때
            transform.rotation = isDefaultLeft ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            dir.x = 1;
            if (dir.y > 0) dir.y = 0;

            // 슬라이더는 회전효과 제거
            hpSlider.transform.localRotation = isDefaultLeft ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
          }

          else if (dir.x < 0)
          {
            // 왼쪽으로 이동 중일 때
            transform.rotation = isDefaultLeft ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            dir.x = -1;
            if (dir.y > 0) dir.y = 0;

            // 슬라이더는 회전효과 제거
            hpSlider.transform.localRotation = isDefaultLeft ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

          }

          if (!CanMove())
          {
            rb.velocity = Vector2.zero;
            state = EnemyState.Idle;
            return;
          }

          UpdateConstraints();

          // 플레이어가 공격 범위 안에 있거나 공격 중이라면 이동 무효
          if (distanceToPlayer <= attackRange || isAttack)
          {
            state = EnemyState.Idle;
            return;
          }

          if (!isDead && !isKnockBack && Vector2.Distance(transform.position, tp.transform.position) > returnDistance)
          {
            isReturning = true;
          }

          // 속도 적용
          // rb.velocity = dir * speed;

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
    // 부딪힌 물체가 공격이라면
    if (col.gameObject.CompareTag("Attack"))
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
    if (isUpgrade)
    {
      state = EnemyState.Idle;
      return;
    }

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

    
    ExpManager.instance.AddExp(exp);
    GoldManager.instance.GetGold(gold);

    DropItem();

  }


  public IEnumerator KnockBack(float dem, Vector2 dir)
  {
    // 넉백중이라면 무시
    if(isKnockBack)
      yield break;

    audioSource.PlayOneShot(hitClip, 0.4f);   // 소리 재생

    // 체력 감소 계산
    float damage = dem - (dem * (def - (def * (playerDefIgn * 0.01f))) * 0.01f);
    int intDamage = Mathf.RoundToInt(damage);
    currentHp -= intDamage;

    ShowDamageText(intDamage);

    // 체력 바 반영
    hpSlider.value = currentHp;

    // 누적 데미지 더하기
    accumulatedDamage += intDamage;

    // 공격 중이거나 누적 데미지가 낮으면 피격 애니메이션 재생 무시
    if (isAttacking || accumulatedDamage < hp * 0.1f)
      yield break;

    state = EnemyState.KnockBack;
    isKnockBack = true;

    accumulatedDamage = 0f;

    Golem golem = this as Golem;  // 현재 객체를 Golem으로 변환 시도
    if (golem != null && (golem.phase == 1 || golem.phase == 3))
    {
      // 피격 애니메이션만 재생
      yield return new WaitForSeconds(0.7f);
      isKnockBack = false;
      state = EnemyState.Idle;
      yield break;
    }


      // 넉백 효과
      rb.velocity = Vector2.zero;
    // 공격이 왼쪽으로부터 왔을 때
    if (dir.x > 0)
    {
      rb.AddForce(new Vector2(2, 1) * 1.2f, ForceMode2D.Impulse);
    }

    // 공격이 오른쪽으로부터 왔을 때
    else if (dir.x < 0)
    {
      rb.AddForce(new Vector2(-2, 1) * 1.2f, ForceMode2D.Impulse);
    }

    StartCoroutine(HitFlash());

    // 넉백이 끝날때 까지 대기
    yield return new WaitForSeconds(0.4f);


    isKnockBack = false;
    state = EnemyState.Idle;

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

  protected virtual bool CanMove()
  {
    return true; // 기본값은 이동 허용
  }

  // 넉백당하거나 이동중을 제외하고는 전부 이동 불가
  protected virtual void UpdateConstraints()
  {
    if (state == EnemyState.Idle || state == EnemyState.Attack)
    {
      // 이동 금지
      rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
    }
    else
    {
      // 이동 허용 (Freeze 제거)
      rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
    }
  }

  void ShowDamageText(int damage)
  {

    // 적 위에 생성
    Vector3 spawnPos = transform.position + Vector3.up * 0.3f;

    GameObject textObj = Instantiate(demTextObj, spawnPos, Quaternion.identity);
    DemText damageTextScript = textObj.GetComponent<DemText>();
    damageTextScript.SetText(damage);
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
