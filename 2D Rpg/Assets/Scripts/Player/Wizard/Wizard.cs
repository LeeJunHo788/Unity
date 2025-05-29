using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : PlayerController
{

  float teleportDistance = 4f;   // 텔레포트 거리

  [Header("오브젝트")]
  public GameObject tpObj;
  public GameObject wizardAttackObj;
  public GameObject voidShield;     // 1번 스킬 오브젝트
  public GameObject electricLighting;   // 2번 스킬 오브젝트

  protected override void Awake()
  {
    anim = GetComponent<Animator>();    // 애니메이터 가져오기
    anim.speed = 1f;    // 애니메이션 재생 시간

    jumpForce = 11; // 점프력
    maxJump = 1;    // 최대 점프 횟수
    speed = 4.0f;   // 이동속도

    att = 6;        // 공격력
    def = 5;       // 방어력
    maxHp = 25;     // 최대 체력
    defIgn = 10;     // 방어 무시
    goldAcq = 0;    // 골드 획득량
    itemDrop = 0.1f;    // 아이템 드랍 확률
    expAcq = 0;     // 경험치 획득량
    potion = 3;     // 포션 3개



    psvSkillCooldown = 3f;
    skill1Cooldown = 40f;
    skill2Cooldown = 7f;

    base.Awake();

  }

  protected override void Start()
  {
    playerName = gameObject.name;   // 현재 객체의 이름

    // 애니메이터의 컨트롤러에서 모든 애니메이션 클립 가져오기
    RuntimeAnimatorController controller = anim.runtimeAnimatorController;
    AnimationClip[] clips = controller.animationClips;

    foreach (AnimationClip clip in clips)
    {
      // 사망 애니메이션의 길이 가져오기
      if (clip.name == playerName + "_Death")
      {
        deathAnim = clip;
        deathTime = clip.length;
      }

      // 공격 애니메이션의 길이 가져오기
      else if (clip.name == playerName + "_Attack")
      {
        attackAnim = clip;
        attackTime = clip.length;
      }

      // 피격 애니메이션의 길이 가져오기
      else if (clip.name == playerName + "_Hit")
      {
        knockBackAnim = clip;
        knockBackTime = clip.length;
      }

      // 방어 애니메이션의 길이 가져오기
      else if (clip.name == playerName + "_Block")
      {
        blockingAnim = clip;
      }
    }

    // 넉백시간
    knockBackTime = (knockBackAnim.length / (1f)) * anim.GetFloat("KnockBackSpeed");
    knockBackTime /= anim.speed;


    // 사망 시간
    deathTime = deathAnim.length / 0.5f;
    deathTime /= anim.speed;

    base.Start();
  }

  protected override void Update()
  {
    float clipSpeed = 1f; // 애니메이션 클립의 재생 속도
    float animatorSpeed = anim.speed; // 애니메이터 전체 스피드 (1)
    attackSpeed = anim.GetFloat("AttackSpeed"); // 공격 속도 값 

    attackTime = (attackAnim.length / clipSpeed) / (animatorSpeed * attackSpeed);


    base.Update();

    // StartCoroutine(Teleport());

  }


  protected override void Attack()
  {
    StartCoroutine(AttackCoroutine());  // 공격 코루틴 실행
  }


  IEnumerator AttackCoroutine()
  {
    if (Input.GetKey(KeyCode.X) && !isAttack && !isBlock && !isKnockBack)
    {
      isAttack = true;
      state = PlayerState.Attack;
      anim.SetTrigger("Attack");

      yield return new WaitForSeconds(attackTime * 0.75f); // 선딜

      Vector2 pos; // 공격이 생성될 위치
      audioSource.PlayOneShot(attackClip);  // 소리 재생


      if (transform.eulerAngles.y == 0) // 캐릭터가 오른쪽을 보고 있으면
      {
        pos = transform.position + new Vector3(0.5f,0.2f);

        Instantiate(wizardAttackObj, pos, Quaternion.Euler(0, 0, 0));
      }


      else
      {
        pos = transform.position + new Vector3(-0.5f, 0.2f);

        Instantiate(wizardAttackObj, pos, Quaternion.Euler(0, 180, 0));
      }


      yield return new WaitForSeconds(attackTime - (attackTime * 0.75f)); // 공격 시간만큼 대기(선딜에서 뺀 만큼
      
      isAttack = false;
      state = PlayerState.Idle;
    }
  }

  protected override void HandleSkillInput()
  {
    if (Input.GetKeyDown(KeyCode.Q) && !isSkill1Cooling)
    {
      // 1번 스킬 레벨 제한 7레벨
      if (level < 7)
        return;

      StartCoroutine(SkillCooldown(skill1Slider, skill1Cooldown, 1));
      StartCoroutine(Skill1Apply());
    }

    if (Input.GetKeyDown(KeyCode.W) && !isSkill2Cooling)
    {

      // 2번 스킬 레벨 제한 12
      if (level < 12 || isAttack || isKnockBack)
          return;

      StartCoroutine(SkillCooldown(skill2Slider, skill2Cooldown, 2));
      StartCoroutine(Skill2());
    }

    //  패시브 스킬
    if (Input.GetKeyDown(KeyCode.C) && !isPsvSkillCooling)
    {
      StartCoroutine(SkillCooldown(psvSkillSlider, psvSkillCooldown, 3));
      StartCoroutine(Teleport());
    }
  }

  IEnumerator Skill1Apply()
  {
    GameObject clone = Instantiate(voidShield);
    audioSource.PlayOneShot(skill1Clip);  // 소리 재생


    // 버프 전 스탯 저장
    float pretpDistance = teleportDistance;
    float preDef = def;

    // 텔레포트 거리 증가, 방어력 증가, 초당 체력회복
    teleportDistance += 2f;
    def *= 2.5f;

    // 스킬을 쓴 직후 스탯 저장
    float buffTpDistance = teleportDistance;
    float buffDef = def;

    float duration = 15f;
    float tickTime = 3f;
    float timer = 0f;

    while (timer < duration)
    {
      // [18] 체력 회복 (최대 체력의 2%)
      int healAmount = Mathf.RoundToInt(maxHp * 0.1f); // 3초마다 체력 10%회복
      currentHp = Mathf.Min(currentHp + healAmount, maxHp); // 최대 체력 초과 방지

      HpChangeMethod();

      yield return new WaitForSeconds(tickTime);
      timer += tickTime;
    }

    yield return new WaitForSeconds(15f);

    def -= buffDef - preDef;
    teleportDistance -= buffTpDistance - pretpDistance;
    Destroy(clone);

  }

  IEnumerator Skill2()
  {

    isAttack = true;
    state = PlayerState.Attack;
    anim.SetTrigger("Attack");

    yield return new WaitForSeconds(attackTime * 0.75f); // 선딜

    audioSource.PlayOneShot(skill2Clip);  // 소리 재생

    // 범위 탐색
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 5f);
    
    foreach (Collider2D hit in hits)
    {
      if (hit.CompareTag("Enemy")) // 적 탐지
      {
        EnemyController ec = hit.GetComponent<EnemyController>();
        if (ec != null)
        {
          Vector2 pos = hit.transform.position + new Vector3(0, 1.2f);

          GameObject clone = Instantiate(electricLighting, hit.transform.position, Quaternion.identity);


          // 플레이어 위치 기준으로 계산
          Vector2 knockDir = hit.transform.position - transform.position;
          ec.StartCoroutine(ec.KnockBack(att * 1.3f, knockDir));

          Destroy(clone, 0.6f);

        }
      }

      else if (hit.CompareTag("Boss")) // 적 탐지
      {
        BossController bc = hit.GetComponent<BossController>();
        if (bc != null)
        {
          Vector2 pos = hit.transform.position + new Vector3(0, 1.2f);

          GameObject clone = Instantiate(electricLighting, hit.transform.position, Quaternion.identity);


          // 플레이어 위치 기준으로 계산
          Vector2 knockDir = hit.transform.position - transform.position;
          bc.StartCoroutine(bc.KnockBack(att * 1.3f, knockDir));

          Destroy(clone, 0.6f);

        }
      }


    }
    yield return new WaitForSeconds(attackTime - (attackTime * 0.75f)); // 공격 시간만큼 대기(선딜에서 뺀 만큼

    isAttack = false;
    state = PlayerState.Idle;

  }

  IEnumerator Teleport()
  {
    if (Input.GetKeyDown(KeyCode.C) && !isKnockBack) 
    {
      Vector2 inputDir = Vector2.zero; // 입력 방향 초기화

      // 방향키 입력 처리
      if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) inputDir.x += 1;
      if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) inputDir.x -= 1;
      if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) inputDir.y += 1;
      if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) inputDir.y -= 1;

      inputDir.Normalize(); 


      Vector2 startPos = transform.position;
      Vector2 targetPos = startPos + inputDir * teleportDistance; // 목표 위치 계산

      RaycastHit2D hit = Physics2D.Raycast(startPos, inputDir, teleportDistance);

      if (hit.collider != null && hit.collider.CompareTag("Ground")) // 장애물이 있으면
      {
        // 충돌 지점 바로 앞까지 이동 
        targetPos = hit.point - inputDir * 0.1f;
      }

      transform.position = targetPos; // 

      audioSource.PlayOneShot(psvSkillClip);  // 소리 재생

      // 텔레포트 이펙트
      GameObject tpEffect = Instantiate(tpObj, transform.position, Quaternion.identity);
      Destroy(tpEffect, 0.6f);

      yield return null; 

      
    }

  }


  void LevelUp(int a)
  {

    maxHp += 3;       // 최대 체력 증가
    currentHp += 3;   // 증가치 만큼 체력 회복

    if (currentHp > maxHp)
    {
      currentHp = maxHp;
    }


    att += 1.0f;   // 공격력 증가
    def = def + def * 0.03f;   // 방어력 증가
    anim.SetFloat("AttackSpeed", anim.GetFloat("AttackSpeed") + 0.03f);

    HpChangeMethod();

    if(level == 7)
    {
      skill1LevelText.SetActive(false);
      skill1Icon.color = Color.white;
      teleportDistance += 1f;
    }

    else if(level == 12)
    {
      skill2LevelText.SetActive(false);
      skill2Icon.color = Color.white;
      teleportDistance += 1f;

    }


  }


  // 오브젝트 활성화시 호출되는 함수
  protected override void OnEnable()
  {
    base.OnEnable();
    ExpManager.LevelUp += LevelUp;       // 이벤트 구독
  }

  // 오브젝트 비활성화시 호출되는 함수
  protected override void OnDisable()
  {
    base.OnDisable();
    ExpManager.LevelUp -= LevelUp;       // 이벤트 구독 해제

  }
}
