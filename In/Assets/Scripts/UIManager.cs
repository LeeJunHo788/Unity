using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  GameObject player;        // �÷��̾� ������Ʈ
  PlayerController pc;      // �÷��̾� ��Ʈ�ѷ�

  public Slider playerHpSlider;    // ü�� �����̴�
  public Text playerHpValText;     // ü�� �ؽ�Ʈ

  public Slider playerExpSlider;   // ����ġ �����̴�
  public Text playerExpValText;    // ����ġ �ؽ�Ʈ
  public Text playerLevelText;     // ���� �ؽ�Ʈ

  public Text playerGoldText;      // �� �ؽ�Ʈ
  public Text playerPotionText;    // ���� �ؽ�Ʈ
  public Image potionImage;        // ���� �̹���
  public Slider potionSlider;       // ���� ��Ÿ�� �����̴�

  private static UIManager instance;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject); // �� ���� �� ������Ʈ ����
    }
    else
    {
      Destroy(gameObject); // �ߺ� ����
    }
  }

  void Start()
  {

    StartCoroutine(Starting());
  }

  IEnumerator Starting()
  {
    // �÷��̾�� �÷��̾� ��Ʈ�ѷ� ��������
    while (GameObject.FindWithTag("Player") == null)
    {
      yield return null; // ���� �����ӱ��� ���
    }
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();

    potionSlider.gameObject.SetActive(false);
  }

  private void Update()
  {
    if (pc == null)
      return; // ���� �ʱ�ȭ�� �� �� ��� �ƹ� �͵� ����

  }

  // ������Ʈ Ȱ��ȭ�� ȣ��Ǵ� �Լ�
  private void OnEnable()
  {
    PlayerController.HpChange += UpdateHpUI;    // �̺�Ʈ ����
    PlayerController.PotionChange += UpdatePotionUI;    // �̺�Ʈ ����
    ExpManager.ExpChange += UpdateExpUI;       // �̺�Ʈ ����
    GoldManager.GoldChange += UpdateGoldUI;       // �̺�Ʈ ����
  }

  // ������Ʈ ��Ȱ��ȭ�� ȣ��Ǵ� �Լ�
  private void OnDisable()
  {
    PlayerController.HpChange -= UpdateHpUI;    // �̺�Ʈ ���� ����
    PlayerController.PotionChange -= UpdatePotionUI;    // �̺�Ʈ ����
    ExpManager.ExpChange -= UpdateExpUI;       // �̺�Ʈ ���� ����
    GoldManager.GoldChange -= UpdateGoldUI;       // �̺�Ʈ ���� ����
  }

  // ü�� ������Ʈ
  void UpdateHpUI(int maxHp, int currentHP)
  {
    playerHpSlider.maxValue = maxHp;
    playerHpSlider.value = currentHP;

    playerHpValText.text = currentHP + " / " + maxHp;
  }

  // ���� ������Ʈ
  void UpdatePotionUI(int count)
  {
    playerPotionText.text = count.ToString();

    if (pc != null)
      StartCoroutine(PotionUpdate());
  }

  IEnumerator PotionUpdate()
  {
    // ���� ��ȭ
    potionImage.color = Color.gray;   
    playerPotionText.color = Color.gray;
    
    // �����̴� Ȱ��ȭ, �ʱ�ȭ
    potionSlider.gameObject.SetActive(true);
    potionSlider.value = 0f;

    float elapsed = 0f; // �ð� ��� ����

    while (elapsed < pc.potionCoolTime) // ��Ÿ�� ���� �ݺ�
    { 
      elapsed += Time.deltaTime; //�ð� ����
      potionSlider.value = elapsed / pc.potionCoolTime;  //�����̴� ���� ���
      yield return null; // ���� �����ӱ��� ���
    }

    potionSlider.value = 1f;


    potionImage.color = Color.white;
    playerPotionText.color = Color.black;
    potionSlider.gameObject.SetActive(false);
  }

  // ����ġ ������Ʈ
  void UpdateExpUI(float maxExp, float currentExp, int level)
  {
    playerExpSlider.maxValue = maxExp;
    playerExpSlider.value = currentExp;

    // ����� ǥ��
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
