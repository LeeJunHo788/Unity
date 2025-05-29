using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 

public class ShopManager : MonoBehaviour
{
  public GameObject shopUI; // 상점 UI 패널
  public ItemDatabase itemDatabase; // 모든 아이템 데이터가 들어 있는 데이터베이스
  public ItemData[] selectedItems = new ItemData[3]; // 상점에 등장할 아이템 3개

  public GameObject[] items; // 상점에 등장할 아이템 배열
  private List<ItemData> usedItems = new List<ItemData>(); // 이전에 등장한 아이템 저장 리스트

  private Text[] itemNames; // 아이템 목록 이름 텍스트 배열
  private Text[] itemInfo;  // 아이템 목록 설명 배열
  private int[] itemPrices; // 아이템 가격 배열
  private Image[] itemIcons; // 아이템 아이콘 UI
  public Image selectionArrow; // 선택 화살표 이미지
  private Vector3 arrowOffset = new Vector3(200f, -30, 0); // 화살표 위치


  Text enterText;    // 입장 메세지 나타낼 텍스트
  Image background;  // 텍스트 뒤에 깔리는 배경


  private int selectedItemIndex = 0; // 선택된 아이템 인덱스
  bool isPlayerInRange;    // 활성화 여부

  int restockCount = 1;    // 상점 새로고침 횟수
  int restockPrice;        // 새로고침 가격
  Text restockText;        // 상점 새로고침 텍스트

  int buyCount = 1;        // 포션 구매 횟수
  int potionPrice = 15;         // 포션 가격
  Text potionText;         // 포션 텍스트

  Text message;            // 상점 문구 텍스트
  bool isMessageShaking = false;        // 메세지 떨림 상태
  bool canBuy = true;     // 상점을 연 직후인지 체크하는 용도

  private bool[] purchasedItems; // 각 아이템의 구매 여부를 저장하는 배열


  GameObject gameManager; //  아이템 매니저가 붙어있는 게임메니저 오브젝트
  ItemManager itemManager;

  GameObject player;      // 플레이어 객체
  PlayerController pc;


  void Start()
  {
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

    enterText = GetComponentInChildren<Text>();     // 입장 텍스트 가져오기
    background = GetComponentInChildren<Image>();   // 배경 이미지 가져오기


    message = GameObject.Find("Text_InfoMessage").GetComponent<Text>();
    restockText = GameObject.Find("Text_Restock").GetComponent<Text>();
    restockPrice = 10 * (2 * restockCount);
    restockText.text = $"품목 바꾸기 R \n ({restockPrice} G) ";

    potionText = GameObject.Find("Text_PotionBuy").GetComponent<Text>();
    potionText.text = $"포션 구매 H \n ({potionPrice} G) ";


    // 배열 초기화
    itemNames = new Text[items.Length];
    itemInfo = new Text[items.Length];
    itemPrices = new int[items.Length];
    itemIcons = new Image[items.Length];

    // 구매된 아이템 확인 배열 초기화
    purchasedItems = new bool[items.Length];
    for (int i = 0; i < purchasedItems.Length; i++)
    {
      purchasedItems[i] = false;
    }


    SelectRandomItems(); // 랜덤 아이템 선택
    UpdateShopUI(); // UI 업데이트

    // 게임 오브젝트로부터 아이템 매니저 스크립트 가져오기
    gameManager = GameObject.Find("GameManager");
    itemManager = gameManager.GetComponent<ItemManager>();

    message.gameObject.SetActive(false);
    shopUI.SetActive(false);

  }

  void Update()
  {
    if (pc == null)
      return; // 아직 초기화가 안 된 경우 아무 것도 안함

    if (pc.isPaused)
      return;

    CheckPlayerInRange(); // 플레이어가 범위 안에 있는지 확인

    if (isPlayerInRange)    // 범위내에 플레이어가 있으면
    {
      background.gameObject.SetActive(true);   // 배경 활성화

      // 텍스트 업데이트
      enterText.text = "상점에 입장합니다.\nEnter : 확인";
      enterText.gameObject.SetActive(true);

    }

    else
    {
      background.gameObject.SetActive(false);   // 배경 비활성화
      enterText.gameObject.SetActive(false);   // 텍스트 비활성화
    }

    // 엔터키를 누르면 상점 입장
    if (isPlayerInRange && Input.GetKeyDown(KeyCode.Return) && canBuy)
    {
      OpenShop();
    }

    if (shopUI.activeSelf)
    {
      HandleShopInput();
    }

    if (shopUI.activeSelf && Input.GetKeyDown(KeyCode.R))
    {
      ReStockItems();
    }

    if (shopUI.activeSelf && Input.GetKeyDown(KeyCode.H))
    {
      BuyPotion();
    }

  }

