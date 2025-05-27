using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Shop/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
  public ItemData[] shopItems;  // 상점 아이템 데이터 배열

}