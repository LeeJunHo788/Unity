using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Wizard : PlayerController
{

  float teleportDistance = 3f;   // �ڷ���Ʈ �Ÿ�

  [Header("������Ʈ")]
  public GameObject tpObj;
  public GameObject wizardAttackObj;
  public GameObject voidShield;     // 1�� ��ų ������Ʈ
  public GameObject electricLighting;   // 2�� ��ų ������Ʈ

  protected override void Awake()
  {
    anim = GetComponent<Animator>();    // �ִϸ����� ��������
    anim.speed = 1f;    // �ִϸ��̼� ��� �ð�

    jumpForce = 11; // ������
    maxJump = 1;    // �ִ� ���� Ƚ��
    speed = 4.0f;   // �̵��ӵ�

    att = 6;        // ���ݷ�
    def = 5;       // ����
    maxHp = 25;     // �ִ� ü��
    defIgn = 10;     // ��� ����
    goldAcq = 0;    // ��� ȹ�淮
    itemDrop = 0.1f;    // ������ ��� Ȯ��
    expAcq = 0;     // ����ġ ȹ�淮
    potion = 3;     // ���� 3��



    psvSkillCooldown = 3f;
    skill1Cooldown = 40f;
    skill2Cooldown = 7f;

    base.Awake();

  }

  protected override void Start()
  {
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
      else if (clip.name == playerName + "_Attack")
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
    knockBackTime = (knockBackAnim.length / (1f)) * anim.GetFloat("KnockBackSpeed");
    knockBackTime /= anim.speed;


    // ��� �ð�
    deathTime = deathAnim.length / 0.5f;
    deathTime /= anim.speed;

    base.Start();
  }

  protected override void Update()
  {
    float clipSpeed = 1f; // �ִϸ��̼� Ŭ���� ��� �ӵ�
    float animatorSpeed = anim.speed; // �ִϸ����� ��ü ���ǵ� (1)
    attackSpeed = anim.GetFloat("AttackSpeed"); // ���� �ӵ� �� 

    attackTime = (attackAnim.length / clipSpeed) / (animatorSpeed * attackSpeed);


    base.Update();

    // StartCoroutine(Teleport());

  }


  protected override void Attack()
  {
    StartCoroutine(AttackCoroutine());  // ���� �ڷ�ƾ ����
  }


  IEnumerator AttackCoroutine()
  {
    if (Input.GetKey(KeyCode.X) && !isAttack && !isBlock && !isKnockBack)
    {
      isAttack = true;
      state = PlayerState.Attack;
      anim.SetTrigger("Attack");

      yield return new WaitForSeconds(0.6f); // ����

      Vector2 pos; // ������ ������ ��ġ
      audioSource.PlayOneShot(attackClip);  // �Ҹ� ���


      if (transform.eulerAngles.y == 0) // ĳ���Ͱ� �������� ���� ������
      {
        pos = transform.position + new Vector3(0.5f,0.2f);

        Instantiate(wizardAttackObj, pos, Quaternion.Euler(0, 0, 0));

      }


      else
      {
        pos = transform.position + new Vector3(-0.5f, 0.2f);

        Instantiate(wizardAttackObj, pos, Quaternion.Euler(0, 180, 0));


      }


      yield return new WaitForSeconds(attackTime -0.6f); // ���� �ð���ŭ ���(�������� �� ��ŭ
      
      isAttack = false;
      state = PlayerState.Idle;
    }
  }

  protected override void HandleSkillInput()
  {
    if (Input.GetKeyDown(KeyCode.Q) && !isSkill1Cooling)
    {
      // 1�� ��ų ���� ���� 7����
      if (level < 7)
        return;

      StartCoroutine(SkillCooldown(skill1Slider, skill1Cooldown, 1));
      StartCoroutine(Skill1Apply());
    }

    if (Input.GetKeyDown(KeyCode.W) && !isSkill2Cooling)
    {

      // 2�� ��ų ���� ���� 15
      if (level < 15 || isAttack || isKnockBack)
          return;

      StartCoroutine(SkillCooldown(skill2Slider, skill2Cooldown, 2));
      StartCoroutine(Skill2());
    }

    //  �нú� ��ų
    if (Input.GetKeyDown(KeyCode.C) && !isPsvSkillCooling)
    {
      StartCoroutine(SkillCooldown(psvSkillSlider, psvSkillCooldown, 3));
      StartCoroutine(Teleport());
    }
  }

  IEnumerator Skill1Apply()
  {
    GameObject clone = Instantiate(voidShield);
    audioSource.PlayOneShot(skill1Clip);  // �Ҹ� ���


    // ���� �� ���� ����
    float pretpDistance = teleportDistance;
    float preDef = def;

    // �ڷ���Ʈ �Ÿ� ����, ���� ����, �ʴ� ü��ȸ��
    teleportDistance += 2f;
    def *= 2.5f;

    // ��ų�� �� ���� ���� ����
    float buffTpDistance = teleportDistance;
    float buffDef = def;

    float duration = 15f;
    float tickTime = 3f;
    float timer = 0f;

    while (timer < duration)
    {
      // [18] ü�� ȸ�� (�ִ� ü���� 2%)
      int healAmount = Mathf.RoundToInt(maxHp * 0.1f); // 3�ʸ��� ü�� 10%ȸ��
      currentHp = Mathf.Min(currentHp + healAmount, maxHp); // �ִ� ü�� �ʰ� ����

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

    yield return new WaitForSeconds(0.6f); // ����

    audioSource.PlayOneShot(skill2Clip);  // �Ҹ� ���

    // ���� Ž��
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 5f);
    
    foreach (Collider2D hit in hits)
    {
      if (hit.CompareTag("Enemy")) // �� Ž��
      {
        EnemyController ec = hit.GetComponent<EnemyController>();
        if (ec != null)
        {
          Vector2 pos = hit.transform.position + new Vector3(0, 1.2f);

          GameObject clone = Instantiate(electricLighting, hit.transform.position, Quaternion.identity);


          // �÷��̾� ��ġ �������� ���
          Vector2 knockDir = hit.transform.position - transform.position;
          ec.StartCoroutine(ec.KnockBack(att * 1.3f, knockDir));

          Destroy(clone, 0.6f);

        }
      }

      else if (hit.CompareTag("Boss")) // �� Ž��
      {
        BossController bc = hit.GetComponent<BossController>();
        if (bc != null)
        {
          Vector2 pos = hit.transform.position + new Vector3(0, 1.2f);

          GameObject clone = Instantiate(electricLighting, hit.transform.position, Quaternion.identity);


          // �÷��̾� ��ġ �������� ���
          Vector2 knockDir = hit.transform.position - transform.position;
          bc.StartCoroutine(bc.KnockBack(att * 1.3f, knockDir));

          Destroy(clone, 0.6f);

        }
      }


    }
    yield return new WaitForSeconds(attackTime - 0.6f); // ���� �ð���ŭ ���(�������� �� ��ŭ

    isAttack = false;
    state = PlayerState.Idle;

  }

  IEnumerator Teleport()
  {
    if (Input.GetKeyDown(KeyCode.C) && !isKnockBack) 
    {
      Vector2 inputDir = Vector2.zero; // �Է� ���� �ʱ�ȭ

      // ����Ű �Է� ó��
      if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) inputDir.x += 1;
      if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) inputDir.x -= 1;
      if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) inputDir.y += 1;
      if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) inputDir.y -= 1;

      inputDir.Normalize(); 


      Vector2 startPos = transform.position;
      Vector2 targetPos = startPos + inputDir * teleportDistance; // ��ǥ ��ġ ���

      RaycastHit2D hit = Physics2D.Raycast(startPos, inputDir, teleportDistance);

      if (hit.collider != null && hit.collider.CompareTag("Ground")) // ��ֹ��� ������
      {
        // �浹 ���� �ٷ� �ձ��� �̵� 
        targetPos = hit.point - inputDir * 0.1f;
      }

      transform.position = targetPos; // 

      audioSource.PlayOneShot(psvSkillClip);  // �Ҹ� ���

      // �ڷ���Ʈ ����Ʈ
      GameObject tpEffect = Instantiate(tpObj, transform.position, Quaternion.identity);
      Destroy(tpEffect, 0.6f);

      yield return null; 

      
    }

  }


  void LevelUp(int a)
  {

    maxHp += (level * 3);       // �ִ� ü�� ����
    currentHp += (level * 3);   // ����ġ ��ŭ ü�� ȸ��

    if (currentHp > maxHp)
    {
      currentHp = maxHp;
    }



    att += 1.0f;   // ���ݷ� ����
    def = def + def * 0.03f;   // ���� ����

    HpChangeMethod();

    if(level == 5)
    {
      skill1LevelText.SetActive(false);
      teleportDistance += 1f;
    }

    else if(level == 15)
    {
      skill2LevelText.SetActive(false);
      teleportDistance += 1f;

    }


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
