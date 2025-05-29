using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
  public GameObject chara; // 캐릭터 이미지
  public Vector3 normalScale = new Vector3(110f, 110f, 1f);
  public Vector3 highlightedScale = new Vector3(130f, 130f, 1f); // 커질 크기

  public AudioClip clickSound; //  클릭 효과음
  public AudioClip highlightSound; //  하이라이트 효과음
  private AudioSource audioSource; // 효과음 재생용 오디오 소스

  void Start() // ← 15번째 줄: 시작 시 AudioSource 가져오기
  {
    audioSource = GetComponent<AudioSource>();
    if (audioSource == null)
    {
      audioSource = gameObject.AddComponent<AudioSource>();
    }
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (chara != null)
    {
      chara.transform.localScale = highlightedScale;

    }

    if (highlightSound != null && audioSource != null) 
    {
      audioSource.PlayOneShot(highlightSound);
    }
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (chara != null)
      chara.transform.localScale = normalScale;
  }

  public void OnPointerClick(PointerEventData eventData) 
  {
    if (clickSound != null && audioSource != null)
    {
      audioSource.PlayOneShot(clickSound); 
    }
  }
}
