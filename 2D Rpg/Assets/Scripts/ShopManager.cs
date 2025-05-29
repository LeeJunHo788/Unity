using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 

public class ShopManager : MonoBehaviour
{
  public GameObject shopUI; // ���� UI �г�
  public ItemDatabase itemDatabase; // ��� ������ �����Ͱ� ��� �ִ� �����ͺ��̽�
  public ItemData[] selectedItems = new ItemData[3]; // ������ ������ ������ 3��

  public GameObject[] items; // ������ ������ ������ �迭
  private List<ItemData> usedItems = new List<ItemData>(); // ������ ������ ������ ���� ����Ʈ

  private Text[] itemNames; // ������ ��� �̸� �ؽ�Ʈ �迭
  private Text[] itemInfo;  // ������ ��� ���� �迭
  private int[] itemPrices; // ������ ���� �迭
  private Image[] itemIcons; // ������ ������ UI
  public Image selectionArrow; // ���� ȭ��ǥ �̹���
  private Vector3 arrowOffset = new Vector3(200f, -30, 0); // ȭ��ǥ ��ġ


  Text enterText;    // ���� �޼��� ��Ÿ�� �ؽ�Ʈ
  Image background;  // �ؽ�Ʈ �ڿ� �򸮴� ���


  private int selectedItemIndex = 0; // ���õ� ������ �ε���
  bool isPlayerInRange;    // Ȱ��ȭ ����

  int restockCount = 1;    // ���� ���ΰ�ħ Ƚ��
  int restockPrice;        // ���ΰ�ħ ����
  Text restockText;        // ���� ���ΰ�ħ �ؽ�Ʈ

  int buyCount = 1;        // ���� ���� Ƚ��
  int potionPrice = 15;         // ���� ����
  Text potionText;         // ���� �ؽ�Ʈ

  Text message;            // ���� ���� �ؽ�Ʈ
  bool isMessageShaking = false;        // �޼��� ���� ����
  bool canBuy = true;     // ������ �� �������� üũ�ϴ� �뵵

  private bool[] purchasedItems; // �� �������� ���� ���θ� �����ϴ� �迭


  GameObject gameManager; //  ������ �Ŵ����� �پ��ִ� ���Ӹ޴��� ������Ʈ
  ItemManager itemManager;

  GameObject player;      // �÷��̾� ��ü
  PlayerController pc;


  void Start()
  {
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

    enterText = GetComponentInChildren<Text>();     // ���� �ؽ�Ʈ ��������
    background = GetComponentInChildren<Image>();   // ��� �̹��� ��������


    message = GameObject.Find("Text_InfoMessage").GetComponent<Text>();
    restockText = GameObject.Find("Text_Restock").GetComponent<Text>();
    restockPrice = 10 * (2 * restockCount);
    restockText.text = $"ǰ�� �ٲٱ� R \n ({restockPrice} G) ";

    potionText = GameObject.Find("Text_PotionBuy").GetComponent<Text>();
    potionText.text = $"���� ���� H \n ({potionPrice} G) ";


    // �迭 �ʱ�ȭ
    itemNames = new Text[items.Length];
    itemInfo = new Text[items.Length];
    itemPrices = new int[items.Length];
    itemIcons = new Image[items.Length];

    // ���ŵ� ������ Ȯ�� �迭 �ʱ�ȭ
    purchasedItems = new bool[items.Length];
    for (int i = 0; i < purchasedItems.Length; i++)
    {
      purchasedItems[i] = false;
    }


    SelectRandomItems(); // ���� ������ ����
    UpdateShopUI(); // UI ������Ʈ

    // ���� ������Ʈ�κ��� ������ �Ŵ��� ��ũ��Ʈ ��������
    gameManager = GameObject.Find("GameManager");
    itemManager = gameManager.GetComponent<ItemManager>();

    message.gameObject.SetActive(false);
    shopUI.SetActive(false);

  }