  // 플레이어 감지 메서드
  void CheckPlayerInRange()
  {
    isPlayerInRange = false;
    Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1.0f);

    foreach (Collider2D col in cols)
    {
      if (col != null && col.CompareTag("Player"))
      {
        isPlayerInRange = true;
        break;
      }
    }
  }

  // 랜덤 아이템 선택 메서드
  void SelectRandomItems()
  {
    List<ItemData> availableItems = new List<ItemData>(itemDatabase.shopItems); // 상점 아이템 리스트 가져오기

    // 이전에 등장했던 아이템은 제거
    availableItems.RemoveAll(item => usedItems.Contains(item));

    selectedItems = new ItemData[3]; // 기존 아이템 초기화

    // 만약 선택 가능한 아이템이 부족하면 usedItems 초기화 (모든 아이템이 등장한 경우)
    if (availableItems.Count < 3)
    {
      usedItems.Clear(); // 사용된 아이템 리스트 초기화
      availableItems = new List<ItemData>(itemDatabase.shopItems); // 전체 아이템 다시 가져오기
    }

    for (int i = 0; i < 3; i++)
    {
      if (availableItems.Count == 0) break; // 아이템이 부족하면 중단

      int randomIndex = Random.Range(0, availableItems.Count); // 랜덤 인덱스 선택
      selectedItems[i] = availableItems[randomIndex]; // 선택한 아이템 저장
      usedItems.Add(selectedItems[i]); // 새로 선택된 아이템을 usedItems 리스트에 추가 (중복 방지)

      availableItems.RemoveAt(randomIndex); // 중복 방지를 위해 제거

    }
  }

  // 아이템의 정보를 불러와서 상점 UI에 표시하는 메서드
  void UpdateShopUI()
  {
    for (int i = 0; i < selectedItems.Length; i++)
    {
      if (selectedItems[i] != null)
      {
        itemNames[i] = items[i].transform.Find("Item_Name")?.GetComponent<Text>();
        itemInfo[i] = items[i].transform.Find("Item_Info")?.GetComponent<Text>();
        itemIcons[i] = items[i].transform.Find("Item_Sprite")?.GetComponent<Image>();

        itemNames[i].text = selectedItems[i].itemName;
        itemInfo[i].text = selectedItems[i].info;
        itemPrices[i] = selectedItems[i].price;
        itemIcons[i].sprite = selectedItems[i].sprite;

        items[i].transform.Find("Item_Price").GetComponent<Text>().text = itemPrices[i].ToString() + "G";

        itemIcons[i].preserveAspect = true;  // 원본 비율 유지
        itemIcons[i].transform.localScale = transform.localScale * 0.8f;  // 0.8배로 축소
      }

      if (purchasedItems[i])
      {
        // 텍스트 색상을 회색으로 변경
        itemNames[i].color = Color.gray;
        itemInfo[i].color = Color.gray;
      }

    }
  }


  // 상점 입장 메서드
  void OpenShop()
  {
    shopUI.SetActive(true);
    Time.timeScale = 0f; // 게임 일시 정지
    pc.inShop = true;     // 상점 입장 상태 

  }

  // 상점 퇴장 메서드
  void CloseShop()
  {
    shopUI.SetActive(false);
    Time.timeScale = 1f; // 게임 재개
    canBuy = true;  // 상점을 닫으면 true;
    pc.inShop = false;    // 상점 퇴장
  }

  // 아이템 리스톡 메서드
  void ReStockItems()
  {
    if (GoldManager.instance.currentGold >= restockPrice)
    {
      GoldManager.instance.UseGold(restockPrice);     // 골드 소비

      // 재배치당 골드 증가
      restockPrice = 20 * (int)Mathf.Pow(2, restockCount);
      restockText.text = $"품목 바꾸기 R \n ({restockPrice} G)";

      SelectRandomItems(); // 새로운 아이템 선택
      UpdateShopUI(); // UI 업데이트
      restockCount++; // 횟수 카운트

      // 구매 여부 false
      purchasedItems = new bool[3];
      for (int i = 0; i < purchasedItems.Length; i++)
      {
        purchasedItems[i] = false;
      }

      StartCoroutine(DropAndReturn(restockText));
      UpdateItemSelection();
    }

    else
    {
      warningMessage("소지금이 부족합니다 !");
    }

  }

  void BuyPotion()
  {
    if (GoldManager.instance.currentGold >= potionPrice)
    {
      GoldManager.instance.UseGold(potionPrice);

      potionPrice = (buyCount + 1) * 15;
      potionText.text = $"포션 구매 H \n ({potionPrice} G)";

      pc.potion++;
      buyCount++;
      pc.GetPotion();
      StartCoroutine(DropAndReturn(potionText));
    }

    else
    {
      warningMessage("소지금이 부족합니다 !");
    }

  }

  // 품목 강조표시 메서드
  void HandleShopInput()
  {
    // 상점을 연 직후에는 첫 번째 엔터 입력을 무시
    if (canBuy && Input.GetKeyDown(KeyCode.Return))
    {
      canBuy = false;  // 첫 엔터 입력을 소모하고 false
      return;
    }

    // 방향키로 선택 변경
    if (Input.GetKeyDown(KeyCode.UpArrow))
    {
      selectedItemIndex = (selectedItemIndex - 1 + items.Length) % items.Length;
      UpdateItemSelection();
    }
    else if (Input.GetKeyDown(KeyCode.DownArrow))
    {
      selectedItemIndex = (selectedItemIndex + 1) % items.Length;
      UpdateItemSelection();
    }


    // 엔터 키로 아이템 구매
    if (Input.GetKeyDown(KeyCode.Return))
    {
      BuyItem(selectedItemIndex);

    }

    // ESC 키로 상점 닫기
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      CloseShop();
    }
  }

  void BuyItem(int itemIndex)
  {
    // 구매된 아이템은 구매 불가
    if (purchasedItems[itemIndex])
    {
      warningMessage("이미 구매한 아이템입니다");
      return;
    }

    int price = itemPrices[itemIndex];

    if (GoldManager.instance.currentGold >= price)
    {
      GoldManager.instance.UseGold(price);

      itemManager.ApplyItemEffect(selectedItems[itemIndex]); // 능력치 적용

      // 구매된 것으로 표시
      purchasedItems[itemIndex] = true;

      // UI 업데이트
      itemNames[itemIndex].color = Color.gray;
      itemInfo[itemIndex].color = Color.gray;

      StartCoroutine(DropAndReturnObj(items[itemIndex]));
      

      StartCoroutine(buyItemMessage());
    }
    else
    {
      warningMessage("소지금이 부족합니다 !");
    }
  }

  


  void UpdateItemSelection()
  {
    // 선택된 아이템 강조 (색상 변경)
    for (int i = 0; i < items.Length; i++)
    {
      if (i == selectedItemIndex)
      {
        if (purchasedItems[i])   // 이미 구매된 아이템이라면 색깔 변경없이 볼드체로만 변경
        {
          itemNames[i].fontSize = 34;
          itemInfo[i].fontSize = 34;
          itemNames[i].fontStyle = FontStyle.Bold;
          itemInfo[i].fontStyle = FontStyle.Bold;
        }

        else
        {
          itemNames[i].color = new Color(81f / 255f, 187f / 255f, 255f / 255f, 255f / 255f); // 선택된 아이템은 파란색
          itemInfo[i].color = new Color(81f / 255f, 187f / 255f, 255f / 255f, 255f / 255f); // 선택된 아이템은 파란색
          itemNames[i].fontSize = 34;
          itemInfo[i].fontSize = 34;
          itemNames[i].fontStyle = FontStyle.Bold;
          itemInfo[i].fontStyle = FontStyle.Bold;
        }
        if (selectionArrow != null)
        {
          selectionArrow.gameObject.SetActive(true); // 보이게 설정
          selectionArrow.rectTransform.position = itemNames[i].rectTransform.position + arrowOffset;

        }
      }

      else
      {
        // 구매 상품은 회색
        if (purchasedItems[i])
        {
          itemNames[i].color = Color.gray;
          itemInfo[i].color = Color.gray;
        }
        else
        {
          itemNames[i].color = Color.white; // 나머지는 흰색
          itemInfo[i].color = Color.white; // 나머지는 흰색

        }

        itemNames[i].fontSize = 32;
        itemInfo[i].fontSize = 32;
        itemNames[i].fontStyle = FontStyle.Bold;
        itemInfo[i].fontStyle = FontStyle.Bold;
      }
    }
  }


  IEnumerator buyItemMessage()
  {
    message.gameObject.SetActive(true);
    message.color = new Color(81f / 255f, 187f / 255f, 255f / 255f, 255f / 255f);
    message.text = "구매 완료 !";

    // 2초후 비활성화
    yield return new WaitForSecondsRealtime(2);
    message.gameObject.SetActive(false);
  }

  void warningMessage(string text)
  {
    message.color = Color.red;
    message.text = text;
    if(isMessageShaking == false)
    {
     StartCoroutine(Shake(message));

    }
  }

  // 메세지 떨림 효과
  IEnumerator Shake(Text message)
  {
    isMessageShaking = true;

    message.gameObject.SetActive(true);

    float duration = 0.3f;         // 떨림 지속 시간
    float magnitude = 3f;          // 떨림 강도 (픽셀)
    float speed = 100f;             // 떨림 속도
   

    Vector2 originalPos = message.transform.localPosition;
    float elapsed = 0f;

    elapsed = 0f;

    while (elapsed < duration)
    {
      float offsetY = Mathf.Sin(elapsed * speed) * magnitude; // 상하 떨림
      message.transform.localPosition = originalPos + new Vector2(0, offsetY);

      elapsed += Time.unscaledDeltaTime;
      yield return null;
    }

    message.transform.localPosition = originalPos;

    yield return new WaitForSecondsRealtime(2);
    isMessageShaking = false;
    message.gameObject.SetActive(false);

  }

  IEnumerator DropAndReturn(Text message)
  {
    Vector2 originalPos = message.transform.localPosition;
    float dropAmount = -8f;        // 얼마나 아래로 내릴지
    float duration = 0.1f;          // 내려갔다 올라오는 총 시간
    float halfDuration = duration / 2f;

    float elapsed = 0f;

    // 내려가는 동안
    while (elapsed < halfDuration)
    {
      float t = elapsed / halfDuration;
      message.transform.localPosition = Vector2.Lerp(originalPos, originalPos + new Vector2(0, dropAmount), t);
      elapsed += Time.unscaledDeltaTime;
      yield return null;
    }

    // 위치 보정
    message.transform.localPosition = originalPos + new Vector2(0, dropAmount);

    elapsed = 0f;

    // 올라오는 동안
    while (elapsed < halfDuration)
    {
      float t = elapsed / halfDuration;
      message.transform.localPosition = Vector2.Lerp(originalPos + new Vector2(0, dropAmount), originalPos, t);
      elapsed += Time.unscaledDeltaTime;
      yield return null;
    }

    // 원래 위치로 보정
    message.transform.localPosition = originalPos;
  }

  IEnumerator DropAndReturnObj(GameObject message)
  {
    Vector2 originalPos = message.transform.localPosition;
    float dropAmount = -8f;        // 얼마나 아래로 내릴지
    float duration = 0.1f;          // 내려갔다 올라오는 총 시간
    float halfDuration = duration / 2f;

    float elapsed = 0f;

    // 내려가는 동안
    while (elapsed < halfDuration)
    {
      float t = elapsed / halfDuration;
      message.transform.localPosition = Vector2.Lerp(originalPos, originalPos + new Vector2(0, dropAmount), t);
      elapsed += Time.unscaledDeltaTime;
      yield return null;
    }

    // 위치 보정
    message.transform.localPosition = originalPos + new Vector2(0, dropAmount);

    elapsed = 0f;

    // 올라오는 동안
    while (elapsed < halfDuration)
    {
      float t = elapsed / halfDuration;
      message.transform.localPosition = Vector2.Lerp(originalPos + new Vector2(0, dropAmount), originalPos, t);
      elapsed += Time.unscaledDeltaTime;
      yield return null;
    }

    // 원래 위치로 보정
    message.transform.localPosition = originalPos;
  }


}


