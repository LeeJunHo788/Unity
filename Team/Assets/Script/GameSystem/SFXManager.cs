using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
  //�� �Ѿ�� ������ �ϵ� ��� ����� Ŭ�� ��Ƴ��� ��ũ��Ʈ��
  //ȣ���� ���� ��ũ��Ʈ���� �ؾ���
  public static SFXManager instance;

  public AudioClip axeThrew;
  public AudioClip bombThrew;
  public AudioClip bone;
  public AudioClip gun;
  public AudioClip knifeThrew;
  public AudioClip flame;
  public AudioClip enemyKill;
  public AudioClip boss3Kill; //����Ʈ����3, �������� ����
  public AudioClip catKill; //����Ʈ����1 ����
  public AudioClip buttonClicked;
  public AudioClip item; //������ �׵�� ���� �Ҹ�
  public AudioClip uiApear; //�ر�â ���ö� 

  private void Awake()
  {
    if(instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject); //�� �Ѿ�� ���� 
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
