using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class ItemData : ScriptableObject
{
  public Sprite sprite;             // �̹���
  public string itemName;           // �̸�
  public string info;               // ����
  public float maxHpMul = 1;          // �ִ�ü�� ����
  public int maxHpPlus = 0;         // �ִ�ü�� ������
  public float currentHpMul = 1;      // ����ü�� ����
  public int currentHpPlus = 0;     // ����ü�� ������

  public float attMul = 1f;         // ���ݷ� ����
  public float attPlus = 0f;        // ���ݷ� ������

  public float defMul = 1f;         // ���� ����
  public float defPlus = 0f;        // ���� ������

  public float spdMul = 1f;         // �̵��ӵ� ����
  public float spdPlus = 0f;        // �̵��ӵ� ������

  public float attSpdPlus = 0f;     // ���ݼӵ� ������

  public float goldAcqPlus = 0f;    // ��� ȹ�淮 ������

  public float itemDropPlus = 0f;    // ��� ȹ�淮 ������

  public float defIgnPlus = 0f;     // ���� ���� ������

  public int jumpCountPlus = 0;    // ���� Ƚ�� ������

  public int price;                 // ����

  public float weight = 0;              // ����ġ(����� ����)
}
