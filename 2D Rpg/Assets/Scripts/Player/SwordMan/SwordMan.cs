using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMan : PlayerController
{
  public GameObject shield;   // 방패 오브젝트
  public GameObject swordSlash;   // 검기 이펙트


  protected override void Awake()
  {
    anim = GetComponent<Animator>();    // 애니메이터 가져오기
    anim.speed = 1.2f;    // 애니메이션 재생 시간

    jumpForce = 11; // 점프력
    maxJump = 1;    // 최대 점프 횟수
    speed = 4.0f;   // 이동속도

    att = 5;        // 공격력
    def = 20;       // 방어력
    maxHp = 30;     // 최대 체력
    defIgn = 0;     // 방어 무시
    goldAcq = 0;    // 골드 획득량
    itemDrop = 0.1f;    // 아이템 드랍 확률
    expAcq = 0;     // 경험치 획득량
    potion = 3;     // 포션 3개

    skill1Cooldown = 20f;
    skill2Cooldown = 10f;

    base.Awake();

  }

  protected override void Start()
  {
    psvSkillSlider.gameObject.SetActive(false);
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
      else if (clip.name == playerName + "_Attack1")
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
    knockBackTime = (knockBackAnim.length / (0.5f)) * anim.GetFloat("KnockBackSpeed");
    knockBackTime /= anim.speed;


    // 사망 시간
    deathTime = deathAnim.length / 0.7f;
    deathTime /= anim.speed;

    // 방패 오브젝트 가져오기
    shield = GameObject.Find("Shield");

    base.Start();
  }

  protected override void Update()
  {
    // 방패 비활성화
    if (!(state == PlayerState.BlockHit || state == PlayerState.BlockIdle))
      shield.SetActive(false);

    if (isBlock)
    {
      // 방향키를 떼었을 때 높은 마찰력 적용 (멈춤)
      box.sharedMaterial = highFrictionMaterial;
    }
    else
    {
      // 방향키를 누르면 낮은 마찰력 적용 (움직일 수 있도록)
      box.sharedMaterial = lowFrictionMaterial;
    }

    float clipSpeed = 0.65f; // 애니메이션 클립의 재생 속도
    float animatorSpeed = anim.speed; // 애니메이터 전체 스피드 (1.2)
    attackSpeed = anim.GetFloat("AttackSpeed"); // 공격 속도 값 

    attackTime = (attackAnim.length / clipSpeed) / (animatorSpeed * attackSpeed);

    comboResetTime = attackTime + (attackTime * 0.3f);

    base.Update();

    // 방어
    Block();

    // 방어 해제
    BlockEnd();
  }



  // 방어
  private void Block()
  {
    // 공격, 넉백 중이 아닐때 C를 누르면 방어 실행
    if(Input.GetKey(KeyCode.C) && !isAttack && !isKnockBack)
    {
      // 죽었거나 방어 성공시에 실행X
      if (blocking || isDead)
        return;

      state = PlayerState.BlockIdle;
      isBlock = true;

      // 방패 활성화
      shield.SetActive(true);
    }
  }

  // 방어 해제
  private void BlockEnd()
  {
    if (isBlock)
    {
      // 방어중일 때 C를 떼면 방어 해제
      if (Input.GetKeyUp(KeyCode.C))
      {
        state = PlayerState.Idle;
        isBlock = false;

        // 방패 비활성화
        shield.SetActive(false);
      }
    }
  }

  // 방어 성공
  public void BlockSucces()
  {
    StartCoroutine(Blocking());
  }

  IEnumerator Blocking()
  {
    blocking = true;
    state = PlayerState.BlockHit;
    isBlock = false;

    yield return new WaitForSeconds(blockingAnim.length);
    blocking = false;

  }

  protected override void Attack()
  {
    StartCoroutine(SwordAttackCoroutine());  // SwordMan 전용 공격 코루틴 실행
  }

  
  IEnumerator SwordAttackCoroutine()
  {
    if (Input.GetKey(KeyCode.X) && !isAttack && !isBlock && !isKnockBack)
    {
      isAttack = true;
      state = PlayerState.Attack;

      // 콤보 단계 설정
      int maxCombo = 1;

      if (level >= 10) maxCombo = 3;
      else if (level >= 5) maxCombo = 2;

      
      comboStep = 0;
      comboTimer = comboResetTime;

      while (Input.GetKey(KeyCode.X) && comboStep < maxCombo)
      {
        anim.SetTrigger("Attack" + comboStep);    // 해당 콤보 애니메이션 실행
        audioSource.PlayOneShot(attackClip);

        comboStep++;
        comboTimer = comboResetTime;

        yield return new WaitForSeconds(attackTime); // 공격 시간만큼 대기

        if (comboStep > maxCombo)
        {
          break;
        }

      }

      isAttack = false;
      state = PlayerState.Idle;
      comboStep = 0;
    }
  }

  protected override void HandleSkillInput()
  {
    if (Input.GetKeyDown(KeyCode.Q) && !isSkill1Cooling)
    {
      // 스킬 제한 7레벨
      if (level < 7)
        return;

      StartCoroutine(SkillCooldown(skill1Slider, skill1Cooldown, 1));


      StartCoroutine(Skill1Apply());
    }

    if (Input.GetKeyDown(KeyCode.W) && !isSkill2Cooling)
    {

      // 스킬 제한 12레벨
      if (level < 12 || isAttack || isKnockBack)
        return;

      StartCoroutine(SkillCooldown(skill2Slider, skill2Cooldown, 2));

      StartCoroutine(Skill2());
    }
  }

  IEnumerator Skill1Apply()
  {
    audioSource.PlayOneShot(skill1Clip);
    // 버프 전 스탯 저장
    float preAtt = att;
    float preSpeed = speed;
    float preAttSpd = anim.GetFloat("AttackSpeed");
    float preDef = def;

    // 공 1.5 이속 1.5 공속 1.2 방어력 0.8
    att *= 1.5f;
    speed *= 1.5f;
    anim.SetFloat("AttackSpeed", preAttSpd * 1.25f);
    def *= 0.8f;


    // 스킬을 쓴 직후 스탯 저장
    float buffAtt = att;
    float buffSpeed = speed;
    float buffAttSpd = anim.GetFloat("AttackSpeed");
    float buffDef = def;


    yield return new WaitForSeconds(10f);

    att -= buffAtt - preAtt;
    speed -= buffSpeed - preSpeed;
    anim.SetFloat("AttackSpeed",  anim.GetFloat("AttackSpeed") - (buffAttSpd - preAttSpd));
    def -= buffDef - preDef;

  }

  IEnumerator Skill2()
  {
    isAttack = true;
    state = PlayerState.Attack;
    anim.SetTrigger("Attack0");

    audioSource.PlayOneShot(skill2Clip);

    yield return new WaitForSeconds(0.2f);

    Vector3 pos;

    if (transform.eulerAngles.y == 0)
    {
      pos = transform.position + new Vector3(0.5f, 1.1f);

      Instantiate(swordSlash, pos, Quaternion.Euler(0, 0, 0));

    }

    else
    {
      pos = transform.position + new Vector3(-0.5f, 1.1f);

      Instantiate(swordSlash, pos, Quaternion.Euler(0, 180, 0));

      Debug.Log(transform.eulerAngles.y);

    }

    yield return new WaitForSeconds(attackTime - 0.2f); // 공격 시간만큼 대기
    isAttack = false;
    state = PlayerState.Idle;
  }


  void LevelUp(int a)
  {

    maxHp += 7;       // 최대 체력 증가
    currentHp += 7;   // 증가치 만큼 체력 회복

    if (currentHp > maxHp)
    {
      currentHp = maxHp;
    }

    if (level == 7)
    {
      skill1LevelText.SetActive(false);
      skill1Icon.color = Color.white;
    }

    else if (level == 12)
    {
      skill2LevelText.SetActive(false);
      skill2Icon.color = Color.white;

    }

    att += 1.2f;   // 공격력 증가
    anim.SetFloat("AttackSpeed", anim.GetFloat("AttackSpeed") + 0.03f);
    def = def + def * 0.05f;   // 방어력 증가

    HpChangeMethod();
    

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
