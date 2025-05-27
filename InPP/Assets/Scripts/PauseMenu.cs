using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
  [Header("UI ������Ʈ")]
  public GameObject statMenuUI;
  public GameObject controllerKeyUI;
  public GameObject pauseMenuUI;
  public GameObject quitConfirmPopup;

  [Header("��ư �� ��Ŀ��")]
  public Button firstSelectedButton; // ó�� ���õ� ��ư
  public GameObject yesButton; // Ȯ�� ��ư (�˾�)
  public GameObject attButton; // ���ݷ� ��ư

  [Header("����")]
  public bool isPaused = false;

  [Header("ȿ����")]
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
      yield return null; // ���� �����ӱ��� ���
    }
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();
  }

  void Update()
  {
    if (pc == null)
      return; // ���� �ʱ�ȭ�� �� �� ��� �ƹ� �͵� ����

    if (pc.inShop)
    {
      closedShop = true;
    }


    if (Input.GetKeyDown(KeyCode.Escape))
    {
     
      if (closedShop)
      {
        closedShop = false; // �� ���� ���� ����
        return;
      }

      if (quitConfirmPopup.activeSelf || statMenuUI.activeSelf || controllerKeyUI.activeSelf)
      {
        // �˾��� ���������� �˾� �ݰ� PauseMenu�� ���ư�
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
    // ��Ŀ���� UI�� ���ų�, Raycast �Ұ����� ��� -> ��Ŀ�� ����
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

    // StatButton �ؽ�Ʈ ����
    var statButtons = statMenuUI.GetComponentsInChildren<StatButton>();
    foreach (var statButton in statButtons)
    {
      statButton.UpdateStatText();
    }

    EventSystem.current.SetSelectedGameObject(null); // �ʱ�ȭ
    EventSystem.current.SetSelectedGameObject(attButton); // ù ��ư ����

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

  // ���� Ȯ�� �޼���
  public void ConfirmQuit()
  {
    // DontDestroyOnLoad ��ü �� PersistentObject�� ���� �͵��� ã�� ����
    foreach (PersistentObject obj in FindObjectsOfType<PersistentObject>())
    {
      Destroy(obj.gameObject);
    }

    SceneManager.LoadScene("MainScene");

    Time.timeScale = 1;

  }


  // ���� ��� �޼���
  public void CancelQuit()
  {
    quitConfirmPopup.SetActive(false);
    statMenuUI.SetActive(false);
    controllerKeyUI.SetActive(false);

    // ���̶���Ʈ �ʱ�ȭ
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