  void Update()
  {
    if (pc == null)
      return; // ���� �ʱ�ȭ�� �� �� ��� �ƹ� �͵� ����

    if (pc.isPaused)
      return;

    CheckPlayerInRange(); // �÷��̾ ���� �ȿ� �ִ��� Ȯ��

    if (isPlayerInRange)    // �������� �÷��̾ ������
    {
      background.gameObject.SetActive(true);   // ��� Ȱ��ȭ

      // �ؽ�Ʈ ������Ʈ
      enterText.text = "������ �����մϴ�.\nEnter : Ȯ��";
      enterText.gameObject.SetActive(true);

    }

    else
    {
      background.gameObject.SetActive(false);   // ��� ��Ȱ��ȭ
      enterText.gameObject.SetActive(false);   // �ؽ�Ʈ ��Ȱ��ȭ
    }

    // ����Ű�� ������ ���� ����
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

  // �÷��̾� ���� �޼���
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

  // ���� ������ ���� �޼���
  void SelectRandomItems()
  {
    List<ItemData> availableItems = new List<ItemData>(itemDatabase.shopItems); // ���� ������ ����Ʈ ��������

    // ������ �����ߴ� �������� ����
    availableItems.RemoveAll(item => usedItems.Contains(item));

    selectedItems = new ItemData[3]; // ���� ������ �ʱ�ȭ

    // ���� ���� ������ �������� �����ϸ� usedItems �ʱ�ȭ (��� �������� ������ ���)
    if (availableItems.Count < 3)
    {
      usedItems.Clear(); // ���� ������ ����Ʈ �ʱ�ȭ
      availableItems = new List<ItemData>(itemDatabase.shopItems); // ��ü ������ �ٽ� ��������
    }

    for (int i = 0; i < 3; i++)
    {
      if (availableItems.Count == 0) break; // �������� �����ϸ� �ߴ�

      int randomIndex = Random.Range(0, availableItems.Count); // ���� �ε��� ����
      selectedItems[i] = availableItems[randomIndex]; // ������ ������ ����
      usedItems.Add(selectedItems[i]); // ���� ���õ� �������� usedItems ����Ʈ�� �߰� (�ߺ� ����)

      availableItems.RemoveAt(randomIndex); // �ߺ� ������ ���� ����

    }
  }

  // �������� ������ �ҷ��ͼ� ���� UI�� ǥ���ϴ� �޼���
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

        itemIcons[i].preserveAspect = true;  // ���� ���� ����
        itemIcons[i].transform.localScale = transform.localScale * 0.8f;  // 0.8��� ���
      }

