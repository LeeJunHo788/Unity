using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMouse : MonoBehaviour
{    
    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
