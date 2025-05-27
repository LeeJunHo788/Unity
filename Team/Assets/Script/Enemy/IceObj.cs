using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceObj : EnemyAttack
{
  public Vector3 offset;
  public float fallSpeed;
  public float delayBeforeFall; // ���� ���� �ð�
  public float fallDuration;    // ���� ���� �ð�

  private Vector3 startPos;
  private float timer = 0f;
  private bool isFalling = false;
  private float fallTimer = 0f;

  private void Start()
  {
    startPos = transform.position = transform.parent.position + offset;

    fallSpeed = 3.5f;
    delayBeforeFall = 0.45f; // ���� ���� �ð�
    fallDuration = 0.4f;    // ���� ���� �ð�

    lifeTime = 1.5f;        // ���� �ð�
    attDamage = 10f;       // ������
    DefIgnore = 20f;       // ���¹���
    destroyOnHit = false;     // �ǰ� �� ������Ʈ�� ������ ����

    Destroy(transform.parent.gameObject, 1.5f); // �θ���� ���� �ı�
  }


  protected override void Update()
  {
    if (!isFalling)
    {
      timer += Time.deltaTime;
      if (timer >= delayBeforeFall)
      {
        isFalling = true; // ���� ����
        fallTimer = 0f;   // ���� �ð� ī��Ʈ ����
      }
      else return;
    }

    if (fallTimer < fallDuration)
    {
      fallTimer += Time.deltaTime;
      transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }
  }




}
