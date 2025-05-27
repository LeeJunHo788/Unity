using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
  protected Rigidbody2D rb;    // ������ �ٵ�
  protected BoxCollider2D box;    // �ڽ� �ݶ��̴�
  protected AudioSource audioSource;    // ȿ���� ����� ����� �ҽ�

  Vector2 rayStart;               // ���̰� �����ϴ� ����
  float rayLength;       // ���� ����
  Vector2 rightBottom;    // �ڽ� �ݶ��̴��� ���� �ϴ��� ���� ����
  Vector2 leftBottom;     // �ڽ� �ݶ��̴��� ���� �ϴ��� ���� ����


  protected enum PlayerState { Idle, Move, Attack, Jump, Fall, BlockIdle, BlockHit, KnockBack, Dead } // �÷��̾��� ����
  protected PlayerState state;     // ���� ����

  protected float Xvel = 0;   // ���� �̵� �ӵ�
  protected float Yvel = 0;   // ���� �̵� �ӵ�
  protected float jumpForce;    // ������

  protected bool onSlope;             // ���� ����
  protected float angle;              // ������� ����
  protected float comboTimer = 0f;    // �޺� �ð� üũ

  public bool onGround = false;   // �ٴڿ� ��Ҵ��� ����
  protected Animator anim;    // �ִϸ�����

  protected int jumpCount = 0;    // ���� ���� Ƚ��
  protected int maxJump;   // �ִ� ���� Ƚ��

  public int comboStep = 0;   // ���� �޺� �ܰ�
  public float comboResetTime;   // �޺� �ʱ�ȭ �ð�
  public bool isAttack = false;   // ���� ������ ����

  public bool isBlock = false;    // ��� ������ ����
  public bool blocking = false;    // ��� ���� ����


  protected AnimationClip attackAnim;    // ���� �ִϸ��̼�
  public float attackTime;    // ���� �ִϸ��̼��� ����
  public float attackSpeed;

  protected AnimationClip blockingAnim;    // ��� �ִϸ��̼�



  protected AnimationClip knockBackAnim;   // �˹� �ִϸ��̼�
  protected float knockBackTime;    // �÷��̾��� �˹� �ð�
  protected float knockBackForce = 4.0f;    // �˹� ��
  public bool isKnockBack = false;    // �˹� �� ����
  protected bool isInvincible = false;          // ���� �� ����
  float invincibleTime = 1.0f;        // �����ð�

  public bool inShop = false;         // ���� ���� ����
  public bool isPaused = false;       // �Ͻ����� ����


  protected AnimationClip deathAnim;
  protected float deathTime;
  protected bool isDead = false;   // ��� ���� Ȯ��

  [Header("����")]
  public float speed;   // ���� �̵� �ӷ�
  public float att;    // �÷��̾� ���ݷ�
  public float def;    // �÷��̾� ����
  public int maxHp;   // �÷��̾� �ִ� ü��
  public int currentHp;    // �÷��̾� ���� ü��
  public float defIgn;  // �÷��̾� ���� ����   
  public float goldAcq; // �÷��̾� ��� ȹ�淮
  public float itemDrop; // �÷��̾� ������ ȹ�� Ȯ��
  public float expAcq;  // ����ġ ȹ�淮
  public int level;    // �÷��̾� ����
  public float potionCoolTime;         // ���� ��Ÿ��


  public int potion;    // ���� ����
  public string playerName;   // �÷��̾� ĳ������ �̸�

  public bool isPotionCoolTime = false; // ���� ��Ÿ�� �����Ȳ


  protected Material defaultMat;     // ���� ��Ƽ����
  protected SpriteRenderer spriteRenderer;
  public Material hitFlashMat;      // �ǰݽ� ���׸���


  public GameObject gameOverCanvas;     // ���� ���� ĵ����


  public PhysicsMaterial2D lowFrictionMaterial;  // �̵� ������ �� ����� ���� ��Ƽ����
  public PhysicsMaterial2D highFrictionMaterial; // ���� �� ����� ���� ��Ƽ����

  [Header("��ų UI")]
  public Slider psvSkillSlider;
  public Image psvSkillIcon;


  public Slider skill1Slider;
  public Image skill1Icon;
  public GameObject skill1LevelText;

  public Slider skill2Slider;
  public Image skill2Icon;
  public GameObject skill2LevelText;

  [Header("ȿ����")]
  public AudioClip attackClip;      // �⺻ ����
  public AudioClip psvSkillClip;    // �нú� ��ų
  public AudioClip skill1Clip;      // 1�� ��ų
  public AudioClip skill2Clip;      // 2�� ��ų
  public AudioClip potionClip;      // ���� ���
  public AudioClip hitClip;         // �ǰ�


  protected bool isPsvSkillCooling = false;
  protected bool isSkill1Cooling = false;
  protected bool isSkill2Cooling = false;

  protected float psvSkillCooldown;   // ��ú� ��ų ��Ÿ��
  protected float skill1Cooldown;   // Q ��ų ��Ÿ��
  protected float skill2Cooldown;   // W ��ų ��Ÿ��

  private static PlayerController instance;

  public static event Action<int, int> HpChange; // ü�� ���� �̺�Ʈ
  public static event Action<int> PotionChange;  // ���� ��� �̺�Ʈ


  protected virtual void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject); // �� ���� �� ������Ʈ ����
    }
    else
    {
      Destroy(gameObject); // �ߺ� ����
    }

    level = 1;    // �ʱ� ����
  }


  protected virtual void Start()
  {
    rb = GetComponent<Rigidbody2D>();   // ������ٵ� �ҷ�����
    box = GetComponent<BoxCollider2D>();  // �ڽ� �ݶ��̴� ��������
    audioSource = GetComponent<AudioSource>();  // ����� �ҽ� ��������

    rayStart = rightBottom;
    rayLength = 0.4f;   // ������ ���̼���

    currentHp = maxHp; // �ʱ� ü��


    HpChange?.Invoke(maxHp, currentHp);    // ü�� ������Ʈ
    PotionChange?.Invoke(potion);           // ���� ������Ʈ

    potionCoolTime = 10f;                  // ���� ��Ÿ��

    state = PlayerState.Idle;


    if (skill1Slider != null) skill1Slider.value = 1f;
    if (skill2Slider != null) skill2Slider.value = 1f;

    skill1Slider.gameObject.SetActive(false);
    skill2Slider.gameObject.SetActive(false);

    skill1Icon.color = Color.gray;
    skill2Icon.color = Color.gray;


    spriteRenderer = GetComponent<SpriteRenderer>();
    defaultMat = spriteRenderer.material;    // ���� ���׸��� ����
    



  }

  protected virtual void Update()
  {
    if (inShop || isPaused) return;

    // �ִϸ��̼� ����
    PlayAnimation();

    // ���� �������
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

    // ���� ����ü���� 0���� ��������
    if (currentHp <= 0)
    {
      if (isDead)
        return;

      // �������� ���߰� ��� �ִϸ��̼� ���
      rb.gravityScale = 0;
      rb.linearVelocity = Vector2.zero;
      state = PlayerState.Dead;


      // ���̾ �����Ͽ� ���� ����
      gameObject.layer = LayerMask.NameToLayer("Dead");

      // ��ü ��Ȱ��ȭ (���߿� ������ �ٲ� ��)
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

    // �̵� ����
    // �˹����̸� �̵� ���� X
    if (isKnockBack || blocking) return;

    else
    {
      // �̵� ó�� �Լ�
      Move();

      // ���� ó�� �Լ�
      Jump();

      // �� ����
      onGround = IsOnGround();

      if (!onGround)
      {
        box.sharedMaterial = lowFrictionMaterial; // ������ ���� �׻� ���� ������ ����
      }


      // ���� �پ� ���� �� ���ݽ� ������ ������ ����
      if ((isAttack || isBlock) && onGround)
      {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
      }

      else
      {
        rb.linearVelocity = new Vector2(Xvel, rb.linearVelocity.y);
      }

    }

    // �ӵ����� 0�̸� �⺻ ����
    if (rb.linearVelocity == new Vector2(0, 0) && state != PlayerState.BlockIdle && state != PlayerState.Attack)
    {
      state = PlayerState.Idle;
    }

    // ����
    Attack();

    if (comboStep > 0)
    {
      comboTimer -= Time.deltaTime;

      if (comboTimer <= 0)
      {
        // �����ð��� ������ �޺� �ʱ�ȭ
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

    // �ڽ� �ݶ��̴� ���� �Ǵ� ������ �Ʒ����� �Ʒ� �������� Ray�� ��
    RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, rayLength, groundLayer);

    if (hit.collider != null) // �ٴڰ� �浹���� ���
    {
      angle = Vector2.Angle(hit.normal, Vector2.up); // �ٴڰ� ����(Up) ������ ���� ���

      if (angle > 5f) // ������ 5�� �̻��̸� ���� ó��  
      {
        onSlope = true;
      }

      else
      {
        onSlope = false;
      }
    }

    // ����, ��� ���̸� �̵� �Ұ�
    if (!(isAttack || isBlock))
    {
      // ����Ű�� ������ �̵�
      float moveInput = 0;

      if (Input.GetKey(KeyCode.LeftArrow))
      {
        moveInput = -1;   // ���� �̵�
      }

      else if (Input.GetKey(KeyCode.RightArrow))
      {
        moveInput = 1;    // ������ �̵�
      }

      // ���� ���� �ƴ϶�� ĳ���� ȸ�� (���� �߿��� ȸ�� �Ұ�)
      if (moveInput == -1)
      {
        transform.rotation = Quaternion.Euler(0, 180, 0);
        rayStart = leftBottom; // ���̰� ���۵Ǵ� ������ ȸ��
      }

      else if (moveInput == 1)
      {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rayStart = rightBottom; // ���̰� ���۵Ǵ� ������ ȸ��
      }


      // ���� ����
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


      else if (onSlope && onGround) // ���� ���� �� ���� ��
      {
        if (moveInput == 0)
        {
          // ����Ű�� ������ �� ���� ������ ���� (����)
          box.sharedMaterial = highFrictionMaterial;
        }
        else
        {
          // ����Ű�� ������ ���� ������ ���� (������ �� �ֵ���)
          box.sharedMaterial = lowFrictionMaterial;
        }
      }
      else
      {
        // ���������� �⺻������ ���� ������ ����
        box.sharedMaterial = lowFrictionMaterial;
      }

      // X�� �̵��� = ���� * �ӵ�
      Xvel = moveInput * speed;
      Yvel = rb.linearVelocity.y;

      rb.linearVelocity = new Vector2(Xvel, Yvel);

    }
  }

  private bool IsOnGround()
  {
    LayerMask groundLayer = 1 << 6;

    // ������ �Ʒ����� ���� �߻�
    RaycastHit2D rightHit = Physics2D.Raycast(rightBottom, Vector2.down, rayLength, groundLayer);
    Debug.DrawRay(rightBottom, Vector2.down * rayLength, Color.green); // ����׿� ����

    // ���� �Ʒ����� ���� �߻�
    RaycastHit2D leftHit = Physics2D.Raycast(leftBottom, Vector2.down, rayLength, groundLayer);
    Debug.DrawRay(leftBottom, Vector2.down * rayLength, Color.blue); // ����׿� ����

    if (rightHit.collider != null || leftHit.collider != null)

    if ((rightHit.collider != null || leftHit.collider != null) && rb.linearVelocity.y <= 5f) // �ٴڰ� �浹���� ���
    {

      jumpCount = 0;
      return true;

    }

    return false; // �ٴ��� �������� ������ false
  }


  private void Jump()
  {
    // �˹� �߿� ���� �Ұ�
    if (isKnockBack) return;

    // ���� �پ������鼭 �����̽��� ������ ����
    // �������̸� ���� �Ұ�
    if (jumpCount < maxJump && Input.GetKeyDown(KeyCode.Space) && !isAttack)
    {
      rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
      rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
      jumpCount++;

    }

    // ���� ó�� (Jump or Fall)
    // ĳ������ Y�� �̵��ӵ��� ������ = �ö󰡰� ������ ��������
    if (rb.linearVelocity.y > 0 && onGround == false
        && state != PlayerState.Attack)   // ���� �Է½� ���� ���� �̹ݿ�
    {
      state = PlayerState.Jump;
    }

    else if (rb.linearVelocity.y <= 0 && onGround == false
             && state != PlayerState.Attack)
    {
      state = PlayerState.Fall;
    }


  }

  // ���� ������ �ڽ� ��ũ��Ʈ����
  protected virtual void Attack()
  {
    
  }


  // ���� �ǰ�
  public void TakeDem(float dem, float defIgn, Vector2 hitDir)
  {
    // �����̶�� �ǰ� ����
    if (isInvincible) return;

    StartCoroutine(KnockBack(dem, defIgn, hitDir));
    StartCoroutine(InvincibleOn());

  }

  // �����ǰ�
  private void Hit(Collision2D collision)
  {
    // �÷��̾�� �ε��� ��ü�� ���̶��
    if (collision.gameObject.CompareTag("Enemy"))
    {
      // �����̶�� �ǰ� ����
      if (isInvincible) return;

      // ���� ��ũ��Ʈ ��������
      EnemyController ec = collision.gameObject.GetComponent<EnemyController>();

      // ���� ���ݷ� ��������
      float dem = ec.att;
      float defIgn = ec.defIgn;

      // ���� �÷��̾ ������ ����
      Vector2 hitDir = (transform.position - collision.transform.position);

      // ���� ��� ���°� �ƴ϶�� �˹� ó��
      if (!ec.isDead)
      {
        StartCoroutine(KnockBack(dem, defIgn, hitDir));
        StartCoroutine(InvincibleOn());
      }
    }

    // �÷��̾�� �ε��� ��ü�� �����̶��
    else if (collision.gameObject.CompareTag("EnemyAttack") || collision.gameObject.CompareTag("EnemyBullet"))
    {
      // �����̶�� �ǰ� ����
      if (isInvincible) return;

      // ���� ��ũ��Ʈ ��������
      EnemyAttackController eatc = collision.gameObject.GetComponent<EnemyAttackController>();

      // ���� ���ݷ� ��������
      float dem = eatc.att;
      float defIgn = eatc.defIgn;

      // ���� �÷��̾ ������ ����
      Vector2 hitDir = (transform.position - collision.transform.position);

      StartCoroutine(KnockBack(dem, defIgn, hitDir));
      StartCoroutine(InvincibleOn());
    }

    // �÷��̾�� �ε��� ��ü�� ���̶��
    else if (collision.gameObject.CompareTag("Boss"))
    {
      // �����̶�� �ǰ� ����
      if (isInvincible) return;

      // ���� ��ũ��Ʈ ��������
      BossController bc = collision.gameObject.GetComponent<BossController>();

      // ���� ���ݷ� ��������
      float dem = bc.att;
      float defIgn = bc.defIgn;

      // ���� �÷��̾ ������ ����
      Vector2 hitDir = (transform.position - collision.transform.position);

      // ���� ��� ���°� �ƴ϶�� �˹� ó��
      if (!bc.isDead)
      {
        StartCoroutine(KnockBack(dem, defIgn, hitDir));
        StartCoroutine(InvincibleOn());
      }
    }
  }

  // �˹�
  IEnumerator KnockBack(float dem, float defIgn, Vector2 dir)
  {
    state = PlayerState.KnockBack;
    isBlock = false;
    isKnockBack = true;

    // ü�� ����
    currentHp -= Mathf.RoundToInt(dem - (dem * (def - (def * (defIgn * 0.01f))) * 0.01f));
    audioSource.PlayOneShot(hitClip);  // �Ҹ� ���

    if (currentHp < 0)
    {
      currentHp = 0;
    }



    HpChange?.Invoke(maxHp, currentHp);

    // �˹� ȿ��
    rb.linearVelocity = new Vector2(0, 0);

    // ������ �������κ��� ���� ��
    if (dir.x > 0)
    {
      rb.AddForce(new Vector2(1, 1) * knockBackForce, ForceMode2D.Impulse);
    }

    // ������ ���������κ��� ���� ��
    else if (dir.x < 0)
    {
      rb.AddForce(new Vector2(-1, 1) * knockBackForce, ForceMode2D.Impulse);
    }

    StartCoroutine(HitFlash());

    // �˹��� ������ ���� ���
    yield return new WaitForSeconds(knockBackTime);


    state = PlayerState.Idle;
    isKnockBack = false;

  }

  // �̺�Ʈ ȣ���� ���� �޼���
  protected void HpChangeMethod()
  {
    HpChange.Invoke(maxHp, currentHp);
  }

  // �����ð�
  IEnumerator InvincibleOn()
  {
    isInvincible = true;

    // �����ð� �� ���� ���� 
    yield return new WaitForSeconds(invincibleTime);
    isInvincible = false;
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    // ���̶�� �ǰ�
    Hit(collision);

    // ���̶��
    if (collision.gameObject.CompareTag("Ground"))
    {
      // ����ī��Ʈ �ʱ�ȭ
      if (onGround)
      {
        jumpCount = 0;
      }
    }


  }

  // �ǰ� �ð� ȿ��
  IEnumerator HitFlash()
  {
    spriteRenderer.material = hitFlashMat;  // ���׸��� ����
    yield return new WaitForSeconds(0.15f); // ��� ��ٷȴٰ�
    spriteRenderer.material = defaultMat; // ���� ������ ����
  }

  void OnCollisionStay2D(Collision2D collision)
  {
    Hit(collision);
  }



  void Death()
  {
    // ��ü ��Ȱ��ȭ
    gameObject.SetActive(false);

    GameObject goCanvas = Instantiate(gameOverCanvas);

    Time.timeScale = 0f;  // ���� ����

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


  // ������Ʈ Ȱ��ȭ�� ȣ��Ǵ� �Լ�
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

  // ������Ʈ ��Ȱ��ȭ�� ȣ��Ǵ� �Լ�
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

  // �����ۿ� ���� ü�� ����
  void ItemHpChange(float maxHpMulVal, float maxHpPlusVal, float currentHpMulVal, float currentHpPlusVal)
  {
    maxHp = Mathf.RoundToInt((maxHp * maxHpMulVal) + maxHpPlusVal);

    currentHp = Mathf.RoundToInt((currentHp * currentHpMulVal) + currentHpPlusVal);


    if (currentHp > maxHp)
    {
      currentHp = maxHp;
    }

    // UI���� ����
    HpChange?.Invoke(maxHp, currentHp);
  }


  // �����ۿ� ���� ���ݷ� ����
  void ItemAttChange(float attMulVal, float attPlusVal)
  {
    att = (attMulVal * att) + attPlusVal;

  }

  // �����ۿ� ���� ���� ����
  void ItemDefChange(float defMulVal, float defPlusVal)
  {
    def = (defMulVal * def) + defPlusVal;
    if (def > 99.9f)
      def = 99.9f;
  }

  // �����ۿ� ���� �̵��ӵ� ����
  void ItemSpdChange(float spdMulVal, float spdPlusVal)
  {
    speed = (spdMulVal * speed) + spdPlusVal;

    if (speed > 10)
      speed = 10;

  }

  // �����ۿ� ���� ���ݼӵ� ����
  void ItemAttSpdChange(float attSpdPlusVal)
  {
    float newVal = anim.GetFloat("AttackSpeed") + attSpdPlusVal;

    anim.SetFloat("AttackSpeed", newVal);

    if(anim.GetFloat("AttackSpeed") > 2)
      anim.SetFloat("AttackSpeed", 2);


  }

  // �����ۿ� ���� ��� ȹ�淮 ����
  void ItemGoldAcqChange(float goldAcqPlusVal)
  {
    goldAcq += goldAcqPlusVal;

  }

  // �����ۿ� ���� ������ ȹ��� ����
  void ItemItemDropChange(float itemDropPlusVal)
  {
    itemDrop += itemDropPlusVal;

  }

  // �����ۿ� ���� ���� ���� ����
  void ItemDefIgnChange(float defIgnPlusVal)
  {
    defIgn += defIgnPlusVal;
    
    if(defIgn > 100)
      defIgn = 100;

  }
  
  // �����ۿ� ���� ���� Ƚ�� ����
  void ItemJumpCountChange(int jumpCountPlusVal)
  {
    maxJump += jumpCountPlusVal;

  }


  IEnumerator UsePotion()
  {
    // H��ư���� ���ǻ��
    if (Input.GetKeyDown(KeyCode.H))
    {
      // �ִ�ü���϶��� ���� ��� �Ұ�
      if (potion > 0 && currentHp < maxHp && isPotionCoolTime == false)
      {
        isPotionCoolTime = true;
        potion--;
        currentHp += Mathf.RoundToInt(maxHp * 0.4f);  // �ִ�ü���� 40% ȸ��

        audioSource.PlayOneShot(potionClip);  // �Ҹ� ���

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

  


  // �÷��̾��� ���¿� ���� �ִϸ��̼� ���� �Լ�
  void PlayAnimation()
  {
    switch (state)
    {
      case PlayerState.Idle:
        {
          // ����, ���, �˹� ���̶�� ������ �ִϸ��̼� ���
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
          // ����, ���, �˹� ���̶�� ������ �ִϸ��̼� ���
          if (!(isAttack || isBlock || isKnockBack))
          { anim.SetInteger("State", 3); }
        }
        break;
      case PlayerState.Fall:
        {
          // ����, ���, �˹� ���̶�� ������ �ִϸ��̼� ���
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
