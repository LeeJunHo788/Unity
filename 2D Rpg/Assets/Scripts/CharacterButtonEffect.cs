using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
  public GameObject chara; // ĳ���� �̹���
  public Vector3 normalScale = new Vector3(110f, 110f, 1f);
  public Vector3 highlightedScale = new Vector3(130f, 130f, 1f); // Ŀ�� ũ��

  public AudioClip clickSound; //  Ŭ�� ȿ����
  public AudioClip highlightSound; //  ���̶���Ʈ ȿ����
  private AudioSource audioSource; // ȿ���� ����� ����� �ҽ�

  void Start() // �� 15��° ��: ���� �� AudioSource ��������
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
