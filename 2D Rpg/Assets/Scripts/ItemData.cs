using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class ItemData : ScriptableObject
{
  public Sprite sprite;             // 이미지
  public string itemName;           // 이름
  public string info;               // 정보
  public float maxHpMul = 1;          // 최대체력 배율
  public int maxHpPlus = 0;         // 최대체력 증가량
  public float currentHpMul = 1;      // 현재체력 배율
  public int currentHpPlus = 0;     // 현재체력 증가량

  public float attMul = 1f;         // 공격력 배율
  public float attPlus = 0f;        // 공격력 증가량

  public float defMul = 1f;         // 방어력 배율
  public float defPlus = 0f;        // 방어력 증가량

  public float spdMul = 1f;         // 이동속도 배율
  public float spdPlus = 0f;        // 이동속도 증가량

  public float attSpdPlus = 0f;     // 공격속도 증가량

  public float goldAcqPlus = 0f;    // 골드 획득량 증가량

  public float itemDropPlus = 0f;    // 골드 획득량 증가량

  public float defIgnPlus = 0f;     // 방어력 무시 증가량

  public int jumpCountPlus = 0;    // 점프 횟수 증가량

  public int price;                 // 가격

  public float weight = 0;              // 가중치(드랍시 적용)
}
