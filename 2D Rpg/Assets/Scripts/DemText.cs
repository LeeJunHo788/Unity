using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemText : MonoBehaviour
{
  public float moveSpeed = 0.3f;    // 텍스트 이동속도
  public float lifeTime = 0.5f;     // 텍스트 생존 시간
  
  public Text demText;

  private static DemText instance;


  protected virtual void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject); // 씬 변경 시 오브젝트 유지
    }
    else
    {
      Destroy(gameObject); // 중복 방지
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

    Destroy(gameObject);  // 다 끝나면 자동으로 제거!
  }
}
