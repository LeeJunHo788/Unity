using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
  protected AudioSource audioSource;
  public AudioClip getItemClip;

  Text text_Name;    // 이름 텍스트
  Text text_Effect;  // 효과 텍스트
  Image background;  // 텍스트 뒤에 깔리는 배경

  // 아이템 획득시 스탯 변경 이벤트
  public static event System.Action<float , float, float, float> ItemHpChange;
  public static event System.Action<float, float> ItemAttChange;
  public static event System.Action<float, float> ItemDefChange;
  public static event System.Action<float, float> ItemSpdChange;
  public static event System.Action<float> ItemAttSpdChange;
  public static event System.Action<float> ItemGoldAcqChange;
  public static event System.Action<float> ItemItemDropChange;
  public static event System.Action<float> ItemDefIgnChange;
  public static event System.Action<int> ItemJumpCountChange;

  public ItemData itemData;
 
  // 플레이어 객체와 플레이어 컨트롤러
  GameObject player;
  PlayerController pc;

  void Start()
  {
    audioSource = GetComponent<AudioSource>();
   
    StartCoroutine(Starting());
  }

  IEnumerator Starting()
  {
    // 플레이어 객체와 pc가져오기
    while (GameObject.FindWithTag("Player") == null)
    {
      yield return null; // 다음 프레임까지 대기
    }
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();

    // 텍스트 가져오기
    Text[] texts = GetComponentsInChildren<Text>();
    if (itemData != null)
    {
      foreach (Text text in texts)
      {
        if (text.name == "Text_Name") // 오브젝트 이름이 "Text_Name"일 경우
        {
          text_Name = text;
        }

        else if (text.name == "Text_Effect") // 오브젝트 이름이 "Text_Effect"일 경우
        {
          text_Effect = text;
        }
      }

      // 텍스트 할당
      text_Name.text = itemData.itemName;
      text_Effect.text = itemData.info;

    }

    background = GetComponentInChildren<Image>();   // 배경 이미지 가져오기

    
  }

  private void Update()
  {
    if(text_Name != null && text_Effect != null && background != null)
    {
      if (CheckPlayerInRange())    // 범위내에 플레이어가 있으면
      {
        // 배경 텍스트 활성화
        background.gameObject.SetActive(true);  
        text_Name.gameObject.SetActive(true);
        text_Effect.gameObject.SetActive(true);

      }
      else
      {
        // 배경 텍스트 비활성화
        background.gameObject.SetActive(false);
        text_Name.gameObject.SetActive(false);
        text_Effect.gameObject.SetActive(false);

      }

    }
  }

  // 플레이어 감지 메서드
  bool CheckPlayerInRange()
  {
    Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 2.5f);

    foreach (Collider2D col in cols)
    {
      if (col != null && col.CompareTag("Player"))
      {
        return true;
      }
    }

    return false;
  }

  public void ApplyItemEffect(ItemData item)
  {
    ChangeHp(item.maxHpMul, item.maxHpPlus, item.currentHpMul, item.currentHpPlus);
    ChangeAtt(item.attMul, item.attPlus);
    ChangeDef(item.defMul, item.defPlus);
    ChangeSpd(item.spdMul, item.spdPlus);
    ChangeAttSpd(item.attSpdPlus);
    ChangeGoldAcq(item.goldAcqPlus);
    ChangeItemDrop(item.itemDropPlus);
    ChangeDefIgn(item.defIgnPlus);
    ChangeJumpCount(item.jumpCountPlus);

  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      ApplyItemEffect(itemData);

      GameObject soundObj = new GameObject("ItemSound");
      AudioSource source = soundObj.AddComponent<AudioSource>();
      source.PlayOneShot(getItemClip);
      Destroy(soundObj, getItemClip.length); // 소리 재생 끝나면 삭제

      Destroy(gameObject); // 아이템은 바로 삭제
    }
  }


  public void ChangeHp(float maxHpMulVal, float maxHpPlusVal, float currentHpMulVal, float currentHpPlusVal)
  {

    ItemHpChange?.Invoke(maxHpMulVal, maxHpPlusVal, currentHpMulVal, currentHpPlusVal); // HpChange 이벤트 호출
  }

  public void ChangeAtt(float attMulVal, float attPlusVal)
  {
    ItemAttChange?.Invoke(attMulVal, attPlusVal); // AttChange 이벤트 호출
  }

  public void ChangeDef(float defMulVal, float defPlusVal)
  {
    ItemDefChange?.Invoke(defMulVal, defPlusVal); // DefChange 이벤트 호출
  }

  public void ChangeSpd(float spdMulVal, float spdPlusVal)
  {
    ItemSpdChange?.Invoke(spdMulVal, spdPlusVal); // SpdChange 이벤트 호출
  }

  public void ChangeAttSpd(float attSpdPlusVal)
  {
    
    ItemAttSpdChange?.Invoke(attSpdPlusVal); // AttSpdChange 이벤트 호출
  }

  public void ChangeGoldAcq(float goldAcqPlusVal)
  {
    ItemGoldAcqChange?.Invoke(goldAcqPlusVal); // AttSpdChange 이벤트 호출
  }

  public void ChangeItemDrop(float itemDropPlusVal)
  {
    ItemItemDropChange?.Invoke(itemDropPlusVal); // AttSpdChange 이벤트 호출
  }

  public void ChangeDefIgn(float defIgnPlusVal)
  {
    ItemDefIgnChange?.Invoke(defIgnPlusVal); // AttSpdChange 이벤트 호출
  }

  public void ChangeJumpCount(int jumpCountPlusVal)
  {
    ItemJumpCountChange?.Invoke(jumpCountPlusVal); // AttSpdChange 이벤트 호출
  }
}
