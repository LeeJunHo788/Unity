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
  Vector2 dir;            // �̵�����

  public bool onGround = false;       // ���� ��Ҵ��� ����
  public bool onSlope;             // ���� ����
  protected float angle;              // ������� ����

  protected enum EnemyState { Idle, Move, Attack, KnockBack, Dead }   // ���� ����
  protected EnemyState state;   // ���� ����


  protected Animator anim;  // �� �ִϸ�����
  protected float deathTime;    // �� ��� �ִϸ��̼��� ����(�ִϸ��̼��� �̸��� �ݵ�� "�̸�_Death"�� �� ��)
  protected float knockBackTime;    // �� �˹� �ִϸ��̼��� ����(�ִϸ��̼��� �̸��� �ݵ�� "�̸�_Hit"�� �� ��)
  protected float attackTime;   // ���� �ִϸ��̼��� Ŭ�� ����
  public bool isDead = false;   // �� ��� ����
 

  protected float range;  // Ž�� ����
  protected float speed;   // �̵��ӵ�
  protected Collider2D[] cols;   // Ž���� �ݶ��̴�

  protected bool canMove = true;        // false�� �ε���
  protected bool isFlyingEnemy = false; // true�� ���� ��, false�� ���� ��
  protected bool canAttack = false;     // ���� ���� ���� üũ�� ����, �⺻���� false
  protected float attackRange; // ���� ����
  protected bool isAttack = false;     // ���� ��
  protected bool isAttacking = false;  // ���� ��� ���� ��

  public bool isRecentKnockback = false;        // ������ �˹� ���� ����
  protected bool isSturn = false;
  public bool isKnockBack = false;    // �˹� ���� ����

  protected bool isAttackMovementAllowed = false; // ���� �� �������� ������ �� Ȯ�� �⺻���� false

  protected Coroutine dashCoroutine; // ���� ���� ���� �ڷ�ƾ�� ����
  protected bool isDashing = false;

  
  protected Material defaultMat;     // ���� ��Ƽ����
  public Material hitFlashMat;    // ������ �� ����� ��� ��Ƽ����
  private SpriteRenderer spriteRenderer;

  [Header("����")]
  public string mobName;   // �� �̸�
  public float att;    // �� ���ݷ�
  public float def;    // �� ����
  public float defIgn;  // �� ���¹���
  public int hp;   // �� �ִ�ü��
  public int currentHp;    // �� ���� ü��
  public Slider hpSlider;    // ü�� �����̴�
  public int exp;      // �� óġ�� ����ġ
  public int gold;      // �� óġ�� ���
  public float spawnWeight = 10f;  // ���� Ȯ�� �⺻ 10

  [Header("��� ������")]
  public GameObject[] dropItemPool;   // óġ�� ����ϴ� ������ 



  protected GameObject player;    // �÷��̾� ��ü
  protected PlayerController pc;  // �÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ
  protected float playerAtt;    // �÷��̾� ��ü�� ���ݷ�
  protected float playerDefIgn; // �÷��̾� ��ü�� ���� ����

  protected GameObject tp; // �ڷ����� ��ü
  protected TeleporterManager tpm; // �ڷ����� ��ü

  public GameObject demTextObj;   // ������ �ؽ�Ʈ ������Ʈ
  public AudioClip hitClip;

  public static event Action<int> OnEnemyDeath;   // �̺�Ʈ ����

  protected virtual void Awake()
  {

  }

  protected virtual void Start()
  {
    rb = GetComponent<Rigidbody2D>();    // ������ �ٵ�
    box = GetComponent<BoxCollider2D>();  // �ڽ� �ݶ��̴� ��������
    groundLayer = LayerMask.GetMask("Ground"); // Ground ���̾ �����ϰ� ����
    audioSource = GetComponent<AudioSource>();    // ����� �ҽ� ��������

    player = GameObject.FindGameObjectWithTag("Player");    // �÷��̾� ��ü ��������
    pc = player.GetComponent<PlayerController>();
    playerAtt = pc.att;   // �÷��̾��� ���ݷ� ��������
    playerDefIgn = pc.defIgn;   // �÷��̾��� ���� ���� ��������

    
    spriteRenderer = GetComponent<SpriteRenderer>();    // ��������Ʈ ������ ��������
    defaultMat = spriteRenderer.material;               // ���� ���׸��� ����


    tp = GameObject.FindGameObjectWithTag("Teleporter");     // �ڷ����� ��ü ��������
    tpm = tp.GetComponent<TeleporterManager>();


    mobName = this.gameObject.name;   // ���� ��ü�� �̸� ��������
    hpSlider = gameObject.GetComponentInChildren<Slider>();   // ���� ��ü�� ü�� �����̴� ��������


    // �ʱ� ü��, ü�¹� ����
    currentHp = hp;
    hpSlider.maxValue = hp;
    hpSlider.value = currentHp;

    // ���� �ִϸ�����
    anim = GetComponent<Animator>();


    // �ִϸ������� ��Ʈ�ѷ����� ��� �ִϸ��̼� Ŭ�� ��������
    RuntimeAnimatorController controller = anim.runtimeAnimatorController;
    AnimationClip[] clips = controller.animationClips;

    foreach (AnimationClip clip in clips)
    {
      // ��� �ִϸ��̼��� ���� ��������
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

    if (!canMove)   // �ε��� ���̸� ��ġ ����
    {
      rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
      rb.constraints |= RigidbodyConstraints2D.FreezePositionY;

    }

    if (isFlyingEnemy)
    {
      FlyToPlayer(); // ���� �� ���� �̵� �Լ�
      return;
    }


    PlayAnimation();


    if (isKnockBack || isAttack)
      return;



    else
    {
      // ���� ����ü���� 0���� ��������
      if (currentHp <= 0)
      {
        if (isDead == true)
          return;

        // �������� ���߰� ��� �ִϸ��̼� ���
        rb.linearVelocity = Vector2.zero;
        state = EnemyState.Dead;

        isDead = true;

        // ���̾ �����Ͽ� ���� ����
        gameObject.layer = LayerMask.NameToLayer("Dead");

        // ��ü ��Ȱ��ȭ (���߿� ������ �ٲ� ��)
        Invoke("Death", deathTime);
        return;
      }

      // ���� �߽����� ���� ���� ���� �ִ� �ݶ��̴�
      cols = Physics2D.OverlapCircleAll(transform.position, range);

      // ������ �ݶ��̴� �˻� ����
      foreach (Collider2D col in cols)
      {
        // �ݶ��̴��� �±װ� Player���
        if (col != null && col.CompareTag("Player"))
        {

          Vector2 targetPos = col.transform.position;   // �Ѿƿ� �÷��̾��� ��ġ
          dir = (targetPos - (Vector2)transform.position).normalized; // ���� ����
          float distanceToPlayer = Vector2.Distance(transform.position, targetPos); // �÷��̾���� �Ÿ� ���

          // ���� ������ ���� �÷��̾ ���� ���� �ȿ� �ְ�, ���� ���� �ƴ϶�� ���� ����
          if (distanceToPlayer <= attackRange && !isAttack && !isDead && canAttack)
          {
            TryAttack();
            return;
          }

          // x��, y�� ����
          float xDiff = Mathf.Abs(targetPos.x - transform.position.x);
          float yDiff = Mathf.Abs(targetPos.y - transform.position.y);

          // ���� �������� �ִ� ���
          if (!onGround)
          {
            // �Ʒ��θ� �̵�
            dir = new Vector2(0, -1);
            return;
          }


          // x�� ���̰� ���� ���� ���϶��
          else if (xDiff < 0.2f)
          {

            // �÷��̾�� �Ʒ��� �ִ� ���
            if (yDiff < 0.2f && transform.position.y - targetPos.y > 0)
            {
              rb.linearVelocity = Vector2.zero; // �� ���߱�
              state = EnemyState.Idle; // ��� ����
              return;
            }

            
          }

         
          // �÷��̾�� ���� �����鼭 ������ �ƴѰ����� �̵� ���� ���
          if (yDiff > 3.0f && transform.position.y - targetPos.y > 0 && rb.linearVelocity.x != 0 && !onSlope)
          {
            // �� ������ ����ĳ��Ʈ�� ��� ���� ���� ����
            Vector2 left = Vector2.left;
            Vector2 right = Vector2.right;
            float rayDistance = 3f; // ���� �Ÿ� ����

            Debug.DrawRay(transform.position, left * rayDistance, Color.red);   // ���� ����
            Debug.DrawRay(transform.position, right * rayDistance, Color.red);  // ������ ����

            // ����ĳ��Ʈ�� �� ����
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, rayDistance, groundLayer);
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, rayDistance, groundLayer);


            bool wallLeft = hitLeft.collider != null && Mathf.Abs(Vector2.Angle(hitLeft.normal, Vector2.right)) < 10f;
            bool wallRight = hitRight.collider != null && Mathf.Abs(Vector2.Angle(hitRight.normal, Vector2.left)) < 10f;

            if (wallLeft && !wallRight)
            {
              // ���ʿ� ���� �ְ� �������� ���� ���� �� ���������� �̵�
              dir = Vector2.right;
            }
            else if (!wallLeft && wallRight)
            {
              // �����ʿ� ���� �ְ� ������ ���� ���� �� �������� �̵�
              dir = Vector2.left;
            }
            else if ((!wallLeft && !wallRight) || (wallLeft && wallRight))
            {
              // ���� �� ���� �ְų� ������ ������ ���� �̵�
              dir = (targetPos - (Vector2)transform.position).normalized;
              dir.y = 0; // ���� �̵���

              // �÷��̾ �����ʿ� ����
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

          // ȸ�� ����
          if (dir.x > 0)
          {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            dir.x = 1;
            if (dir.y > 0)
            {
              dir.y = 0;
            }

            // �����̴��� ȸ��ȿ�� ����
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

            // �����̴��� ȸ��ȿ�� ����
            hpSlider.transform.localRotation = Quaternion.Euler(0, 180, 0);

          }
          

          // ���º���
          state = EnemyState.Move;

          return;

        }

      }

      // �÷��̾ �������� ����� ����
      rb.linearVelocity = Vector2.zero;


      // ���� ��ȯ
      state = EnemyState.Idle;

    }

  }


  private void OnCollisionEnter2D(Collision2D col)
  {

    // �ε��� ��ü�� �����̰ų� �÷��̾���
    if (col.gameObject.CompareTag("Player") || (col.gameObject.CompareTag("Shield")))
    {

      // �ε��� ����
      Vector2 hitDir = (transform.position - player.transform.position);

      // �˹� ����, �������� 0
      StartCoroutine(KnockBack(0, hitDir));
    }

    // �ε��� ��ü�� �����̶��
    else if (col.gameObject.CompareTag("Attack"))
    {
      // �ε��� ����
      Vector2 hitDir = (transform.position - player.transform.position);

      // ������ �̸�(���������� �ʿ�)
      string attackName = col.gameObject.name;

      // �˹� ����, ������ ����
      StartCoroutine(KnockBack(playerAtt * PlayerPrefs.GetFloat(attackName), hitDir));

    }


  }

  private void OnCollisionStay2D(Collision2D col)
  {
    // �ε��� ��ü�� ���̶��
    if (col.gameObject.CompareTag("Ground"))
    {
      foreach (ContactPoint2D contact in col.contacts)
      {
        Vector2 normal = contact.normal;

        // �浹�� ǥ���� ������ ���� ��� (���� ������ y���� ���� �̻��̸� �ٴ����� ����)
        if (contact.normal.y > 0.5f)
        {
          onGround = true;
        }

        if (isKnockBack || currentHp <= 0) return; // �˹� ���̰ų� ����� �̵� �ߴ�

        Vector2 slopeDir = new Vector2(-normal.y, normal.x).normalized; // �ð� �������� 90�� ȸ��

        // ���� �ٶ󺸴� ���⿡ ���� ���� ����
        if (transform.rotation.eulerAngles.y == 180)
        {
          slopeDir = -slopeDir;
        }

        float slopeX = Mathf.Abs(slopeDir.x);

        // ���� x���� �ʹ� ������ �̵� ���� (0�� �����)
        if (slopeX < 0.01f)
          return;

        float projectedSpeed = Vector2.Dot(dir, slopeDir);
        float ratio = speed / slopeX;


        Vector2 moveDir = slopeDir * projectedSpeed * ratio;

        // �ӵ� ����
        rb.linearVelocity = moveDir;
        
      }

    }
  }



  void OnCollisionExit2D(Collision2D col)
  {
    // ������ ��ü�� ���̶��
    if (col.gameObject.CompareTag("Ground"))
    {
      // �˹� ���� �ƴ� ��
      if (!isKnockBack)
      {
        onGround = false;
      }
    }
  }

  // ���
  void Death()
  {
    // ��ü ���� 
    Destroy(gameObject);

    // �ڷ����Ͱ� �غ����� �ƴ� ���� ����ġ, ���, ������ ȹ��
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

    // ���� �߿� �ǰ� �� ���� ���� ����
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
      rb.linearVelocity = Vector2.zero;

      isRecentKnockback = true;
    }

    audioSource.PlayOneShot(hitClip);

    int totalDem = Mathf.RoundToInt(dem - (dem * (def - (def * (playerDefIgn * 0.01f))) * 0.01f));

    // ü�� ����
    currentHp -= totalDem;

    // ü�� �� �ݿ�
    hpSlider.value = currentHp;

    ShowDamageText(totalDem);

    // ���� ���̰ų� ����ϰų� �ε����̸� �˹� ȿ�� ���
    if (isAttack || currentHp <= 0 || !canMove)
    { yield break; }


    state = EnemyState.KnockBack;
    isKnockBack = true;

    // �˹� ȿ��
    rb.linearVelocity = Vector2.zero;
    // ������ �������κ��� ���� ��
    if (dir.x > 0)
    {
      // ���� ���̶�� �����θ� �и�
      if (isFlyingEnemy)
        rb.AddForce(new Vector2(2, 0) * 1.2f, ForceMode2D.Impulse);
      else
        rb.AddForce(new Vector2(2, 1) * 1.2f, ForceMode2D.Impulse);
    }

    // ������ ���������κ��� ���� ��
    else if (dir.x < 0)
    {
      // ���� ���̶�� �����θ� �и�
      if (isFlyingEnemy)
        rb.AddForce(new Vector2(-2, 0) * 1.2f, ForceMode2D.Impulse);
      else
        rb.AddForce(new Vector2(-2, 1) * 1.2f, ForceMode2D.Impulse);
    }


    StartCoroutine(HitFlash());   // �ǰݽ� ������� �����̴� ȿ��

    // �˹��� ������ ���� ���
    yield return new WaitForSeconds(0.5f);


    isKnockBack = false;
    state = EnemyState.Idle;

    if(isAttack)
    {
      rb.linearVelocity = Vector2.zero;
      yield return new WaitForSeconds(1);
      isAttack = false;
    }

  }

  // �ǰ� �ð� ȿ��
  IEnumerator HitFlash()
  {
    spriteRenderer.material = hitFlashMat;  // ���׸��� ����
    yield return new WaitForSeconds(0.15f); // ��� ��ٷȴٰ�
    spriteRenderer.material = defaultMat; // ���� ������ ����
  }

  public void ForceDeath()
  {
    if (!isDead)  // �̹� ���� ���� �ٽ� ������ ����
    {
      currentHp = 0;

    }
  }

  void DropItem()
  {
    float rnd = UnityEngine.Random.value;

    // ������ ��� Ȯ�� ���� 
    if (rnd > pc.itemDrop)
    {
      return;
    }


    // �����ۺ� Ȯ�� ����� ���� �� ����ġ ���
    float totalWeight = 0f;

    foreach (var obj in dropItemPool)
    {
      ItemManager item = obj.GetComponent<ItemManager>();

      if (item != null && item.itemData != null)
      {
        totalWeight += item.itemData.weight;
      }
    }


    // ���� ���� �̰� �׿� �ش��ϴ� ������ ����
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
    // �˹� ���̶�� ���� �Ұ�
    if (isKnockBack)
      yield break;

    isAttack = true;
    isAttacking = true;
    rb.linearVelocity = Vector2.zero;
    state = EnemyState.Attack;
    

    // �ڽ� Ŭ�������� ���� ����
    yield return null;

    yield return new WaitForSeconds(1.5f); // ���� �� ��Ÿ��
    isAttacking = false;

    yield return new WaitForSeconds(0.5f); // ���� �� ��Ÿ��
    isAttack = false;
    state = EnemyState.Idle;
  }

  // ���� �� �̵� �޼���
  protected virtual void FlyToPlayer()
  {
    if (player == null)
      return;

    

    PlayAnimation();

    // ���� ����ü���� 0���� ��������
    if (currentHp <= 0)
    {
      if (isDead == true)
        return;

      // �������� ���߰� ��� �ִϸ��̼� ���
      rb.linearVelocity = Vector2.zero;
      state = EnemyState.Dead;

      isDead = true;

      // ���̾ �����Ͽ� ���� ����
      gameObject.layer = LayerMask.NameToLayer("Dead");

      // ��ü ��Ȱ��ȭ (���߿� ������ �ٲ� ��)
      Invoke("Death", deathTime);
      return;
    }

    // �������̰ų� �ǰ����̸� �̵� ���
    if (isKnockBack)
      return;

   

    float distance = Vector2.Distance(transform.position, player.transform.position);

    // ���� ������ ���� �÷��̾ ���� ���� �ȿ� �ְ�, ���� ���� �ƴ϶�� ���� ����
    if (distance <= attackRange && !isAttack && !isDead && canAttack)
    {
      TryAttack();
      return;
    }

    // ���� �� �÷��̾� Ž��
    if (distance <= range)
    {
      Vector2 dir = ((player.transform.position - transform.position) + new Vector3(0, 0.6f)).normalized;

      // ȸ�� ���� ����
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

     
      // �ӵ� ����
      rb.linearVelocity = dir * speed;

      
      

     if(!isAttack && !isAttacking)
      state = EnemyState.Move;

      
    }
    else
    {
      
      rb.linearVelocity = Vector2.zero;
      state = EnemyState.Idle;
    }

  }

  // ���� ���� �޼���
  protected virtual IEnumerator DashAttack(Vector2 direction, float dashPower, float chargeTime, float duration)
  {
    isDashing = true;

    rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
    rb.linearVelocity = Vector2.zero;
    yield return new WaitForSeconds(chargeTime); // ���� �� �غ� �ð�

    rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
    rb.AddForce(direction.normalized * dashPower, ForceMode2D.Impulse);

    yield return new WaitForSeconds(duration); // ���� ���� �ð�

    rb.linearVelocity = Vector2.zero;
    isDashing = false;
  }

  protected IEnumerator Stay()
  {
    yield return new WaitForSeconds(1f);
  }

  void ShowDamageText(int damage)
  {

    // �� ���� ����
    Vector3 spawnPos = transform.position + Vector3.up * 0.3f;

    GameObject textObj = Instantiate(demTextObj, spawnPos, Quaternion.identity);
    DemText damageTextScript = textObj.GetComponent<DemText>();
    damageTextScript.SetText(damage);
  }

  public void TakeDamage(float damage)
  {
    if (isDead) return;

    int totalDamage = Mathf.RoundToInt(damage - (damage * (def - (def * (playerDefIgn * 0.01f))) * 0.01f));

    // ü�� ����
    currentHp -= totalDamage;

    // ü�¹� �ݿ�
    hpSlider.value = currentHp;

    // ������ �ؽ�Ʈ ���
    ShowDamageText(totalDamage);

    // �ǰ� �� �����̱�
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
