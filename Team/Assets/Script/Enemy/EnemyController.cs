using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyController : MonoBehaviour
{
  protected GameObject player;    // �÷��̾� ������Ʈ
  protected PlayerController pc;  // �÷��̾� ��Ʈ�ѷ�
  protected SpawnManager sm;
  protected Vector2 dir;          // �̵� ����
  protected Rigidbody2D rb;       //������ٵ�
  protected Material originMat;   // �⺻ ���׸���


  // public virtual event System.Action OnDeath; // ����� �߻��ϴ� �̺�Ʈ
  public static System.Action OnDeath; 
  public static System.Action OnBossDeath; 



  protected Animator animator;        //�� �ִϸ�����
  protected bool isWalking = false;   //���� �Ȱ��ִ°�
  protected bool forceDie = false;

  public bool isAttacking;        //������
  protected bool canAttack = true;  //���ݰ���?

  protected Vector3 originalSliderScale;  //�����̴����Ⱚ����

  public bool isDead = false; //�׾��°� üũ(�̵������� ����)


  [Header("����")]
  public float maxHp;       //�� ���� �뷱��
  public float currentHp;
  public float moveSpeed;    // �̵��ӵ�
  public float attackDem;    // ���ݷ�
  public float defence;     // ����
  public float defIgnore;   // ����


  [Header("�Ҵ� ������Ʈ")]
  public Slider slider;     //�� ü�¹�
  public GameObject expObj; // ����ġ ������Ʈ
  public GameObject potionObj;  // ���� ������Ʈ
  public GameObject damageTextPrefab;   // ������ �ؽ�Ʈ
  public Material hitMat;                 // �ǰ� ���׸���

  //ȿ����
  private AudioSource audioSource;

  protected virtual void Start()
  {
    player = GameObject.FindWithTag("Player");    // �÷��̾� ã��
    pc = player.GetComponent<PlayerController>();
    animator = GetComponent<Animator>();          //�� �ִϸ����� ��������
    rb = GetComponent<Rigidbody2D>();
    originMat = GetComponent<SpriteRenderer>().material;
    

    //ü�¹� �ʱ� ����
    slider.maxValue = maxHp;
    slider.minValue = 0;
    slider.value = currentHp;
    originalSliderScale = slider.transform.localScale;

    //������ҽ� ������Ʈ ã��
    audioSource = GetComponent<AudioSource>();
  }

  protected virtual void Update()
  {
    if (isDead) return; //�׾����� �ƹ��͵� ����

    if (isAttacking) return;

    dir = (player.transform.position - transform.position).normalized;    // �÷��̾ ���ϴ� ����
    transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);    // �̵�

    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;              // ���� ���ϱ�

    //�̵� ���̸� isWalking true�� ����(bool �Ķ����)
    isWalking = dir.magnitude > 0.01f;
    animator.SetBool("isWalking", isWalking);
   

    //���⿡ ���� flipó��
    if (dir.x >= 0)
    {
      transform.localScale = new Vector3(1, 1, 1);//������
      slider.transform.localScale = new Vector3(Mathf.Abs(originalSliderScale.x), originalSliderScale.y, originalSliderScale.z);
    }
    else
    {
      transform.localScale = new Vector3(-1, 1, 1); //�¿����
      slider.transform.localScale = new Vector3(-Mathf.Abs(originalSliderScale.x), originalSliderScale.y, originalSliderScale.z);
    }
  }

  protected virtual void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("PlayerAttack"))       // ���ݰ� �ε����� ���
    {

      Vector2 dir = (collision.transform.position - player.transform.position).normalized;    // �о ����

      rb.AddForce(dir * pc.knockBackForce, ForceMode2D.Impulse);             // �˹�
      StartCoroutine(StopEnemy(rb));                            // 0.2���� ����

      //���⼭���� �����̴� ü�±�� �����ڵ�
      BulletController bc = collision.GetComponent<BulletController>();   // ����� �������� ���� �Ҹ� ��Ʈ�ѷ� ��������

      if (bc == null || bc.hasHit) return;  // �̹� �ǰ��� �Ѿ��̶�� �ǰ�ȿ�� ����

      bc.hasHit = true;  // �ǰ� ó�� ǥ��

      //�߻�ü ����
      Destroy(collision.gameObject);

      float damage = pc.attackDem * bc.damageCoefficient;                 // �޴� ���ط� = �÷��̾� ���ݷ� * ������ ���

      HitEnemy(damage); // ������ ����
      
    }
  }

  public virtual void Dead()
  {
    isDead = true; //�̵������
    currentHp = 0; //���̳ʽ��� �������°� ����
    rb.velocity = Vector2.zero; //�˹����
    rb.simulated = false; //���� ��Ȱ��ȭ
    animator.SetTrigger("Dead"); //Dead �ִϸ��̼� ���

    //ȿ�������
    if (audioSource != null)
    {
      string prefabName = gameObject.name; // ���� ������Ʈ �̸�
      if (prefabName.Contains("EliteBoss1_Cat")) // �������̸��� ����Ʈ����1�̸�
      {
        audioSource.PlayOneShot(SFXManager.instance.catKill);
      }
      else if (prefabName.Contains("Stage3Boss") || prefabName.Contains("EliteBoss3"))// ����Ʈ ����3 �̳� ����������
      {
        audioSource.PlayOneShot(SFXManager.instance.boss3Kill);
      }
      else
      {
        if (SFXManager.instance.enemyKill != null)
          audioSource.PlayOneShot(SFXManager.instance.enemyKill);
      }
    }
    OnDeath?.Invoke();
  }

  public virtual void HitEnemy(float damage)
  {
    // ������ ���
    float finalDamage = Mathf.RoundToInt(damage - (damage * (defence - (defence * (pc.defIgnore * 0.01f))) * 0.01f)); 
    bool isCritical = Random.value < (pc.criticalChance / 100f);    // ġ��Ÿ Ȯ��

    // ġ��Ÿ���
    if (isCritical)
    {
      finalDamage *= pc.criticalDem;     // ������ ����
    }


    currentHp -= Mathf.RoundToInt(finalDamage);          //���� ����
    slider.value = currentHp; //�����̴�����
    StartCoroutine(MaterialShift());



    // ������ �ؽ�Ʈ ����
    GameObject dmgText = Instantiate(damageTextPrefab, transform.position + Vector3.up, Quaternion.identity);
    dmgText.GetComponent<DamageTextController>().SetDamage(finalDamage, isCritical);   // ������, ġ��Ÿ ���� ����

    if (currentHp <= 0)
      Dead();

   
  }

  // �� ���� �޼���
  protected IEnumerator StopEnemy(Rigidbody2D rb)
  {
    yield return new WaitForSeconds(0.05f);  // 0.05�� �Ŀ�
    rb.velocity = Vector2.zero;             // ����
  }

  //�� ��ü �������� �̺�Ʈ�� �Լ�
  public virtual void EnemyDie()
  {
    // ���� ����� �ƴ� ��� ����ġ ���
    if(!forceDie)
    {
      DropObject();      // ����ġ ������Ʈ ���
      pc.killCount++;       // óġ Ƚ�� �߰�
      pc.KillCountUpdate(); // óġ Ƚ�� ǥ�� ������Ʈ
    }

    Destroy(gameObject); //�� ����

  }

  // ������Ʈ ���
  void DropObject()
  {
    float chance = Random.Range(0f, 100f);
    if (chance < 1f)
    {
      if (potionObj != null)
      Instantiate(potionObj, transform.position, Quaternion.Euler(0,0,0));
    }
    else
    {
      Instantiate(expObj, transform.position, Quaternion.Euler(0, 0, 0));
    }
  }

  protected IEnumerator MaterialShift()
  {
    GetComponent<SpriteRenderer>().material = hitMat;   // ���׸��� ����(������ ������ ȿ��)
    yield return new WaitForSeconds(0.15f); // 0.15��
    GetComponent<SpriteRenderer>().material = originMat;  // ���׸��� ����
  }

  // ���� ���
  public void ForceDie()
  {
    forceDie = true;
    Dead();
  }
}
