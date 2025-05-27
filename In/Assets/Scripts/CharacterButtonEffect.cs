using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  public GameObject chara; // 캐릭터 이미지
  public Vector3 normalScale = new Vector3(110f, 110f, 1f);
  public Vector3 highlightedScale = new Vector3(130f, 130f, 1f); // 커질 크기

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (chara != null)
      chara.transform.localScale = highlightedScale;
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (chara != null)
      chara.transform.localScale = normalScale;
  }
}
