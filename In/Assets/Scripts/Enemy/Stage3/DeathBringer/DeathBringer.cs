using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeathBringer : BossController
{
  public int phase = 1;
  protected bool isUpgrade = false;

  public PhysicsMaterial2D lowFrictionMaterial;  // �̵� ������ �� ����� ���� ��Ƽ����
  public PhysicsMaterial2D highFrictionMaterial; // ���� �� ����� ���� ��Ƽ����

  // ���� ������
  public GameObject Attack1;
  public GameObject Attack2;

  // ��ȯ�� ������Ʈ
  public GameObject angel;
  public GameObject firewizard;
  public GameObject haloghost;

  GameObject[] enemySpawners; // ���ʹ� ������ �迭
  Vector2[] randomPositions = new Vector2[4]; // �����ϰ� ���� 4���� ��ġ

  // ���� �ߺ� ��ȯ ���� ����
  bool hasPhase2Started = false;
  bool hasPhase3Started = false;




  protected override void Awake()
  {
    // 1������ ����
    range = 10.0f; // Ž�� ����
    attackRange = 4.0f; // ���ݹ���
    speed = 3.0f; // �̵��ӵ�
    att = 10;     // ���ݷ�
    def = 10;     // ����
    hp = 600;      // ü��
    exp = 20;     // ����ġ
    gold = 30;     // ��
    defIgn = 20;   // ���� ����
    returnDistance = 200f;
  }

  protected override void Start()
  {
    // ���ʹ� ������ ����
    enemySpawners = FindObjectsOfType<GameObject>()
        .Where(obj => obj.name.Contains("EnemySpawner")) // �̸��� EnemySpawner�� ���Ե� ������Ʈ�� ����
        .ToArray(); // �迭�� ��ȯ


    // ���� 4�� ����
    enemySpawners = enemySpawners.OrderBy(obj => Random.value).ToArray();

    randomPositions[0] = enemySpawners[0].transform.position; // 1�� ��ġ
    randomPositions[1] = enemySpawners[1].transform.position; // 2�� ��ġ
    randomPositions[2] = enemySpawners[2].transform.position; // 3�� ��ġ
    randomPositions[3] = enemySpawners[3].transform.position; // 4�� ��ġ

    currentHp = hp;

    anim = gameObject.GetComponent<Animator>();

    base.Start();

  }

  protected override void Update()
  {

    // ü�¿� ���� ������ ����
    if (!hasPhase2Started && currentHp <= hp * 0.7f && phase == 1)   // 2������
    {
      hasPhase2Started = true; // �� �̻� ���� �� �ǵ��� ����
      
      anim.SetTrigger("Upgrade");
      StartCoroutine(PhaseChange2());
    }


    if (!hasPhase3Started && currentHp <= hp * 0.3f && phase == 2)   // 2������
    {
      hasPhase3Started = true;
      anim.SetTrigger("Upgrade");
      StartCoroutine(PhaseChange3());
    }


    if (isUpgrade)
      return;


    

    base.Update();
  }

  protected override IEnumerator PerformAttack()
  {
    isAttack = true;
    isAttacking = true;
    rb.linearVelocity = Vector2.zero;
    box.sharedMaterial = highFrictionMaterial;

    int randomAttack = Random.Range(0, 12); // 0~11 ����

    randomAttack = 1;

    if (randomAttack  < 5)
    {
      anim.SetTrigger("Attack1");
      Vector2 hitDir = (player.transform.position - transform.position);

      if (hitDir.x > 0)
      {
        Vector2 pos = transform.position + new Vector3(2.3f, 1.8f);

        StartCoroutine(Attack_1(pos));

      }

      else if (hitDir.x <= 0)
      {
        Vector2 pos = transform.position + new Vector3(-2.4f, 1.8f);
        StartCoroutine(Attack_1(pos));

      }

    }

    else if (randomAttack > 9)
    {
      StartCoroutine(SpawnEnemy3());
    }

    else
    {
      StartCoroutine(Attack_2(player.transform.position + Vector3.up * 1));
    }


    yield return new WaitForSeconds(1f); // ���� �� ��Ÿ��
    isAttacking = false;

    yield return new WaitForSeconds(0.5f); // ���� �� ��Ÿ��

    box.sharedMaterial = lowFrictionMaterial;
    isAttack = false;
    state = EnemyState.Idle;
  }


  // 1�� ����
  IEnumerator Attack_1(Vector2 pos)
  {

    yield return new WaitForSeconds(0.5f); // 0.3�� ��ٸ���

    if (currentHp <= 0)
    {
      isAttack = false;
      isAttacking = false;
      anim.SetInteger("State", 4);
      yield break;
    }

    GameObject clone = Instantiate(Attack1, pos, Quaternion.identity);

    Vector2 hitDir = (player.transform.position - transform.position);
    if (hitDir.x > 0)
    {
      clone.transform.rotation = isDefaultLeft ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }

    else if (hitDir.x <= 0)
    {
      clone.transform.rotation = isDefaultLeft ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }

      Destroy(clone, 0.6f);
  }

  // 2�� ����
  IEnumerator Attack_2(Vector2 startPos)
  {
    anim.SetTrigger("Attack2");

    yield return new WaitForSeconds(0.7f);


    GameObject attackObj = Instantiate(Attack2, startPos, Quaternion.identity);

    float followDuration = 1.7f; // ����ٴϴ� �ð�
    float timer = 0f;

    // �÷��̾ ����ٴϴ� ����
    while (timer < followDuration)
    {
      if (player != null)
      {
        // �÷��̾� ��ġ�� ����
        attackObj.transform.position = player.transform.position + Vector3.up * 2.5f;
      }

      timer += Time.deltaTime;
      yield return null;
    }

    // ����ٴϴ� �� ���߰� �� �ڸ��� ����
    Vector2 stopPos = attackObj.transform.position;
    attackObj.transform.position = stopPos;

    // ��ٸ� �� ���� ����
    yield return new WaitForSeconds(0.5f);

    // �簢�� ���� ����
    Vector2 boxSize = new Vector2(2.3f, 3.5f); // ���� ũ��
    Collider2D hit = Physics2D.OverlapBox(stopPos, boxSize, 0f, LayerMask.GetMask("Player"));

    if (hit != null)
    {
      // ������ �ִ� �ڵ� ȣ�� (��: hit.GetComponent<PlayerController>().TakeDamage(att); )
      PlayerController pc = hit.GetComponent<PlayerController>();
      if (pc != null)
      {
        pc.TakeDem(45,30, player.transform.position - transform.position);  // ������
      }
    }


    // 1.2�� �� ����
    yield return new WaitForSeconds(0.4f);
    Destroy(attackObj);
  }

  


  IEnumerator PhaseChange2()
  {
    isUpgrade = true;
    box.sharedMaterial = highFrictionMaterial;
    anim.SetTrigger("Upgrade");


    yield return new WaitForSeconds(4);

    yield return StartCoroutine(SpawnEnemy1());

    phase = 2;
    isUpgrade = false;
    box.sharedMaterial = lowFrictionMaterial;
  }

  IEnumerator SpawnEnemy1()
  {
    // �� �� ��ġ
    Vector2 pos1 = transform.position + (Vector3.right * 3.5f);
    Vector2 pos2 = transform.position + (Vector3.left * 3.5f);

    // �밢�� ����
    Vector2 pos3 = transform.position + new Vector3(2, 5.5f);
    Vector2 pos4 = transform.position + new Vector3(-2, 5.5f);

    // �翷�� ������ ��ȯ
    GameObject clone1 = Instantiate(firewizard, pos1, Quaternion.identity);
    GameObject clone2 = Instantiate(firewizard, pos2, Quaternion.identity);
    clone1.name = firewizard.name;
    clone2.name = firewizard.name;

    yield return new WaitForSeconds(4);

    anim.SetTrigger("Upgrade");
    // �����ʿ� õ�� ��ȯ
    GameObject clone5 = Instantiate(angel, randomPositions[0], Quaternion.identity);
    GameObject clone6 = Instantiate(angel, randomPositions[1], Quaternion.identity);
    clone5.name = angel.name;
    clone6.name = angel.name;

    yield return new WaitForSeconds(3);

    // �밢���� ����Ʈ ��ȯ
    GameObject clone3 = Instantiate(haloghost, pos3, Quaternion.identity);
    GameObject clone4 = Instantiate(haloghost, pos4, Quaternion.identity);
    clone3.name = haloghost.name;
    clone4.name = haloghost.name;

    yield return new WaitForSeconds(3);
  }


  IEnumerator PhaseChange3()
  {
    isUpgrade = true;
    box.sharedMaterial = highFrictionMaterial;
    anim.SetTrigger("Upgrade");


    yield return new WaitForSeconds(4);
    yield return StartCoroutine(SpawnEnemy2());
    phase = 2;
    isUpgrade = false;
    box.sharedMaterial = lowFrictionMaterial;
  }

  IEnumerator SpawnEnemy2()
  {
    // �� �� ��ġ
    Vector2 pos1 = transform.position + (Vector3.right * 3.5f);
    Vector2 pos2 = transform.position + (Vector3.left * 3.5f);


    // �翷�� ������ ��ȯ
    GameObject clone1 = Instantiate(firewizard, pos1, Quaternion.identity);
    GameObject clone2 = Instantiate(firewizard, pos2, Quaternion.identity);
    clone1.name = firewizard.name;
    clone2.name = firewizard.name;

    yield return new WaitForSeconds(2.5f);

    anim.SetTrigger("Upgrade");
    // �����ʿ� õ�� ��ȯ
    GameObject clone3 = Instantiate(angel, randomPositions[0], Quaternion.identity);
    GameObject clone4 = Instantiate(angel, randomPositions[1], Quaternion.identity);
    GameObject clone5 = Instantiate(angel, randomPositions[2], Quaternion.identity);
    GameObject clone6 = Instantiate(angel, randomPositions[3], Quaternion.identity);

    clone3.name = angel.name;
    clone4.name = angel.name;
    clone5.name = angel.name;
    clone6.name = angel.name;

    yield return new WaitForSeconds(2);
  }

  // ������ �Ǵ� ���� ��ȯ
  IEnumerator SpawnEnemy3()
  {

    int randomAttack = Random.Range(0, 2); // 0~1 ����

    yield return new WaitForSeconds(3);


    if (randomAttack == 0)
    {
      // �� �� ��ġ
      Vector2 pos1 = transform.position + (Vector3.right * 2);
      Vector2 pos2 = transform.position + (Vector3.left * 2);

      // �翷�� ������ ��ȯ
      GameObject clone1 = Instantiate(firewizard, pos1, Quaternion.identity);
      GameObject clone2 = Instantiate(firewizard, pos2, Quaternion.identity);
      clone1.name = firewizard.name;
      clone2.name = firewizard.name;

    }

    else
    {
      // �밢�� ����
      Vector2 pos3 = transform.position + new Vector3(2, 4);
      Vector2 pos4 = transform.position + new Vector3(-2, 4);

      // �밢���� ����Ʈ ��ȯ
      GameObject clone3 = Instantiate(haloghost, pos3, Quaternion.identity);
      GameObject clone4 = Instantiate(haloghost, pos4, Quaternion.identity);
      clone3.name = haloghost.name;
      clone4.name = haloghost.name;
    }


    yield return new WaitForSeconds(2);

  }
}
