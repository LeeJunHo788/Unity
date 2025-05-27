using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CharacterDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  public string description; // �� ĳ���Ϳ� ���� ����
  public TextMeshProUGUI descriptionText; // �ϴ� �ؽ�Ʈ ����� ��

  public void OnPointerEnter(PointerEventData eventData)
  {
    descriptionText.text = description;
  }



  public void OnPointerExit(PointerEventData eventData)
  {
    descriptionText.text = "ĳ���͸� �������ּ���."; // ���콺 ������ ����
  }
}