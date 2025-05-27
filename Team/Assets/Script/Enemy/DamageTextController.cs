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
    transform.position += Vector3.up * floatSpeed * Time.deltaTime;  // ���� ��¦ ��������
  }

  public void SetDamage(float damage, bool isCritical)
  {
    string damageText = Mathf.RoundToInt(damage).ToString();

    // ġ��Ÿ �߻� ��
    if (isCritical)
    {
      text.color = Color.red;      // ���������� ����
      text.fontSize *= 1.1f;       // ũ�� Ȯ��(����)
      damageText += "!";             // ���� �ڿ� ����ǥ �߰�
    }

    text.text = damageText;    // ��ġ ǥ��
    Destroy(gameObject, lifeTime);  // ���� �ð� �� ����
  }
}
