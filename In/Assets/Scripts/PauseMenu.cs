using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
  [Header("UI 오브젝트")]
  public GameObject statMenuUI;
  public GameObject controllerKeyUI;
  public GameObject pauseMenuUI;
  public GameObject quitConfirmPopup;

  [Header("버튼 및 포커스")]
  public Button firstSelectedButton; // 처음 선택될 버튼
  public GameObject yesButton; // 확인 버튼 (팝업)
  public GameObject attButton; // 공격력 버튼

  [Header("상태")]
  public bool isPaused = false;

  [Header("효과음")]
  public AudioClip pause;
  public AudioClip Unpause;

  AudioSource audioSource;

  GameObject player;
  PlayerController pc;

  bool closedShop = false;

  void Start()
  {
    audioSource = GetComponent<AudioSource>();
    StartCoroutine(Starting());
  }

  IEnumerator Starting()
  {
    while (GameObject.FindWithTag("Player") == null)
    {
      yield return null; // 다음 프레임까지 대기
    }
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();
  }

  void Update()
  {
    if (pc == null)
      return; // 아직 초기화가 안 된 경우 아무 것도 안함

    if (pc.inShop)
    {
      closedShop = true;
    }


    if (Input.GetKeyDown(KeyCode.Escape))
    {
     
      if (closedShop)
      {
        closedShop = false; // 한 번만 막고 해제
        return;
      }

      if (quitConfirmPopup.activeSelf || statMenuUI.activeSelf || controllerKeyUI.activeSelf)
      {
        // 팝업이 켜져있으면 팝업 닫고 PauseMenu로 돌아감
        CancelQuit();
      }
      else if (isPaused)
      {
        Resume();
      }
      else
      {
        Pause();
      }
    }
  }

  void LateUpdate()
  {
    // 포커스된 UI가 없거나, Raycast 불가능한 경우 -> 포커스 제거
    if (EventSystem.current.currentSelectedGameObject == null ||
        !EventSystem.current.currentSelectedGameObject.activeInHierarchy)
    {
      EventSystem.current.SetSelectedGameObject(null);
    }
  }

  public void Pause()
  {
    isPaused = true;
    pc.isPaused = true;
    pauseMenuUI.SetActive(true);
    quitConfirmPopup.SetActive(false);
    Time.timeScale = 0f;

    audioSource.PlayOneShot(pause);

    EventSystem.current.SetSelectedGameObject(null);
    EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
  }

  public void Resume()
  {
    foreach (var btn in FindObjectsOfType<ButtonManager>())
    {
      btn.ForceUnHighlight();
    }

    pauseMenuUI.SetActive(false);
    Time.timeScale = 1f;
    isPaused = false;
    pc.isPaused = false;

    audioSource.PlayOneShot(Unpause);

    EventSystem.current.SetSelectedGameObject(null);

  }

  public void OnStatButton()
  {
    pauseMenuUI.SetActive(false);
    statMenuUI.SetActive(true);

    // StatButton 텍스트 갱신
    var statButtons = statMenuUI.GetComponentsInChildren<StatButton>();
    foreach (var statButton in statButtons)
    {
      statButton.UpdateStatText();
    }

    EventSystem.current.SetSelectedGameObject(null); // 초기화
    EventSystem.current.SetSelectedGameObject(attButton); // 첫 버튼 선택

  }

  public void OnControllKeyButton()
  {
    pauseMenuUI.SetActive(false);
    controllerKeyUI.SetActive(true);


  }

  public void OnQuitButton()
  {
    var pauseButtons = pauseMenuUI.GetComponentsInChildren<ButtonManager>(true);
    foreach (var btn in pauseButtons)
    {
      btn.ForceUnHighlight();
    }
    
    var popupButtons = quitConfirmPopup.GetComponentsInChildren<ButtonManager>(true);
    foreach (var btn in popupButtons)
    {
      btn.ForceUnHighlight();
    }

    quitConfirmPopup.SetActive(true);
    pauseMenuUI.SetActive(false);
    EventSystem.current.SetSelectedGameObject(null);
    EventSystem.current.SetSelectedGameObject(yesButton);
  }

  // 종료 확인 메서드
  public void ConfirmQuit()
  {
    // DontDestroyOnLoad 객체 중 PersistentObject가 붙은 것들을 찾아 삭제
    foreach (PersistentObject obj in FindObjectsOfType<PersistentObject>())
    {
      Destroy(obj.gameObject);
    }

    SceneManager.LoadScene("MainScene");

    Time.timeScale = 1;

  }


  // 종료 취소 메서드
  public void CancelQuit()
  {
    quitConfirmPopup.SetActive(false);
    statMenuUI.SetActive(false);
    controllerKeyUI.SetActive(false);

    // 하이라이트 초기화
    var allButtons = pauseMenuUI.GetComponentsInChildren<ButtonManager>(true);
    foreach (var btn in allButtons)
    {
      btn.ForceUnHighlight();
    }

    var statButtons = statMenuUI.GetComponentsInChildren<ButtonManager>(true);
    foreach (var btn in statButtons)
    {
      btn.ForceUnHighlight();
    }

    pauseMenuUI.SetActive(true);

    
    EventSystem.current.SetSelectedGameObject(null);
    EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);

  }
}