using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/*// 아이템 -> 플레이어 주변에 생성되어 적을 공격하는 오브젝트
// 플레이어의 아이템 슬롯이 전부 사용되면 아이템 선택지에는 사용중인 아이템만 생성됨
// 아이템을 중복 획득시 아이템이 업그레이드 되며 성능이 강해짐
// 장비 -> 게임 중 획득 시 플레이어의 스탯을 상승시킴
// 단순 스탯이 상승하는 것과는 다르게 특정 스탯을 +하면서, 다른 스탯을 -하는 것도있고
// 기존 스탯에 비례하여 상승하는 것도 있음
// 단순 스탯 상승은 플레이어의 레벨이 상승할 때 마다 선택지 생성
// 이 스탯상승은 그냥 +계산만 하는 것임
// 아이템이나 장비는 플레이어의 레빌이 5의 배수로 상승할 때 선택지 생성
// ex) 레벨 1,2,3,4-> 스탯상승선택지 생성 
// 레벨 5-> 아이템, 장비 선택지 생성
// 레벨 6,7,8,9-> 스탯상승선택지 생성
// 레벨 10 -> 아이템, 장비  선택지 생성*/

public enum ItemType
{
  Item, // 아이템
  Equipment, // 장비
  Status // 스탯상승
}
public struct Choice // 선택지 
{
  public ItemType itemType; // 아이템 타입
  public int itemIndex; // 아이템 인덱스
  public int equipmentIndex; // 장비 인덱스
  public int statusIndex; // 스탯 인덱스
}
public class ItemController : MonoBehaviour
{
  public GameObject[] itemPrefabs; // 아이템 프리팹 목록    
  public GameObject[] equipmentPrefabs; // 장비 프리팹 목록  
  public Sprite[] statupSprite; // 스탯 상승 스프라이트  
  public Transform itemPosition; // 아이템 위치    
  public Transform itemButton; // 버튼 


  private GameObject player;
  private int[] selectedItem; // 선택된 아이템 인덱스 배열    
  private List<Transform> availableTransforms; // 사용 가능한 트랜스폼 리스트
  private List<Transform> usedTransforms; // 사용 가능한 트랜스폼 리스트
  private List<Button> itemButtons; // 버튼 리스트
  private List<Itembase> existiedItem; // 기존 아이템 리스트
  private List<Equipmentbase> existiedEquipment; // 기존 장비 리스트
  private PlayerController playerController; // 플레이어 컨트롤러 스크립트(스텟상승을 위해 사용함)
  private EnemyController enemyController; // 적 컨트롤러 스크립트(적 디버프를 위해 사용)
  private Choice[] selectedChoices; // 선택된 아이템, 장비, 스탯 인덱스 배열

  // 여기는 아이템 UI관련 변수들입니다.
  public GameObject itemSlot;// 아이템 슬롯 UI
  public GameObject equipmentSlot; // 장비 슬롯 UI
  private Image[] itemSlots; // 아이템 슬롯 이미지 배열
  private TextMeshProUGUI[] itemlevelTexts; // 아이템 레벨 텍스트 배열
  private Image[] equipmentSlots; // 장비 슬롯 이미지 배열


  private void Start()
  {
    player = GameObject.FindGameObjectWithTag("Player");
    itemSlot = player.transform.Find("ItemPosition").GetComponent<GameObject>();  

    availableTransforms = new List<Transform>(); // 사용 가능한 트랜스폼 리스트 초기화
    usedTransforms = new List<Transform>(); // 사용 가능한 트랜스폼 리스트 초기화
    foreach (Transform child in itemPosition)
    {
      availableTransforms.Add(child); // 아이템 포지션의 자식 트랜스폼 추가
    }
   
        GameObject mainUI = GameObject.Find("MainUI");       
        Transform itemTransform = mainUI.transform.Find("Item_Image");
        itemSlot = itemTransform.gameObject;

    existiedItem = new List<Itembase>(); // 기존 아이템 리스트 초기화
    existiedEquipment = new List<Equipmentbase>(); // 기존 장비 리스트 초기화
    playerController = FindObjectOfType<PlayerController>(); // 플레이어 컨트롤러 스크립트 가져오기
    enemyController = FindObjectOfType<EnemyController>(); // 에너미 컨트롤러 스크립트 가져오기
    InitializeItemUI(); // 아이템 UI 초기화
    InitializeEquipmentUI(); // 장비 UI 초기화
  }
  /*    private void FixedUpdate()
      {
              InitializeButtons();       
      }*/// --> Game Manager GetItem() 메서드에서 호출함

