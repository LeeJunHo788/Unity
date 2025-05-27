using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextController : MonoBehaviour
{
  public TextMeshPro text;    
  public float floatSpeed = 1.5f;
  public float lifeTime = 0.3f;

  private void Start()
  {
    text = GetComponent<TextMeshPro>();
  }

  void Update()
  {
    transform.position += Vector3.up * floatSpeed * Time.deltaTime;  // 위로 살짝 떠오르게
  }

  public void SetDamage(float damage, bool isCritical)
  {
    string damageText = Mathf.RoundToInt(damage).ToString();

    // 치명타 발생 시
    if (isCritical)
    {
      text.color = Color.red;      // 붉은색으로 변경
      text.fontSize *= 1.1f;       // 크기 확대(강조)
      damageText += "!";             // 숫자 뒤에 느낌표 추가
    }

    text.text = damageText;    // 수치 표시
    Destroy(gameObject, lifeTime);  // 일정 시간 뒤 삭제
  }
}
