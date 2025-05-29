using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBackground : MonoBehaviour
{
  public float scrollSpeed;     // 움직이는 속도
  public float tilemapWidth;     // 하나의 가로 길이

  public Transform Back1;             // 첫 번째 
  public Transform Back2;             // 두 번째 

  


  void Update()
  {
    // 왼쪽으로 이동
    Back1.position += Vector3.left * scrollSpeed * Time.deltaTime;
    Back2.position += Vector3.left * scrollSpeed * Time.deltaTime;

    if (Back1.position.x <= -tilemapWidth)
    {
      Back1.position = new Vector3(Back2.position.x + tilemapWidth, Back1.position.y, Back1.position.z);
    }

    if (Back2.position.x <= -tilemapWidth)
    {
      Back2.position = new Vector3(Back1.position.x + tilemapWidth, Back2.position.y, Back2.position.z);
    }
  }
}