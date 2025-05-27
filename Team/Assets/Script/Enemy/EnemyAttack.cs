using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

public class EnemyAttack : MonoBehaviour
{
  protected Vector2 direction;
  protected SpawnManager sm;

  // ���� public ������ ��ӹ��� ��ũ��Ʈ���� ���� ���� �ʿ�
  public float speed;           // �ӵ�
  public float lifeTime;        // ���� �ð�
  public float attDamage;       // ������
  public float DefIgnore;       // ���¹���
  public bool destroyOnHit;     // �ǰ� �� ������Ʈ�� ������ ����

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
    //Sprite ȸ���� ������ ����(�⺻���� ������)
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.Euler(0, 0, angle);

  }

  protected virtual void Update()
  {
    transform.position += (Vector3)(direction * speed * Time.deltaTime);
    // transform.Translate(direction * speed * Time.deltaTime);
  }

  
}
