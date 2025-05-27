using TMPro;
using UnityEngine;

public class StatDescriptionUI : MonoBehaviour
{
  public TextMeshProUGUI statDescriptionText;

  [System.Serializable]
  public class StatInfo
  {
    public string name;
    [TextArea]
    public string description;
  }

  public StatInfo[] statInfos;

  public void ShowStatInfo(int index)
  {
    if (index < 0 || index >= statInfos.Length) return;

    statDescriptionText.text = statInfos[index].description;
  }
}