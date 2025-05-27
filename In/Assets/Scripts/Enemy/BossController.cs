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
  protected Vector2 dir;            // �̵�����

  public bool isDefaultLeft = false;
  protected bool onGround = false;       // ���� ��Ҵ��� ����

  public float returnDistance = 5f; //  ���� �Ÿ�
  private bool isReturning = false; // ���� ������ Ȯ��

  protected enum EnemyState { Idle, Move, Attack, KnockBack, Dead }   // ���� ����
  protected EnemyState state;   // ���� ����


  protected Animator anim;  // �� �ִϸ�����
  protected float deathTime;    // �� ��� �ִϸ��̼��� ����(�ִϸ��̼��� �̸��� �ݵ�� "�̸�_Death"�� �� ��)
  public bool isDead = false;   // �� ��� ����

  protected float range;  // Ž�� ����
  protected float speed;   // �̵��ӵ�
  protected Collider2D[] cols;   // Ž���� �ݶ��̴�


  protected float attackRange; // ���� ����
  protected bool isAttack;     // ���� ��(���� ���ð��� ���� ����)
  protected bool isAttacking;  // ���� ��� ������
  protected float accumulatedDamage = 0f;   // �˹� ó���� ���� ���� ������

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

  public GameObject[] dropItemPool;   // óġ�� ����ϴ� ������ 

  public bool isKnockBack = false;    // �˹� ���� ����


  protected GameObject player;    // �÷��̾� ��ü
  protected PlayerController pc;  // �÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ
  protected float playerAtt;    // �÷��̾� ��ü�� ���ݷ�
  protected float playerDefIgn; // �÷��̾� ��ü�� ���� ����

  protected GameObject tp; // �ڷ����� ��ü
  protected TeleporterManager tpm; // �ڷ����� ��ü

  public GameObject demTextObj;   // ������ �ؽ�Ʈ ������Ʈ
  public AudioClip hitClip;



  protected virtual void Awake()
  {

  }

  protected virtual void Start()
  {

    rb = GetComponent<Rigidbody2D>();
    box = GetComponent<BoxCollider2D>();
    audioSource = GetComponent<AudioSource>();

    player = GameObject.FindGameObjectWithTag("Player");    // �÷��̾� ��ü ��������
    pc = player.GetComponent<PlayerController>();
    playerAtt = pc.att;   // �÷��̾��� ���ݷ� ��������
    playerDefIgn = pc.defIgn;   // �÷��̾��� ���� ���� ��������


    tp = GameObject.FindGameObjectWithTag("Teleporter");     // �ڷ����� ��ü ��������
    tpm = tp.GetComponent<TeleporterManager>();

    spriteRenderer = GetComponent<SpriteRenderer>();
    defaultMat = spriteRenderer.material;


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


    PlayAnimation();


    if (isKnockBack)
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

        // ��ü ����
        Invoke("Death", deathTime + 1f);
        return;
      }

      if (isReturning)
      {
        Vector2 returnDir = ((Vector2)tp.transform.position - (Vector2)transform.position).normalized;

        // ������ �ڷ����� ��ġ�� ���� �����ߴٸ�
        if (Vector2.Distance(transform.position, tp.transform.position) < 1.6f)
        {
          isReturning = false;
          rb.linearVelocity = Vector2.zero;
          state = EnemyState.Idle;

          currentHp += Mathf.RoundToInt(hp * 0.1f);
          if(currentHp > hp)
          {
            currentHp = hp;
          }
          

          return;
        }

        // �ڷ����� �������� �̵�
        rb.linearVelocity = returnDir * speed;

        // ȸ��
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

      // ���� �߽����� ���� ���� ���� �ִ� �ݶ��̴�
      cols = Physics2D.OverlapCircleAll(transform.position, range);

      // ������ �ݶ��̴� �˻� ����
      foreach (Collider2D col in cols)
      {
        // �ݶ��̴��� �±װ� Player���
        if (col != null && col.CompareTag("Player"))
        {
          Vector2 targetPos = col.transform.position; // �Ѿƿ� �÷��̾��� ��ġ
          dir = (targetPos - (Vector2)transform.position).normalized;     // ���� ����
          float distanceToPlayer = Vector2.Distance(transform.position, targetPos); // �÷��̾���� �Ÿ� ���

          // �÷��̾ ���� ���� �ȿ� �ְ�, ���� ���� �ƴ϶�� ���� ����
          if (distanceToPlayer <= attackRange && !isAttack && !isDead)
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
            // y�� ���̰� ���� ���� �̻��̸鼭 �÷��̾�� ���� �ִ� ���
            if (yDiff > 0.2f && transform.position.y - targetPos.y > 0)
            {
              // ������� ����
              return;
            }

            rb.linearVelocity = Vector2.zero; // �� ���߱�
            state = EnemyState.Idle; // ��� ����
            return;
          }

          // �÷��̾�� ���� �����鼭 ������ �ƴѰ����� �̵� ���� ���
          if (yDiff > 3.0f && transform.position.y - targetPos.y > 0 && rb.linearVelocity.x != 0)
          {
            // ������� ����
            return;
          }

          // ȸ�� ����
          if (dir.x > 0)
          {
            // ���������� �̵� ���� ��
            transform.rotation = isDefaultLeft ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            dir.x = 1;
            if (dir.y > 0) dir.y = 0;

            // �����̴��� ȸ��ȿ�� ����
            hpSlider.transform.localRotation = isDefaultLeft ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
          }

          else if (dir.x < 0)
          {
            // �������� �̵� ���� ��
            transform.rotation = isDefaultLeft ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            dir.x = -1;
            if (dir.y > 0) dir.y = 0;

            // �����̴��� ȸ��ȿ�� ����
            hpSlider.transform.localRotation = isDefaultLeft ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

          }

          if (!CanMove())
          {
            rb.linearVelocity = Vector2.zero;
            state = EnemyState.Idle;
            return;
          }

          UpdateConstraints();

          // �÷��̾ ���� ���� �ȿ� �ְų� ���� ���̶�� �̵� ��ȿ
          if (distanceToPlayer <= attackRange || isAttack)
          {
            state = EnemyState.Idle;
            return;
          }

          if (!isDead && !isKnockBack && Vector2.Distance(transform.position, tp.transform.position) > returnDistance)
          {
            isReturning = true;
          }

          // �ӵ� ����
          // rb.velocity = dir * speed;

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
    // �ε��� ��ü�� �����̶��
    if (col.gameObject.CompareTag("Attack"))
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

    
    ExpManager.instance.AddExp(exp);
    GoldManager.instance.GetGold(gold);

    DropItem();

  }


  public IEnumerator KnockBack(float dem, Vector2 dir)
  {
    // �˹����̶�� ����
    if(isKnockBack)
      yield break;

    audioSource.PlayOneShot(hitClip);

    // ü�� ���� ���
    float damage = dem - (dem * (def - (def * (playerDefIgn * 0.01f))) * 0.01f);
    int intDamage = Mathf.RoundToInt(damage);
    currentHp -= intDamage;

    ShowDamageText(intDamage);

    // ü�� �� �ݿ�
    hpSlider.value = currentHp;

    // ���� ������ ���ϱ�
    accumulatedDamage += intDamage;

    // ���� ���̰ų� ���� �������� ������ �ǰ� �ִϸ��̼� ��� ����
    if (isAttacking || accumulatedDamage < hp * 0.1f)
      yield break;

    state = EnemyState.KnockBack;
    isKnockBack = true;

    accumulatedDamage = 0f;

    Golem golem = this as Golem;  // ���� ��ü�� Golem���� ��ȯ �õ�
    if (golem != null && (golem.phase == 1 || golem.phase == 3))
    {
      // �ǰ� �ִϸ��̼Ǹ� ���
      yield return new WaitForSeconds(0.7f);
      isKnockBack = false;
      state = EnemyState.Idle;
      yield break;
    }


      // �˹� ȿ��
      rb.linearVelocity = Vector2.zero;
    // ������ �������κ��� ���� ��
    if (dir.x > 0)
    {
      rb.AddForce(new Vector2(2, 1) * 1.2f, ForceMode2D.Impulse);
    }

    // ������ ���������κ��� ���� ��
    else if (dir.x < 0)
    {
      rb.AddForce(new Vector2(-2, 1) * 1.2f, ForceMode2D.Impulse);
    }

    StartCoroutine(HitFlash());

    // �˹��� ������ ���� ���
    yield return new WaitForSeconds(0.4f);


    isKnockBack = false;
    state = EnemyState.Idle;

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
        Debug.Log(randomValue);
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

  protected virtual bool CanMove()
  {
    return true; // �⺻���� �̵� ���
  }

  // �˹���ϰų� �̵����� �����ϰ��� ���� �̵� �Ұ�
  protected virtual void UpdateConstraints()
  {
    if (state == EnemyState.Idle || state == EnemyState.Attack)
    {
      // �̵� ����
      rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
    }
    else
    {
      // �̵� ��� (Freeze ����)
      rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
    }
  }

  void ShowDamageText(int damage)
  {

    // �� ���� ����
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
