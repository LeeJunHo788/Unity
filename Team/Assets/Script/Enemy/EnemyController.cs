using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyController : MonoBehaviour
{
  protected GameObject player;    // 플레이어 오브젝트
  protected PlayerController pc;  // 플레이어 컨트롤러
  protected SpawnManager sm;
  protected Vector2 dir;          // 이동 방향
  protected Rigidbody2D rb;       //리지드바디
  protected Material originMat;   // 기본 마테리얼


  // public virtual event System.Action OnDeath; // 사망시 발생하는 이벤트
  public static System.Action OnDeath; 
  public static System.Action OnBossDeath; 



  protected Animator animator;        //적 애니메이터
  protected bool isWalking = false;   //적이 걷고있는가
  protected bool forceDie = false;

  public bool isAttacking;        //공격중
  protected bool canAttack = true;  //공격가능?

  protected Vector3 originalSliderScale;  //슬라이더방향값저장

  public bool isDead = false; //죽었는가 체크(이동방지용 변수)


  [Header("스탯")]
  public float maxHp;       //값 추후 밸런싱
  public float currentHp;
  public float moveSpeed;    // 이동속도
  public float attackDem;    // 공격력
  public float defence;     // 방어력
  public float defIgnore;   // 방어무시


  [Header("할당 오브젝트")]
  public Slider slider;     //적 체력바
  public GameObject expObj; // 경험치 오브젝트
  public GameObject potionObj;  // 포션 오브젝트
  public GameObject damageTextPrefab;   // 데미지 텍스트
  public Material hitMat;                 // 피격 마테리얼

  //효과음
  private AudioSource audioSource;

  protected virtual void Start()
  {
    player = GameObject.FindWithTag("Player");    // 플레이어 찾기
    pc = player.GetComponent<PlayerController>();
    animator = GetComponent<Animator>();          //적 애니메이터 가져오기
    rb = GetComponent<Rigidbody2D>();
    originMat = GetComponent<SpriteRenderer>().material;
    

    //체력바 초기 설정
    slider.maxValue = maxHp;
    slider.minValue = 0;
    slider.value = currentHp;
    originalSliderScale = slider.transform.localScale;

    //오디오소스 컴포넌트 찾기
    audioSource = GetComponent<AudioSource>();
  }

  protected virtual void Update()
  {
    if (isDead) return; //죽었으면 아무것도 안함

    if (isAttacking) return;

    dir = (player.transform.position - transform.position).normalized;    // 플레이어를 향하는 방향
    transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);    // 이동

    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;              // 각도 구하기

    //이동 중이면 isWalking true로 설정(bool 파라미터)
    isWalking = dir.magnitude > 0.01f;
    animator.SetBool("isWalking", isWalking);
   

    //방향에 따라 flip처리
    if (dir.x >= 0)
    {
      transform.localScale = new Vector3(1, 1, 1);//오른쪽
      slider.transform.localScale = new Vector3(Mathf.Abs(originalSliderScale.x), originalSliderScale.y, originalSliderScale.z);
    }
    else
    {
      transform.localScale = new Vector3(-1, 1, 1); //좌우반전
      slider.transform.localScale = new Vector3(-Mathf.Abs(originalSliderScale.x), originalSliderScale.y, originalSliderScale.z);
    }
  }

  protected virtual void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("PlayerAttack"))       // 공격과 부딪혔을 경우
    {

      Vector2 dir = (collision.transform.position - player.transform.position).normalized;    // 밀어낼 방향

      rb.AddForce(dir * pc.knockBackForce, ForceMode2D.Impulse);             // 넉백
      StartCoroutine(StopEnemy(rb));                            // 0.2초후 정지

      //여기서부터 슬라이더 체력깎는 관련코드
      BulletController bc = collision.GetComponent<BulletController>();   // 계수를 가져오기 위해 불릿 컨트롤러 가져오기

      if (bc == null || bc.hasHit) return;  // 이미 피격한 총알이라면 피격효과 무시

      bc.hasHit = true;  // 피격 처리 표시

      //발사체 삭제
      Destroy(collision.gameObject);

      float damage = pc.attackDem * bc.damageCoefficient;                 // 받는 피해량 = 플레이어 공격력 * 공격의 계수

      HitEnemy(damage); // 데미지 적용
      
    }
  }

  public virtual void Dead()
  {
    isDead = true; //이동막기용
    currentHp = 0; //마이너스로 내려가는거 방지
    rb.velocity = Vector2.zero; //넉백멈춤
    rb.simulated = false; //물리 비활성화
    animator.SetTrigger("Dead"); //Dead 애니메이션 재생

    //효과음재생
    if (audioSource != null)
    {
      string prefabName = gameObject.name; // 현재 오브젝트 이름
      if (prefabName.Contains("EliteBoss1_Cat")) // 프리팹이름이 엘리트보스1이면
      {
        audioSource.PlayOneShot(SFXManager.instance.catKill);
      }
      else if (prefabName.Contains("Stage3Boss") || prefabName.Contains("EliteBoss3"))// 엘리트 보스3 이나 최종보스면
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
    // 데미지 계산
    float finalDamage = Mathf.RoundToInt(damage - (damage * (defence - (defence * (pc.defIgnore * 0.01f))) * 0.01f)); 
    bool isCritical = Random.value < (pc.criticalChance / 100f);    // 치명타 확인

    // 치명타라면
    if (isCritical)
    {
      finalDamage *= pc.criticalDem;     // 데미지 적용
    }


    currentHp -= Mathf.RoundToInt(finalDamage);          //피해 적용
    slider.value = currentHp; //슬라이더갱신
    StartCoroutine(MaterialShift());



    // 데미지 텍스트 생성
    GameObject dmgText = Instantiate(damageTextPrefab, transform.position + Vector3.up, Quaternion.identity);
    dmgText.GetComponent<DamageTextController>().SetDamage(finalDamage, isCritical);   // 데미지, 치명타 여부 전달

    if (currentHp <= 0)
      Dead();

   
  }

  // 적 정지 메서드
  protected IEnumerator StopEnemy(Rigidbody2D rb)
  {
    yield return new WaitForSeconds(0.05f);  // 0.05초 후에
    rb.velocity = Vector2.zero;             // 정지
  }

  //적 개체 없어지는 이벤트용 함수
  public virtual void EnemyDie()
  {
    // 강제 사망이 아닌 경우 경험치 드랍
    if(!forceDie)
    {
      DropObject();      // 경험치 오브젝트 드랍
      pc.killCount++;       // 처치 횟수 추가
      pc.KillCountUpdate(); // 처치 횟수 표시 업데이트
    }

    Destroy(gameObject); //적 삭제

  }

  // 오브젝트 드랍
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
    GetComponent<SpriteRenderer>().material = hitMat;   // 마테리얼 변경(빨간색 깜빡임 효과)
    yield return new WaitForSeconds(0.15f); // 0.15초
    GetComponent<SpriteRenderer>().material = originMat;  // 마테리얼 복귀
  }

  // 강제 사망
  public void ForceDie()
  {
    forceDie = true;
    Dead();
  }
}
