using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ������ �⺻ Ŭ���� 
 * ������ ��ũ��Ʈ �����Ͻð� �� Ŭ������ ��ӹ޾� ��� 
 */
public class Itembase : MonoBehaviour
{
  public int ItemIndex { get; set; } // ������ �ε���
  public string itemName; // ������ �̸�
  public string itemexplain; // ������ ����
  public int Level = 1; // ������ ����
  public int maxLevel = 10; // ������ �ִ� ����

  public virtual void Upgrade()
  {
    Level++; // ������        
  }
}