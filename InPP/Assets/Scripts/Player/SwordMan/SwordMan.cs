using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMan : PlayerController
{
  public GameObject shield;   // ���� ������Ʈ
  public GameObject swordSlash;   // �˱� ����Ʈ


  protected override void Awake()
  {
    anim = GetComponent<Animator>();    // �ִϸ����� ��������
    anim.speed = 1.2f;    // �ִϸ��̼� ��� �ð�

    jumpForce = 11; // ������
    maxJump = 1;    // �ִ� ���� Ƚ��
    speed = 4.0f;   // �̵��ӵ�

    att = 5;        // ���ݷ�
    def = 20;       // ����
    maxHp = 30;     // �ִ� ü��
    defIgn = 0;     // ��� ����
    goldAcq = 0;    // ��� ȹ�淮
    itemDrop = 0.1f;    // ������ ��� Ȯ��
    expAcq = 0;     // ����ġ ȹ�淮
    potion = 3;     // ���� 3��

    skill1Cooldown = 20f;
    skill2Cooldown = 10f;

    base.Awake();

  }

  protected override void Start()
  {
    psvSkillSlider.gameObject.SetActive(false);
    playerName = gameObject.name;   // ���� ��ü�� �̸�

    // �ִϸ������� ��Ʈ�ѷ����� ��� �ִϸ��̼� Ŭ�� ��������
    RuntimeAnimatorController controller = anim.runtimeAnimatorController;
    AnimationClip[] clips = controller.animationClips;

    foreach (AnimationClip clip in clips)
    {
      // ��� �ִϸ��̼��� ���� ��������
      if (clip.name == playerName + "_Death")
      {
        deathAnim = clip;
        deathTime = clip.length;
      }

      // ���� �ִϸ��̼��� ���� ��������
      else if (clip.name == playerName + "_Attack1")
      {
        attackAnim = clip;
        attackTime = clip.length;
      }

      // �ǰ� �ִϸ��̼��� ���� ��������
      else if (clip.name == playerName + "_Hit")
      {
        knockBackAnim = clip;
        knockBackTime = clip.length;
      }

      // ��� �ִϸ��̼��� ���� ��������
      else if (clip.name == playerName + "_Block")
      {
        blockingAnim = clip;
      }
    }

    // �˹�ð�
    knockBackTime = (knockBackAnim.length / (0.5f)) * anim.GetFloat("KnockBackSpeed");
    knockBackTime /= anim.speed;


    // ��� �ð�
    deathTime = deathAnim.length / 0.7f;
    deathTime /= anim.speed;

    // ���� ������Ʈ ��������
    shield = GameObject.Find("Shield");

    base.Start();
  }

  protected override void Update()
  {
    // ���� ��Ȱ��ȭ
    if (!(state == PlayerState.BlockHit || state == PlayerState.BlockIdle))
      shield.SetActive(false);

    if (isBlock)
    {
      // ����Ű�� ������ �� ���� ������ ���� (����)
      box.sharedMaterial = highFrictionMaterial;
    }
    else
    {
      // ����Ű�� ������ ���� ������ ���� (������ �� �ֵ���)
      box.sharedMaterial = lowFrictionMaterial;
    }

    float clipSpeed = 0.65f; // �ִϸ��̼� Ŭ���� ��� �ӵ�
    float animatorSpeed = anim.speed; // �ִϸ����� ��ü ���ǵ� (1.2)
    attackSpeed = anim.GetFloat("AttackSpeed"); // ���� �ӵ� �� 

    attackTime = (attackAnim.length / clipSpeed) / (animatorSpeed * attackSpeed);

    comboResetTime = attackTime + (attackTime * 0.3f);

    base.Update();

    // ���
    Block();

    // ��� ����
    BlockEnd();
  }



  // ���
  private void Block()
  {
    // ����, �˹� ���� �ƴҶ� C�� ������ ��� ����
    if(Input.GetKey(KeyCode.C) && !isAttack && !isKnockBack)
    {
      // �׾��ų� ��� �����ÿ� ����X
      if (blocking || isDead)
        return;

      state = PlayerState.BlockIdle;
      isBlock = true;

      // ���� Ȱ��ȭ
      shield.SetActive(true);
    }
  }

  // ��� ����
  private void BlockEnd()
  {
    if (isBlock)
    {
      // ������� �� C�� ���� ��� ����
      if (Input.GetKeyUp(KeyCode.C))
      {
        state = PlayerState.Idle;
        isBlock = false;

        // ���� ��Ȱ��ȭ
        shield.SetActive(false);
      }
    }
  }

  // ��� ����
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
    StartCoroutine(SwordAttackCoroutine());  // SwordMan ���� ���� �ڷ�ƾ ����
  }

  
  IEnumerator SwordAttackCoroutine()
  {
    if (Input.GetKey(KeyCode.X) && !isAttack && !isBlock && !isKnockBack)
    {
      isAttack = true;
      state = PlayerState.Attack;

      // �޺� �ܰ� ����
      int maxCombo = 1;

      if (level >= 10) maxCombo = 3;
      else if (level >= 5) maxCombo = 2;

      
      comboStep = 0;
      comboTimer = comboResetTime;

      while (Input.GetKey(KeyCode.X) && comboStep <= maxCombo)
      {
        anim.SetTrigger("Attack" + comboStep);    // �ش� �޺� �ִϸ��̼� ����
        comboStep++;
        comboTimer = comboResetTime;

        yield return new WaitForSeconds(attackTime); // ���� �ð���ŭ ���

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
      // ��ų ���� 7����
      if (level < 7)
        return;

      StartCoroutine(SkillCooldown(skill1Slider, skill1Cooldown, 1));


      StartCoroutine(Skill1Apply());
    }

    if (Input.GetKeyDown(KeyCode.W) && !isSkill2Cooling)
    {

      // ��ų ���� 15����
      if (level < 15 || isAttack || isKnockBack)
        return;

      StartCoroutine(SkillCooldown(skill2Slider, skill2Cooldown, 2));

      StartCoroutine(Skill2());
    }
  }

  IEnumerator Skill1Apply()
  {
    audioSource.PlayOneShot(skill1Clip);
    // ���� �� ���� ����
    float preAtt = att;
    float preSpeed = speed;
    float preAttSpd = attackSpeed;
    float preDef = def;

    // �� 1.5 �̼� 1.5 ���� 1.2 ���� 0.8
    att *= 1.5f;
    speed *= 1.5f;
    attackSpeed *= 1.2f;
    def *= 0.8f;


    // ��ų�� �� ���� ���� ����
    float buffAtt = att;
    float buffSpeed = speed;
    float buffAttSpd = attackSpeed;
    float buffDef = def;


    yield return new WaitForSeconds(10f);

    att -= buffAtt - preAtt;
    speed -= buffSpeed - preSpeed;
    attackSpeed -= buffAttSpd - preAttSpd;
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

    yield return new WaitForSeconds(attackTime - 0.2f); // ���� �ð���ŭ ���
    isAttack = false;
    state = PlayerState.Idle;
  }


  void LevelUp(int a)
  {

    maxHp += (level * 5);       // �ִ� ü�� ����
    currentHp += (level * 5);   // ����ġ ��ŭ ü�� ȸ��

    if (currentHp > maxHp)
    {
      currentHp = maxHp;
    }

    att += 0.7f;   // ���ݷ� ����
    def = def + def * 0.05f;   // ���� ����

    HpChangeMethod();
    

  }


  // ������Ʈ Ȱ��ȭ�� ȣ��Ǵ� �Լ�
  protected override void OnEnable()
  {
    base.OnEnable();
    ExpManager.LevelUp += LevelUp;       // �̺�Ʈ ����
  }

  // ������Ʈ ��Ȱ��ȭ�� ȣ��Ǵ� �Լ�
  protected override void OnDisable()
  {
    base.OnDisable();
    ExpManager.LevelUp -= LevelUp;       // �̺�Ʈ ���� ����

  }
}
