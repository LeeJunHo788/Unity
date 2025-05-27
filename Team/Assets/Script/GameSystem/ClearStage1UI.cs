using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearStage1UI : MonoBehaviour
{
  public GameObject clearUI;  //���Ѹ�� �ر� UI
  GameManager gm;
  private AudioSource audioSource; //ȿ������ ���� 

  //���� �Ѵ� ������ �����ϳ�, ���� ����Ǹ� ���µǵ���
  private static bool hasOpen = false;

  private void Start()
  {
    //���� �Ŵ��� �ι��� �ڵ� �߰�
    GameObject gmObj = GameObject.Find("GameManager");

    if(gmObj != null)
    {
      gm = gmObj.GetComponent<GameManager>();
      if (gm == null)
        Debug.Log("GameManager������Ʈ�� ����");
    }
    else
    {
      Debug.Log("���ӸŴ��� ������Ʈ�� ã�� �� ����");
    }
      audioSource = GetComponent<AudioSource>(); //������Ŵ���ã��
  }

  private void Update()
  {
    if (hasOpen) return;

    if(gm!= null && gm.clearedStage1)
    {
      clearUI.SetActive(true); //��������1Ŭ����� �ر�UIȰ��ȭ
      if (audioSource != null && SFXManager.instance != null && SFXManager.instance.buttonClicked != null)
      {
        audioSource.PlayOneShot(SFXManager.instance.buttonClicked);
      }
      else
      {
        Debug.LogWarning("ȿ���� ��� ���� �����ϳ��� null");
      }
      hasOpen = true;
      Debug.Log("static���� ��-��!");
    }
  }

  //��ư�� ������ �Լ�
  public void CloseClearPanel()
  {
    clearUI.SetActive(false); //��Ȱ��ȭ
  }
}
