using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
  [SerializeField]
  private InputField nameInputField;

  [SerializeField]
  private GameObject EnterRoomUI;

  Text placeholderText;

  private void Start()
  {
    placeholderText = nameInputField.placeholder.GetComponent<Text>();
  }

  public void OnClickEnterRoomButton()
  {
    if (nameInputField.text != "")
    {
      EnterRoomUI.gameObject.SetActive(true);
      gameObject.SetActive(false);
    }

    else
    {
      placeholderText.text = "이름을 입력해주세요";
      placeholderText.color = Color.red;
    }
  }



    
}
