using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CharacterDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  public string description; // 이 캐릭터에 대한 설명
  public TextMeshProUGUI descriptionText; // 하단 텍스트 출력할 곳

  public void OnPointerEnter(PointerEventData eventData)
  {
    descriptionText.text = description;
  }



  public void OnPointerExit(PointerEventData eventData)
  {
    descriptionText.text = "캐릭터를 선택해주세요."; // 마우스 빠지면 비우기
  }
}