using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
  public enum StatType { Att, Def, DefIgn, AttackSpeed, Speed, GoldAcq, ItemDrop, Exit }
  public StatType statType;

  public StatDescriptionUI descriptionUI;
  GameObject player;
  PlayerController pc;

  private TextMeshProUGUI buttonText;

  void Awake()
  {
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();
    buttonText = GetComponentInChildren<TextMeshProUGUI>();
    UpdateStatText(); // 처음에 한 번 텍스트 업데이트
  }

  public void UpdateStatText()
  {
    if (player == null || buttonText == null) return;

    string label = GetStatName(statType);

    // 돌아가기 버튼이면 숫자 없이 텍스트만 표시
    if (statType == StatType.Exit)
    {
      buttonText.text = label;
      return;
    }

    string rawValue = GetStatValue(statType).ToString();

    /*
    // 퍼센트로 표현할 스탯
    bool isPercentStat = statType == StatType.DefIgn || statType == StatType.GoldAcq || statType == StatType.ItemDrop;

    string value = isPercentStat ? $"{rawValue:F1}%" : rawValue.ToString(); // 소수점 1자리까지 퍼센트로 출력*/

    buttonText.text = $"{label}  {rawValue}";
  }

  string GetStatName(StatType type)
  {
    switch (type)
    {
      case StatType.Att: return "공격력";
      case StatType.Def: return "방어력";
      case StatType.DefIgn: return "방어력 무시";
      case StatType.AttackSpeed: return "공격 속도";
      case StatType.Speed: return "이동 속도";
      case StatType.GoldAcq: return "골드 획득량";
      case StatType.ItemDrop: return "아이템 획득 확률";
      case StatType.Exit: return "돌아가기";
      default: return "";
    }
  }

  string GetStatValue(StatType type)
  {
    switch (type)
    {
      case StatType.Att: return pc.att.ToString("F1");
      case StatType.Def: return pc.def.ToString("F1");
      case StatType.DefIgn: return ((pc.defIgn).ToString("F1") + "%");
      case StatType.AttackSpeed: return ((pc.attackSpeed * 100f).ToString("F1") + "%");
      case StatType.Speed: return pc.speed.ToString("F1");
      case StatType.GoldAcq: return ((pc.goldAcq * 100).ToString("F1") + "%");
      case StatType.ItemDrop: return ((pc.itemDrop * 100).ToString("F1") + "%");
      default: return "";
    }
  }

  // 선택되었을 때 설명 띄우기 (마우스 or 키보드)
  public void OnSelect(BaseEventData eventData)
  {
    descriptionUI.ShowStatInfo((int)statType);
  }

  // 마우스 올렸을 때 설명 띄우기
  public void OnPointerEnter(PointerEventData eventData)
  {
    descriptionUI.ShowStatInfo((int)statType);
  }
}