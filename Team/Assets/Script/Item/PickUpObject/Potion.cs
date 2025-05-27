using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
  public PlayerController pc;

  // Start is called before the first frame update
  void Start()
  {
    pc = GameObject.Find("Player").GetComponent<PlayerController>();
    Destroy(gameObject, 10f);
  }


  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      // 플레이어와 접촉시 체력 30% 회복 후 스스로를 삭제
      pc.currentHp = Mathf.Min(pc.currentHp + pc.maxHp * 0.3f, pc.maxHp);
      pc.hpSlider.value = pc.currentHp;
      Destroy(gameObject);
    }
    else
        return;
  }
}
