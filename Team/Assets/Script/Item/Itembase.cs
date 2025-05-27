using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 아이템 기본 클레스 
 * 아이템 스크립트 구성하시고 이 클래스를 상속받아 사용 
 */
public class Itembase : MonoBehaviour
{
  public int ItemIndex { get; set; } // 아이템 인덱스
  public string itemName; // 아이템 이름
  public string itemexplain; // 아이템 설명
  public int Level = 1; // 아이템 레벨
  public int maxLevel = 10; // 아이템 최대 레벨

  public virtual void Upgrade()
  {
    Level++; // 레벨업        
  }
}