      if (purchasedItems[i])
      {
        // �ؽ�Ʈ ������ ȸ������ ����
        itemNames[i].color = Color.gray;
        itemInfo[i].color = Color.gray;
      }

    }
  }


  // ���� ���� �޼���
  void OpenShop()
  {
    shopUI.SetActive(true);
    Time.timeScale = 0f; // ���� �Ͻ� ����
    pc.inShop = true;     // ���� ���� ���� 

  }

  // ���� ���� �޼���
  void CloseShop()
  {
    shopUI.SetActive(false);
    Time.timeScale = 1f; // ���� �簳
    canBuy = true;  // ������ ������ true;
    pc.inShop = false;    // ���� ����
  }

  // ������ ������ �޼���
  void ReStockItems()
  {
    if (GoldManager.instance.currentGold >= restockPrice)
    {
      GoldManager.instance.UseGold(restockPrice);     // ��� �Һ�

      // ���ġ�� ��� ����
      restockPrice = 20 * (int)Mathf.Pow(2, restockCount);
      restockText.text = $"ǰ�� �ٲٱ� R \n ({restockPrice} G)";

      SelectRandomItems(); // ���ο� ������ ����
      UpdateShopUI(); // UI ������Ʈ
      restockCount++; // Ƚ�� ī��Ʈ

      // ���� ���� false
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
      warningMessage("�������� �����մϴ� !");
    }

  }

  void BuyPotion()
  {
    if (GoldManager.instance.currentGold >= potionPrice)
    {
      GoldManager.instance.UseGold(potionPrice);

      potionPrice = (buyCount + 1) * 15;
      potionText.text = $"���� ���� H \n ({potionPrice} G)";

      pc.potion++;
      buyCount++;
      pc.GetPotion();
      StartCoroutine(DropAndReturn(potionText));
    }

    else
    {
      warningMessage("�������� �����մϴ� !");
    }

  }

  // ǰ�� ����ǥ�� �޼���
  void HandleShopInput()
  {
    // ������ �� ���Ŀ��� ù ��° ���� �Է��� ����
    if (canBuy && Input.GetKeyDown(KeyCode.Return))
    {
      canBuy = false;  // ù ���� �Է��� �Ҹ��ϰ� false
      return;
    }

    // ����Ű�� ���� ����
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


    // ���� Ű�� ������ ����
    if (Input.GetKeyDown(KeyCode.Return))
    {
      BuyItem(selectedItemIndex);

    }

    // ESC Ű�� ���� �ݱ�
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      CloseShop();
    }
  }

  void BuyItem(int itemIndex)
  {
    // ���ŵ� �������� ���� �Ұ�
    if (purchasedItems[itemIndex])
    {
      warningMessage("�̹� ������ �������Դϴ�");
      return;
    }

    int price = itemPrices[itemIndex];

    if (GoldManager.instance.currentGold >= price)
    {
      GoldManager.instance.UseGold(price);

      itemManager.ApplyItemEffect(selectedItems[itemIndex]); // �ɷ�ġ ����

      // ���ŵ� ������ ǥ��
      purchasedItems[itemIndex] = true;

      // UI ������Ʈ
      itemNames[itemIndex].color = Color.gray;
      itemInfo[itemIndex].color = Color.gray;

      StartCoroutine(DropAndReturnObj(items[itemIndex]));
      

      StartCoroutine(buyItemMessage());
    }
    else
    {
      warningMessage("�������� �����մϴ� !");
    }
  }

  


  void UpdateItemSelection()
  {
    // ���õ� ������ ���� (���� ����)
    for (int i = 0; i < items.Length; i++)
    {
      if (i == selectedItemIndex)
      {
        if (purchasedItems[i])   // �̹� ���ŵ� �������̶�� ���� ������� ����ü�θ� ����
        {
          itemNames[i].fontSize = 34;
          itemInfo[i].fontSize = 34;
          itemNames[i].fontStyle = FontStyle.Bold;
          itemInfo[i].fontStyle = FontStyle.Bold;
        }

        else
        {
          itemNames[i].color = new Color(81f / 255f, 187f / 255f, 255f / 255f, 255f / 255f); // ���õ� �������� �Ķ���
          itemInfo[i].color = new Color(81f / 255f, 187f / 255f, 255f / 255f, 255f / 255f); // ���õ� �������� �Ķ���
          itemNames[i].fontSize = 34;
          itemInfo[i].fontSize = 34;
          itemNames[i].fontStyle = FontStyle.Bold;
          itemInfo[i].fontStyle = FontStyle.Bold;
        }
        if (selectionArrow != null)
        {
          selectionArrow.gameObject.SetActive(true); // ���̰� ����
          selectionArrow.rectTransform.position = itemNames[i].rectTransform.position + arrowOffset;

        }
      }

      else
      {
        // ���� ��ǰ�� ȸ��
        if (purchasedItems[i])
        {
          itemNames[i].color = Color.gray;
          itemInfo[i].color = Color.gray;
        }
        else
        {
          itemNames[i].color = Color.white; // �������� ���
          itemInfo[i].color = Color.white; // �������� ���

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
    message.text = "���� �Ϸ� !";

    // 2���� ��Ȱ��ȭ
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

  // �޼��� ���� ȿ��
  IEnumerator Shake(Text message)
  {
    isMessageShaking = true;

    message.gameObject.SetActive(true);

    float duration = 0.3f;         // ���� ���� �ð�
    float magnitude = 3f;          // ���� ���� (�ȼ�)
    float speed = 100f;             // ���� �ӵ�
   

    Vector2 originalPos = message.transform.localPosition;
    float elapsed = 0f;

    elapsed = 0f;

    while (elapsed < duration)
    {
      float offsetY = Mathf.Sin(elapsed * speed) * magnitude; // ���� ����
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
    float dropAmount = -8f;        // �󸶳� �Ʒ��� ������
    float duration = 0.1f;          // �������� �ö���� �� �ð�
    float halfDuration = duration / 2f;

    float elapsed = 0f;

    // �������� ����
    while (elapsed < halfDuration)
    {
      float t = elapsed / halfDuration;
      message.transform.localPosition = Vector2.Lerp(originalPos, originalPos + new Vector2(0, dropAmount), t);
      elapsed += Time.unscaledDeltaTime;
      yield return null;
    }

    // ��ġ ����
    message.transform.localPosition = originalPos + new Vector2(0, dropAmount);

    elapsed = 0f;

    // �ö���� ����
    while (elapsed < halfDuration)
    {
      float t = elapsed / halfDuration;
      message.transform.localPosition = Vector2.Lerp(originalPos + new Vector2(0, dropAmount), originalPos, t);
      elapsed += Time.unscaledDeltaTime;
      yield return null;
    }

    // ���� ��ġ�� ����
    message.transform.localPosition = originalPos;
  }

  IEnumerator DropAndReturnObj(GameObject message)
  {
    Vector2 originalPos = message.transform.localPosition;
    float dropAmount = -8f;        // �󸶳� �Ʒ��� ������
    float duration = 0.1f;          // �������� �ö���� �� �ð�
    float halfDuration = duration / 2f;

    float elapsed = 0f;

    // �������� ����
    while (elapsed < halfDuration)
    {
      float t = elapsed / halfDuration;
      message.transform.localPosition = Vector2.Lerp(originalPos, originalPos + new Vector2(0, dropAmount), t);
      elapsed += Time.unscaledDeltaTime;
      yield return null;
    }

    // ��ġ ����
    message.transform.localPosition = originalPos + new Vector2(0, dropAmount);

    elapsed = 0f;

    // �ö���� ����
    while (elapsed < halfDuration)
    {
      float t = elapsed / halfDuration;
      message.transform.localPosition = Vector2.Lerp(originalPos + new Vector2(0, dropAmount), originalPos, t);
      elapsed += Time.unscaledDeltaTime;
      yield return null;
    }

    // ���� ��ġ�� ����
    message.transform.localPosition = originalPos;
  }


}


