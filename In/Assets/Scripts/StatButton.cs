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
    UpdateStatText(); // ó���� �� �� �ؽ�Ʈ ������Ʈ
  }

  public void UpdateStatText()
  {
    if (player == null || buttonText == null) return;

    string label = GetStatName(statType);

    // ���ư��� ��ư�̸� ���� ���� �ؽ�Ʈ�� ǥ��
    if (statType == StatType.Exit)
    {
      buttonText.text = label;
      return;
    }

    string rawValue = GetStatValue(statType).ToString();

    /*
    // �ۼ�Ʈ�� ǥ���� ����
    bool isPercentStat = statType == StatType.DefIgn || statType == StatType.GoldAcq || statType == StatType.ItemDrop;

    string value = isPercentStat ? $"{rawValue:F1}%" : rawValue.ToString(); // �Ҽ��� 1�ڸ����� �ۼ�Ʈ�� ���*/

    buttonText.text = $"{label}  {rawValue}";
  }

  string GetStatName(StatType type)
  {
    switch (type)
    {
      case StatType.Att: return "���ݷ�";
      case StatType.Def: return "����";
      case StatType.DefIgn: return "���� ����";
      case StatType.AttackSpeed: return "���� �ӵ�";
      case StatType.Speed: return "�̵� �ӵ�";
      case StatType.GoldAcq: return "��� ȹ�淮";
      case StatType.ItemDrop: return "������ ȹ�� Ȯ��";
      case StatType.Exit: return "���ư���";
      default: return "";
    }
  }

  string GetStatValue(StatType type)
  {
    switch (type)
    {
      case StatType.Att: return pc.att.ToString("F1");
      case StatType.Def: return pc.def.ToString("F1");
      case StatType.DefIgn: return ((pc.defIgn * 100f).ToString("F1") + "%");
      case StatType.AttackSpeed: return ((pc.attackSpeed * 100f).ToString("F1") + "%");
      case StatType.Speed: return pc.speed.ToString("F1");
      case StatType.GoldAcq: return ((pc.goldAcq * 100f).ToString("F1") + "%");
      case StatType.ItemDrop: return ((pc.itemDrop * 100f).ToString("F1") + "%");
      default: return "";
    }
  }

  // ���õǾ��� �� ���� ���� (���콺 or Ű����)
  public void OnSelect(BaseEventData eventData)
  {
    descriptionUI.ShowStatInfo((int)statType);
  }

  // ���콺 �÷��� �� ���� ����
  public void OnPointerEnter(PointerEventData eventData)
  {
    descriptionUI.ShowStatInfo((int)statType);
  }
}