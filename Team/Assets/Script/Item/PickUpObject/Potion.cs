using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
  public PlayerController pc;
  private AudioSource audioSource;

  // Start is called before the first frame update
  void Start()
  {
    pc = GameObject.Find("Player").GetComponent<PlayerController>();
    audioSource = GetComponent<AudioSource>(); //오디오 컴포넌트 가져오기
    Destroy(gameObject, 10f);
  }


  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      //효과음 재생
      PlayGetItem();

      // 플레이어와 접촉시 체력 30% 회복 후 스스로를 삭제
      pc.currentHp = Mathf.Min(pc.currentHp + pc.maxHp * 0.3f, pc.maxHp);
      pc.hpSlider.value = pc.currentHp;
      Destroy(gameObject);
    }
    else
        return;
  }

  private void PlayGetItem()
  {
    if(audioSource != null && SFXManager.instance != null && SFXManager.instance.item != null)
    {
      audioSource.PlayOneShot(SFXManager.instance.item);
    }
  }
}
