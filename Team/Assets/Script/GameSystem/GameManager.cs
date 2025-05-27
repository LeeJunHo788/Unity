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

  [Header("텍스트")]
  public TextMeshProUGUI attackDamageText;
  public TextMeshProUGUI DefenseText;
  public TextMeshProUGUI moveSpeedText;
  public TextMeshProUGUI attackSpeedText;
  public TextMeshProUGUI defIgnoreText;
  public TextMeshProUGUI criticalChanceText;

  

  GameObject player;
  PlayerController pc;
    ItemController itemController;

    //스테이지 클리어 상태 확인 변수(무한모드해금조건)
  public bool clearedStage1 = false;
  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지되게 설정
      SceneManager.sceneLoaded += OnSceneLoaded;
    }
    else
    {
      Destroy(gameObject); // 중복 GameManager가 생기면 삭제
      return;
    }
  }

  private void Start()
  {
    FindPlayer();

    itemController = gameObject.GetComponent<ItemController>();

    selectCanvas.SetActive(false);  // 선택화면 비활성화
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

        
        attackDamageText.text = $"공격력 : {pc.attackDem}";
        DefenseText.text = $"방어력 : {pc.defence}";
        moveSpeedText.text = $"이동 속도 : {pc.moveSpeed}";
        attackSpeedText.text = $"공격 속도 : {pc.attackSpeed}";
        defIgnoreText.text = $"방어력 무시 : {pc.defIgnore}";
        criticalChanceText.text = $"치명타 확률 : {pc.criticalChance}";
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
    Time.timeScale = 0f;    // 게임 일시 정지
        itemController.InitializeButtons();   // 아이템 획득 메서드 호출
        selectCanvas.SetActive(true);   // 선택 화면 활성화

  }

  public void Resume()
  {
    Time.timeScale = 1f;
    selectCanvas.SetActive(false);  // 선택화면 비활성화
  }

  public void PauseToResume()
  {
    Time.timeScale = 1f;
    pauseCanvas.SetActive(false);  // 일시정지 메뉴 비활성화
  }

  public void GameExit()
  {
    SceneManager.LoadScene("StageSelect");
    pauseCanvas.SetActive(false);  // 일시정지 메뉴 비활성화

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
    FindPlayer(); // 새 씬에서 플레이어 다시 찾기
  }

  private void FindPlayer()
  {
    player = GameObject.FindWithTag("Player");

    if (player != null)
      pc = player.GetComponent<PlayerController>();
  }

}
