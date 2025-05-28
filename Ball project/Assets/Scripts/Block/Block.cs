using UnityEngine;
using TMPro;
using DG.Tweening;

// �� ���� ���� = 1.2 ���� ���� = 0.6
public class Block : MonoBehaviour
{
  TextMeshPro hpText;

  float hp;
  float def;

  private void Start()
  {
    hpText = GetComponentInChildren<TextMeshPro>();
    hpText.text = hp.ToString();

    transform.DOMoveY(transform.position.y - 0.6f, 0.35f);
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if(collision.CompareTag("Player"))
    {
      PlayerController pc = collision.GetComponent<PlayerController>();

      float dem = Mathf.RoundToInt(GetRandomAround(pc.att));

      hp -=dem ;
      hpText.text = hp.ToString();

    }
  }

  float GetRandomAround(float val)
  {
    return Random.Range(val - (val*0.25f), val + (val * 0.25f));
  }

}