  // InitializeButtons() 메서드는 아이템 선택지 UI를 초기화하는 메서드  
  public void InitializeButtons()
  {
    itemButtons = new List<Button>(); // 버튼 리스트 초기화
    foreach (Transform child in itemButton)
    {
      Button button = child.GetComponent<Button>(); // 버튼 컴포넌트 가져오기
      if (button != null)
      {
        itemButtons.Add(button);// 버튼 리스트에 추가
      }
    }
    selectedChoices = new Choice[itemButtons.Count]; // 선택된 아이템, 장비, 스탯 인덱스 배열 초기화

    if (playerController.level == 1 || playerController.level % 5 == 0) // 레벨 1 또는 5의 배수일 때
    {
      OnItemOrEquipmentChoices(); // 아이템 또는 장비 선택지 생성
    }
    else // 레벨 2,3,4,6,7,8,9일 때
    {
      OnStatusChoices(); // 스탯 상승 선택지 생성
    }
  }
    private void OnItemOrEquipmentChoices()
    {
        List<int> usedIndices = new List<int>(); // 중복 방지 리스트

        // 이미 사용된 아이템 인덱스 추출
        List<int> existingItemIndices = new List<int>();
        foreach (Transform transform in usedTransforms)
        {
            if (transform.childCount > 0) // 트랜스폼에 자식 객체가 있는 경우
            {
                GameObject child = transform.GetChild(0).gameObject;
                Itembase itemBase = child.GetComponent<Itembase>();
                if (itemBase != null && !existingItemIndices.Contains(itemBase.ItemIndex))
                {
                    existingItemIndices.Add(itemBase.ItemIndex); // 이미 사용된 아이템 인덱스 추가
                }
            }
        }

        for (int i = 0; i < itemButtons.Count; i++)
        {
            if (i % 2 == 0) // 아이템 선택지
            {
                if (availableTransforms.Count == 0) // 트랜스폼이 모두 사용된 경우
                {
                    if (existingItemIndices.Count > 0) // 사용 중인 아이템이 있으면
                    {
                        int randomItemIndex;
                        do
                        {
                            randomItemIndex = existingItemIndices[Random.Range(0, existingItemIndices.Count)];
                        } while (usedIndices.Contains(randomItemIndex)); // 중복 방지

                        usedIndices.Add(randomItemIndex);

                        string explain = existiedItem.Find(item => item.ItemIndex == randomItemIndex)?.itemexplain ?? "아이템 설명 없음";
                        Sprite itemSprite = GetItemSprite(itemPrefabs[randomItemIndex]);
                        UpdateButtonUI(itemButtons[i], itemSprite, explain, () => OnItemSelected(randomItemIndex));
                    }
                    else
                    {
                        Debug.LogWarning("사용 가능한 아이템이 없습니다.");
                    }
                }
                else // 트랜스폼이 남아 있는 경우
                {
                    int randomItemIndex; // 랜덤 아이템 인덱스
                    do
                    {
                        randomItemIndex = Random.Range(0, itemPrefabs.Length);
                    } while (usedIndices.Contains(randomItemIndex)); // 중복 방지

                    usedIndices.Add(randomItemIndex);

                    selectedChoices[i] = new Choice
                    {
                        itemType = ItemType.Item,
                        itemIndex = randomItemIndex,
                        equipmentIndex = -1,
                        statusIndex = -1
                    };

                    GameObject itemPrefab = itemPrefabs[randomItemIndex];
                    Itembase item = itemPrefab.GetComponent<Itembase>();
                    string explain = item != null ? item.itemexplain : "아이템 설명 없음";
                    Sprite itemSprite = GetItemSprite(itemPrefab);
                    UpdateButtonUI(itemButtons[i], itemSprite, explain, () => OnItemSelected(randomItemIndex));
                }
            }
            else // 장비 선택지
            {
                int randomEquipmentIndex;
                do
                {
                    randomEquipmentIndex = Random.Range(0, equipmentPrefabs.Length);
                } while (usedIndices.Contains(randomEquipmentIndex)); // 중복 방지

                usedIndices.Add(randomEquipmentIndex);

                selectedChoices[i] = new Choice
                {
                    itemType = ItemType.Equipment,
                    itemIndex = -1,
                    equipmentIndex = randomEquipmentIndex,
                    statusIndex = -1
                };

                GameObject equipmentPrefab = equipmentPrefabs[randomEquipmentIndex];
                Equipmentbase equipment = equipmentPrefab.GetComponent<Equipmentbase>();
                string explain = equipment != null ? equipment.equipmentexplain : "장비 설명 없음";
                Sprite equipmentSprite = GetEquipmentIcon(equipmentPrefab);
                UpdateButtonUI(itemButtons[i], equipmentSprite, explain, () => OnEquipmentSelected(randomEquipmentIndex));
            }
        }
    }
    private void OnStatusChoices()
  {
    List<int> usedIndices = new List<int>(); // 중복 방지 리스트
    string[] statUp = { "공격력 상승", "방어력 상승", "공격속도 상승", "이동속도 상승", "최대체력 상승" }; // 스탯 설명 배열
    for (int i = 0; i < selectedChoices.Length; i++)
    {
      int randomStatusIndex; // 랜덤 스탯 인덱스 변수
      do
      {
        randomStatusIndex = Random.Range(0, 5); // 랜덤 스탯 인덱스 (0~4)
      }
      while (usedIndices.Contains(randomStatusIndex)); // 중복 방지
      usedIndices.Add(randomStatusIndex); // 중복 방지 리스트에 추가
      selectedChoices[i] = new Choice
      {
        itemType = ItemType.Status, // 스탯 타입
        itemIndex = -1, // 아이템 인덱스 초기화
        equipmentIndex = -1, // 장비 인덱스 초기화
        statusIndex = randomStatusIndex // 랜덤 스탯 인덱스
      };
      string explain = statUp[randomStatusIndex]; // 스탯 설명 가져오기
      Sprite statSprite = statupSprite[randomStatusIndex]; // 스탯 스프라이트 가져오기
      UpdateButtonUI(itemButtons[i], statSprite, explain, () => OnStatusSelected(randomStatusIndex)); // 버튼 UI 업데이트
    }
  }

