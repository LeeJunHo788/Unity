using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    // 배경 이미지
    public Material bgMaterial;

    // 스크롤 속도
    public float scrollSpeed = 0.2f;


    private void Update()
    {
        // 스크롤 방향
        Vector2 dir = Vector2.up;

        // 마테리얼에 포함된 텍스쳐 스크롤
        bgMaterial.mainTextureOffset += dir * scrollSpeed * Time.deltaTime;
    }
}
