using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class MainUI : MonoBehaviour
{
  [SerializeField]
  private TMP_InputField nameInputField;

  [SerializeField]
  private CreateRoomManager createRoomManager;

  [SerializeField]
  private GameObject EnterRoomUI;

  TextMeshProUGUI placeholderText;


  private void Start()
  {
    placeholderText = nameInputField.placeholder.GetComponent<TextMeshProUGUI>();
  }

  private void Update()
  {
    if (nameInputField.isFocused)
    {
      placeholderText.enabled = false;
    }
    else
    {
      placeholderText.enabled = string.IsNullOrEmpty(nameInputField.text);
    }
  }

  public void OnClickEnterRoomButton()
  {
    if (nameInputField.text != "")
    {
      NameManager.PlayerName = nameInputField.text;

      EnterRoomUI.gameObject.SetActive(true);
      gameObject.SetActive(false);
    }

    else
    {
      placeholderText.text = "이름을 입력해주세요";
      placeholderText.color = Color.red;
      placeholderText.alpha = 0.7f;
    }
  }

  public void OnClickCreateRoomButton()
  {
    if (nameInputField.text != "")
    {
      NameManager.PlayerName = nameInputField.text;

      // 바로 방 생성 호출
      createRoomManager.CreateRoom();
    }
    else
    {
      placeholderText.text = "이름을 입력해주세요";
      placeholderText.color = Color.red;
      placeholderText.alpha = 0.7f;
    }
  }

}
