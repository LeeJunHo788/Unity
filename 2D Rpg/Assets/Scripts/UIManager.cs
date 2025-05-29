using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  GameObject player;        // 플레이어 오브젝트
  PlayerController pc;      // 플레이어 컨트롤러

  public Slider playerHpSlider;    // 체력 슬라이더
  public Text playerHpValText;     // 체력 텍스트

  public Slider playerExpSlider;   // 경험치 슬라이더
  public Text playerExpValText;    // 경험치 텍스트
  public Text playerLevelText;     // 레벨 텍스트

  public Text playerGoldText;      // 돈 텍스트
  public Text playerPotionText;    // 포션 텍스트
  public Image potionImage;        // 포션 이미지
  public Slider potionSlider;       // 포션 쿨타임 슬라이더

  private static UIManager instance;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject); // 씬 변경 시 오브젝트 유지
    }
    else
    {
      Destroy(gameObject); // 중복 방지
    }
  }

  void Start()
  {

    StartCoroutine(Starting());
  }

  IEnumerator Starting()
  {
    // 플레이어와 플레이어 컨트롤러 가져오기
    while (GameObject.FindWithTag("Player") == null)
    {
      yield return null; // 다음 프레임까지 대기
    }
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();

    potionSlider.gameObject.SetActive(false);
  }

  private void Update()
  {
    if (pc == null)
      return; // 아직 초기화가 안 된 경우 아무 것도 안함

  }

  // 오브젝트 활성화시 호출되는 함수
  private void OnEnable()
  {
    PlayerController.HpChange += UpdateHpUI;    // 이벤트 구독
    PlayerController.PotionChange += UpdatePotionUI;    // 이벤트 구독
    ExpManager.ExpChange += UpdateExpUI;       // 이벤트 구독
    GoldManager.GoldChange += UpdateGoldUI;       // 이벤트 구독
  }

  // 오브젝트 비활성화시 호출되는 함수
  private void OnDisable()
  {
    PlayerController.HpChange -= UpdateHpUI;    // 이벤트 구독 해제
    PlayerController.PotionChange -= UpdatePotionUI;    // 이벤트 구독
    ExpManager.ExpChange -= UpdateExpUI;       // 이벤트 구독 해제
    GoldManager.GoldChange -= UpdateGoldUI;       // 이벤트 구독 해제
  }

  // 체력 업데이트
  void UpdateHpUI(int maxHp, int currentHP)
  {
    playerHpSlider.maxValue = maxHp;
    playerHpSlider.value = currentHP;

    playerHpValText.text = currentHP + " / " + maxHp;
  }

  // 포션 업데이트
  void UpdatePotionUI(int count)
  {
    playerPotionText.text = count.ToString();

    if (pc != null)
      StartCoroutine(PotionUpdate());
  }

  IEnumerator PotionUpdate()
  {
    // 색깔 변화
    potionImage.color = Color.gray;   
    playerPotionText.color = Color.gray;
    
    // 슬라이더 활성화, 초기화
    potionSlider.gameObject.SetActive(true);
    potionSlider.value = 0f;

    float elapsed = 0f; // 시간 경과 변수

    while (elapsed < pc.potionCoolTime) // 쿨타임 동안 반복
    { 
      elapsed += Time.deltaTime; //시간 누적
      potionSlider.value = elapsed / pc.potionCoolTime;  //슬라이더 비율 계산
      yield return null; // 다음 프레임까지 대기
    }

    potionSlider.value = 1f;


    potionImage.color = Color.white;
    playerPotionText.color = Color.black;
    potionSlider.gameObject.SetActive(false);
  }

  // 경험치 업데이트
  void UpdateExpUI(float maxExp, float currentExp, int level)
  {
    playerExpSlider.maxValue = maxExp;
    playerExpSlider.value = currentExp;

    // 백분율 표시
    float expPer = ((float)currentExp / maxExp ) * 100;
    string expPerText = expPer.ToString("F2") + "%";
    string levelText = "Lv." + level.ToString();


    playerExpValText.text = expPerText;
    playerLevelText.text = levelText;
    
  }

  void UpdateGoldUI(float gold)
  {
    if(playerGoldText == null)
    {
      playerGoldText = GameObject.Find("Text_Gold").GetComponent<Text>();
    }

    string goldText = gold.ToString("F0");

    playerGoldText.text = goldText;
  }
}
