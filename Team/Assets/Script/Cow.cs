using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : MonoBehaviour
{
  public float moveSpeed = 2f;
  public float moveDistance = 3f;
  public float stopTime = 1f;
  public bool startFacingRight = true; // �ν����Ϳ��� ���� üũ

  private Vector3 startPos;
  private int direction = 1;
  private bool isStopping = false;
  private float stopTimer = 0f;

  private void Start()
  {
    startPos = transform.position;
    direction = startFacingRight ? 1 : -1;
  }

  private void Update()
  {
    if (isStopping)
    {
      stopTimer -= Time.deltaTime;
      if (stopTimer <= 0f)
      {
        isStopping = false;
      }
      return;
    }

    transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

    if (Mathf.Abs(transform.position.x - startPos.x) >= moveDistance)
    {
      direction *= -1;
      Flip();
      isStopping = true;
      stopTimer = stopTime;
    }
  }

  private void Flip()
  {
    Vector3 scale = transform.localScale;

    if (startFacingRight)
    {
      // ������ ���� ������ ���
      scale.x = Mathf.Abs(scale.x) * (direction > 0 ? 1 : -1);
    }
    else
    {
      // ���� ���� ������ ���
      scale.x = -Mathf.Abs(scale.x) * (direction > 0 ? 1 : -1);
    }

    transform.localScale = scale;
  }
}