  private void UpdateButtonUI(Button button, Sprite sprite, string explain, UnityEngine.Events.UnityAction onClickAction)
  {
    Image buttonImage = button.GetComponent<Image>(); // 버튼 이미지 컴포넌트 가져오기
    TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>(); // 버튼 텍스트 컴포넌트 가져오기    

    if (sprite != null) // 스프라이트가 null이 아닐 때
    {
      buttonImage.sprite = sprite; // 버튼 이미지 설정
      buttonImage.enabled = true; // 이미지 활성화
    }
    else
    {
      buttonImage.sprite = null; // 버튼 이미지 초기화
      buttonImage.enabled = false; // 이미지 비활성화
    }

    if (buttonText != null)
    {
      buttonText.text = explain; // 버튼 텍스트에 설명 설정
    }

    button.onClick.RemoveAllListeners(); // 기존 리스너 제거
    button.onClick.AddListener(onClickAction); // 클릭 시 동작 추가
  }

  private void OnItemSelected(int itemIndex)
  {
    Itembase existItem = FindExistingItem(itemIndex); // 기존 아이템 찾기
    if (existItem != null)
    {
      existItem.Upgrade(); // 기존 아이템 업그레이드
    }
    else
    {
      if (availableTransforms.Count == 0) // 사용 가능한 아이템 포지션이 없으면 종료
      {
        return;
      }

      int randomtrIndex = Random.Range(0, availableTransforms.Count); // 랜덤으로 아이템 포지션 선택

      Transform selectedTransform = availableTransforms[randomtrIndex]; // 선택된 트랜스폼

      while(usedTransforms.Contains(selectedTransform) )
      {
        randomtrIndex = Random.Range(0, availableTransforms.Count); // 랜덤으로 아이템 포지션 선택
        selectedTransform = availableTransforms[randomtrIndex];
      }
      usedTransforms.Add(selectedTransform);

      GameObject spawnedItem = Instantiate(itemPrefabs[itemIndex], selectedTransform.position, Quaternion.identity); // 아이템 생성
      spawnedItem.name = itemPrefabs[itemIndex].name;
      spawnedItem.transform.SetParent(selectedTransform); // 아이템 포지션의 자식으로 설정해서 항상 플레이어의 주변위치에 있도록 설정
      availableTransforms.RemoveAt(randomtrIndex); // 사용된 아이템 포지션 제거

      Itembase newItem = spawnedItem.GetComponent<Itembase>(); // 생성된 아이템의 Itembase 컴포넌트 가져오기
      newItem.ItemIndex = itemIndex; // 아이템 인덱스 설정
      RegisterItem(newItem); // 아이템 등록
    }
    UpdateItemSlots(); // 아이템 슬롯UI 업데이트        
    GameManager gm = GetComponent<GameManager>(); // 게임 매니저 스크립트 가져오기
    gm.Resume(); // 게임 재개        
  }
  private void OnEquipmentSelected(int equipmentIndex)
  {
    GameObject equipmentPrefab = equipmentPrefabs[equipmentIndex]; // 장비 프리팹 가져오기
    GameObject instantiatedEquipment = Instantiate(equipmentPrefab);
    Equipmentbase newEquipment = instantiatedEquipment.GetComponent<Equipmentbase>(); // 장비 데이터 가져오기

    newEquipment.EquipmentIndex = equipmentIndex; // 장비 인덱스 설정
                                                  //RegisterEquipment(newEquipment); // 장비 등록
    existiedEquipment.Insert(0, newEquipment); // 장비 등록        
    newEquipment.ApplyEffect(playerController); // 장비 효과 적용 (플레이어 버프)
    newEquipment.Enemydebuff(enemyController); // 장비 효과 적용 (적 디버프)


    UpdateEquipmentSlots(); // 장비 슬롯UI 업데이트
    GameManager gm = GetComponent<GameManager>(); // 게임 매니저 스크립트 가져오기
    gm.Resume(); // 게임 재개

  }
  private void OnStatusSelected(int statusIndex)
  {
    switch (statusIndex)
    {
      case 0:
        playerController.attackDem += 5; // 공격력 증가
        break;
      case 1:
        playerController.defence += 3; // 방어력 증가
        break;
      case 2:
        playerController.attackSpeed += 1; // 공격 속도 증가
        break;
      case 3:
        playerController.moveSpeed += 0.2f; // 이동 속도 증가
        break;
      case 4:
        playerController.maxHp += 15; // 최대 체력 증가
        break;
    }
    GameManager gm = GetComponent<GameManager>(); // 게임 매니저 스크립트 가져오기
    gm.Resume(); // 게임 재개
  }
  private void RegisterItem(Itembase item)
  {
    if (!existiedItem.Contains(item)) // 기존 아이템 리스트에 없으면 추가
    {
      existiedItem.Add(item); // 기존 아이템 리스트에 추가
    }
  }
  private void RegisterEquipment(Equipmentbase equipmnet)
  {
    if (!existiedEquipment.Contains(equipmnet)) // 기존 장비 리스트에 없으면 추가
    {
      existiedEquipment.Add(equipmnet); // 기존 장비 리스트에 추가
    }
  }
  private Itembase FindExistingItem(int itemIndex)
  {
    foreach (Itembase item in existiedItem)
    {
      if (item != null && item.ItemIndex == itemIndex) // 아이템 인덱스로 비교
      {
        return item; // 기존 아이템 반환
      }
    }
    return null; // 기존 아이템이 없으면 null 반환
  }
  private Sprite GetItemSprite(GameObject prefab)
  {
    SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 가져오기
    if (spriteRenderer != null)
    {
      return spriteRenderer.sprite; // 스프라이트 반환
    }
    Image image = prefab.GetComponent<Image>(); // 이미지 컴포넌트 가져오기
    if (image != null)
    {
      return image.sprite; // 스프라이트 반환
    }
    return null; // 스프라이트가 없으면 null 반환
  }
  private Sprite GetEquipmentIcon(GameObject prefab)
  {
    Equipmentbase data = prefab.GetComponent<Equipmentbase>(); // 장비 데이터 가져오기
    if (data != null)
    {
      return data.equipmentIcon; // 아이콘 반환
    }
    return null; // 아이콘이 없으면 null 반환
  }

