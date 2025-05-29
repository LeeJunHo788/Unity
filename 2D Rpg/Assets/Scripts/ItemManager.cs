using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
  protected AudioSource audioSource;
  public AudioClip getItemClip;

  Text text_Name;    // �̸� �ؽ�Ʈ
  Text text_Effect;  // ȿ�� �ؽ�Ʈ
  Image background;  // �ؽ�Ʈ �ڿ� �򸮴� ���

  // ������ ȹ��� ���� ���� �̺�Ʈ
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
 
  // �÷��̾� ��ü�� �÷��̾� ��Ʈ�ѷ�
  GameObject player;
  PlayerController pc;

  void Start()
  {
    audioSource = GetComponent<AudioSource>();
   
    StartCoroutine(Starting());
  }

  IEnumerator Starting()
  {
    // �÷��̾� ��ü�� pc��������
    while (GameObject.FindWithTag("Player") == null)
    {
      yield return null; // ���� �����ӱ��� ���
    }
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();

    // �ؽ�Ʈ ��������
    Text[] texts = GetComponentsInChildren<Text>();
    if (itemData != null)
    {
      foreach (Text text in texts)
      {
        if (text.name == "Text_Name") // ������Ʈ �̸��� "Text_Name"�� ���
        {
          text_Name = text;
        }

        else if (text.name == "Text_Effect") // ������Ʈ �̸��� "Text_Effect"�� ���
        {
          text_Effect = text;
        }
      }

      // �ؽ�Ʈ �Ҵ�
      text_Name.text = itemData.itemName;
      text_Effect.text = itemData.info;

    }

    background = GetComponentInChildren<Image>();   // ��� �̹��� ��������

    
  }

  private void Update()
  {
    if(text_Name != null && text_Effect != null && background != null)
    {
      if (CheckPlayerInRange())    // �������� �÷��̾ ������
      {
        // ��� �ؽ�Ʈ Ȱ��ȭ
        background.gameObject.SetActive(true);  
        text_Name.gameObject.SetActive(true);
        text_Effect.gameObject.SetActive(true);

      }
      else
      {
        // ��� �ؽ�Ʈ ��Ȱ��ȭ
        background.gameObject.SetActive(false);
        text_Name.gameObject.SetActive(false);
        text_Effect.gameObject.SetActive(false);

      }

    }
  }

  // �÷��̾� ���� �޼���
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
      Destroy(soundObj, getItemClip.length); // �Ҹ� ��� ������ ����

      Destroy(gameObject); // �������� �ٷ� ����
    }
  }


  public void ChangeHp(float maxHpMulVal, float maxHpPlusVal, float currentHpMulVal, float currentHpPlusVal)
  {

    ItemHpChange?.Invoke(maxHpMulVal, maxHpPlusVal, currentHpMulVal, currentHpPlusVal); // HpChange �̺�Ʈ ȣ��
  }

  public void ChangeAtt(float attMulVal, float attPlusVal)
  {
    ItemAttChange?.Invoke(attMulVal, attPlusVal); // AttChange �̺�Ʈ ȣ��
  }

  public void ChangeDef(float defMulVal, float defPlusVal)
  {
    ItemDefChange?.Invoke(defMulVal, defPlusVal); // DefChange �̺�Ʈ ȣ��
  }

  public void ChangeSpd(float spdMulVal, float spdPlusVal)
  {
    ItemSpdChange?.Invoke(spdMulVal, spdPlusVal); // SpdChange �̺�Ʈ ȣ��
  }

  public void ChangeAttSpd(float attSpdPlusVal)
  {
    
    ItemAttSpdChange?.Invoke(attSpdPlusVal); // AttSpdChange �̺�Ʈ ȣ��
  }

  public void ChangeGoldAcq(float goldAcqPlusVal)
  {
    ItemGoldAcqChange?.Invoke(goldAcqPlusVal); // AttSpdChange �̺�Ʈ ȣ��
  }

  public void ChangeItemDrop(float itemDropPlusVal)
  {
    ItemItemDropChange?.Invoke(itemDropPlusVal); // AttSpdChange �̺�Ʈ ȣ��
  }

  public void ChangeDefIgn(float defIgnPlusVal)
  {
    ItemDefIgnChange?.Invoke(defIgnPlusVal); // AttSpdChange �̺�Ʈ ȣ��
  }

  public void ChangeJumpCount(int jumpCountPlusVal)
  {
    ItemJumpCountChange?.Invoke(jumpCountPlusVal); // AttSpdChange �̺�Ʈ ȣ��
  }
}
