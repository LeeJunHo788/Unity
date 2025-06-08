using UnityEngine;
using TMPro;
using DG.Tweening;

// �� ���� ���� = 1.2 ���� ���� = 0.6
public class Block : MonoBehaviour
{
  TextMeshPro hpText;

  protected float hp;
  protected float def;

  protected virtual void Start()
  {
    hpText = GetComponentInChildren<TextMeshPro>();
    hpText.text = hp.ToString();

    PlayerController.Instance.OnPlayerReady += MoveDown;

  }

  void OnCollisionEnter2D(Collision2D collision)
  {
    // ���� ����� �� ü�� ����
    if (collision.gameObject.CompareTag("Player"))
    {
      Debug.Log("����");

      PlayerController pc = collision.gameObject.GetComponent<PlayerController>();

      float dem = Mathf.RoundToInt(GetRandomAround(pc.att));

      hp -= dem;
      hpText.text = hp.ToString();

      if (hp <= 0)
      {
        Destroy(gameObject);
      }

    }
  }

  // ������ ���� �̱�
  float GetRandomAround(float val)
  {
    return Random.Range(val - (val*0.25f), val + (val * 0.25f));
  }

  // �Ʒ� �̵� �Լ�
  void MoveDown()
  {
    transform.DOMoveY(transform.position.y - 0.6f, 0.35f);
  }

  void OnDestroy()
  {
    PlayerController.Instance.OnPlayerReady -= MoveDown;
  }

}
