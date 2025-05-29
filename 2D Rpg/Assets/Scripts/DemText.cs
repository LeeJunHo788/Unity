using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemText : MonoBehaviour
{
  public float moveSpeed = 0.3f;    // �ؽ�Ʈ �̵��ӵ�
  public float lifeTime = 0.5f;     // �ؽ�Ʈ ���� �ð�
  
  public Text demText;

  private static DemText instance;


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

  }

  void Start()
  {
    StartCoroutine(MoveUpAndFade());
  }

  public void SetText(int damage)
  {
    
    demText.text = damage.ToString();
  }

  IEnumerator MoveUpAndFade()
  {
    float elapsed = 0f;
    Vector3 startPos = transform.position;

    while (elapsed < lifeTime)
    {
      elapsed += Time.deltaTime;
      transform.position = startPos + Vector3.up * moveSpeed * (elapsed / lifeTime);
      yield return null;
    }

    Destroy(gameObject);  // �� ������ �ڵ����� ����!
  }
}
