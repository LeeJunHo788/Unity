using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
{
    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 스크롤기능 추가해야함
    }


}
