using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

public class EnemyAttack : MonoBehaviour
{
  protected Vector2 direction;
  protected SpawnManager sm;

  // 하위 public 변수는 상속받은 스크립트에서 별도 지정 필요
  public float speed;           // 속도
  public float lifeTime;        // 생존 시간
  public float attDamage;       // 데미지
  public float DefIgnore;       // 방어력무시
  public bool destroyOnHit;     // 피격 시 오브젝트를 없앨지 여부

  protected virtual void Awake()
  {
    sm = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    
  }

  public virtual void SetDirection(Vector2 dir)
  {
    SetRotation(dir);
    direction = dir.normalized;
    Destroy(gameObject,lifeTime);
  }

  public void SetRotation(Vector2 dir)
  {
    //Sprite 회전만 별도로 설정(기본방향 오른쪽)
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.Euler(0, 0, angle);

  }

  protected virtual void Update()
  {
    transform.position += (Vector3)(direction * speed * Time.deltaTime);
    // transform.Translate(direction * speed * Time.deltaTime);
  }

  
}
