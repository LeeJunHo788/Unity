using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;

  bool isPaused = false;

  public GameObject selectCanvas;
  public GameObject pauseCanvas;

  [Header("�ؽ�Ʈ")]
  public TextMeshProUGUI attackDamageText;
  public TextMeshProUGUI DefenseText;
  public TextMeshProUGUI moveSpeedText;
  public TextMeshProUGUI attackSpeedText;
  public TextMeshProUGUI defIgnoreText;
  public TextMeshProUGUI criticalChanceText;

  

  GameObject player;
  PlayerController pc;
    ItemController itemController;

    //�������� Ŭ���� ���� Ȯ�� ����(���Ѹ���ر�����)
  public bool clearedStage1 = false;
  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); // �� �Ѿ�� �����ǰ� ����
      SceneManager.sceneLoaded += OnSceneLoaded;
    }
    else
    {
      Destroy(gameObject); // �ߺ� GameManager�� ����� ����
      return;
    }
  }

  private void Start()
  {
    FindPlayer();

    itemController = gameObject.GetComponent<ItemController>();

    selectCanvas.SetActive(false);  // ����ȭ�� ��Ȱ��ȭ
    pauseCanvas.SetActive(false);
  }

  private void Update()
  {
    if(Input.GetKeyDown(KeyCode.Escape))
    {
      if (!isPaused)
      {
        isPaused = true;
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);

        
        attackDamageText.text = $"���ݷ� : {pc.attackDem}";
        DefenseText.text = $"���� : {pc.defence}";
        moveSpeedText.text = $"�̵� �ӵ� : {pc.moveSpeed}";
        attackSpeedText.text = $"���� �ӵ� : {pc.attackSpeed}";
        defIgnoreText.text = $"���� ���� : {pc.defIgnore}";
        criticalChanceText.text = $"ġ��Ÿ Ȯ�� : {pc.criticalChance}";
      }
      else
      {
        isPaused = false;
        PauseToResume();
      }
      
    }
  }

  public void GetItem()
  {
    Time.timeScale = 0f;    // ���� �Ͻ� ����
        itemController.InitializeButtons();   // ������ ȹ�� �޼��� ȣ��
        selectCanvas.SetActive(true);   // ���� ȭ�� Ȱ��ȭ

  }

  public void Resume()
  {
    Time.timeScale = 1f;
    selectCanvas.SetActive(false);  // ����ȭ�� ��Ȱ��ȭ
  }

  public void PauseToResume()
  {
    Time.timeScale = 1f;
    pauseCanvas.SetActive(false);  // �Ͻ����� �޴� ��Ȱ��ȭ
  }

  public void GameExit()
  {
    SceneManager.LoadScene("StageSelect");
    pauseCanvas.SetActive(false);  // �Ͻ����� �޴� ��Ȱ��ȭ

    if (pc == null)
    {
      FindPlayer();
    }
    pc.StatInit();

    Time.timeScale = 1f;
    pc.gold = PlayerPrefs.GetInt("Gold");
  }

  private void OnDestroy()
  {
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    FindPlayer(); // �� ������ �÷��̾� �ٽ� ã��
  }

  private void FindPlayer()
  {
    player = GameObject.FindWithTag("Player");

    if (player != null)
      pc = player.GetComponent<PlayerController>();
  }

}