  // 아이템 UI 관련 메서드
  private void InitializeItemUI() // 아이템 UI 초기화
  {
    List<Image> slots = new List<Image>();
    List<TextMeshProUGUI> levelTexts = new List<TextMeshProUGUI>();

    if (itemSlot != null)
    {
      foreach (Transform child in itemSlot.transform)
      {
        Image slot = child.GetComponent<Image>(); // 슬롯 이미지 가져오기
        if (slot != null)
        {
          slots.Add(slot); // 슬롯 리스트에 추가
          TextMeshProUGUI slotText = child.GetComponentInChildren<TextMeshProUGUI>();
          if (slotText != null)
          {
            levelTexts.Add(slotText); // 슬롯 텍스트 리스트에 추가
          }
        }
      }
      itemSlots = slots.ToArray(); // 슬롯 이미지 배열로 변환
      itemlevelTexts = levelTexts.ToArray(); // 슬롯 텍스트 배열로 변환
    }
  }
  private void UpdateItemSlots()
  {
    for (int i = 0; i < itemSlots.Length; i++)
      if (i < existiedItem.Count)
      {
        Sprite itemSprite = GetItemSprite(itemPrefabs[existiedItem[i].ItemIndex]); // 아이템 스프라이트 가져오기
        itemSlots[i].sprite = itemSprite; // 슬롯 이미지 설정
        itemSlots[i].enabled = true; // 슬롯 활성화
        if (itemlevelTexts[i] != null)
        {
          itemlevelTexts[i].text = existiedItem[i].Level.ToString(); // 슬롯 레벨 텍스트 설정
        }
      }
      else
      {
        itemSlots[i].sprite = null; // 슬롯 이미지 초기화
        itemSlots[i].enabled = false; // 슬롯 비활성화
        if (itemlevelTexts[i] != null)
        {
          itemlevelTexts[i].text = ""; // 슬롯 레벨 텍스트 초기화
        }
      }

  }
  private void InitializeEquipmentUI() // 장비 UI 초기화
  {
    List<Image> slots = new List<Image>();
    foreach (Transform child in equipmentSlot.transform)
    {
      Image slot = child.GetComponent<Image>(); // 슬롯 이미지 가져오기
      if (slot != null)
      {
        slots.Add(slot); // 슬롯 리스트에 추가
      }
    }
    equipmentSlots = slots.ToArray(); // 슬롯 이미지 배열로 변환
  }

