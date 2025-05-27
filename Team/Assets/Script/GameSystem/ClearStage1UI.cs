using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearStage1UI : MonoBehaviour
{
  public GameObject clearUI;  //���Ѹ�� �ر� UI
  GameManager gm;
  private AudioSource audioSource; //ȿ������ ���� 
  bool hasOpen = false;

  private void Start()
  {
    gm = GameObject.Find("GameManager").GetComponent<GameManager>(); //���ӸŴ���ã��
    audioSource = GetComponent<AudioSource>(); //������Ŵ���ã��
  }

  private void Update()
  {
    if (hasOpen) return;

    if(gm!= null && gm.clearedStage1)
    {
      clearUI.SetActive(true); //��������1Ŭ����� �ر�UIȰ��ȭ
      //audioSource.PlayOneShot(SFXManager.instance.uiApear); //ȿ����
      hasOpen = true;
    }
  }

  //��ư�� ������ �Լ�
  public void CloseClearPanel()
  {
    clearUI.SetActive(false); //��Ȱ��ȭ
  }
}