  private void UpdateEquipmentSlots()
  {
    for (int i = 0; i < equipmentSlots.Length; ++i)
    {
      if (i < existiedEquipment.Count)
      {
        Equipmentbase equipment = existiedEquipment[i]; // 기존 장비 가져오기
        Sprite equipmentSprite = GetEquipmentIcon(equipment.gameObject); // 장비 스프라이트 가져오기
        if (equipmentSprite != null)
        {
          equipmentSlots[i].sprite = equipmentSprite; // 슬롯 이미지 설정
          equipmentSlots[i].enabled = true; // 슬롯 활성화
        }
      }
      else
      {
        equipmentSlots[i].sprite = null; // 슬롯 이미지 초기화
        equipmentSlots[i].enabled = false; // 슬롯 비활성화
      }
    }
  }

  // 씬이 바뀌었을때 호출되는 함수
  private void OnEnable()
  {
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  private void OnDisable()
  {
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    Debug.Log("찾기");
    // 메인 UI찾기
    GameObject mainUI = GameObject.Find("MainUI");
    player = null;
    playerController = null;
    itemPosition = null;
    availableTransforms = new List<Transform>();
    usedTransforms = new List<Transform>();
    itemSlots = null;
    existiedEquipment = new List<Equipmentbase>();

    player = GameObject.FindGameObjectWithTag("Player");

    if(player != null)
    {
      playerController = player.GetComponent<PlayerController>();
      itemPosition = player.transform.Find("ItemPosition").GetComponent<Transform>();

      foreach (Transform child in itemPosition)
      {
        availableTransforms.Add(child); // 아이템 포지션의 자식 트랜스폼 추가
      }
    }

    Transform itemTransform = mainUI.transform.Find("Item_Image");
    itemSlot = itemTransform.gameObject;
    existiedItem = new List<Itembase>(); // 기존 아이템 리스트 초기화
    InitializeItemUI(); // 아이템 UI 초기화
    InitializeEquipmentUI(); // 장비 UI 초기화

  }
}