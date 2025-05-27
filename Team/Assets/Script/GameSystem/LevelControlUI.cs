using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class LevelControlUI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Text Text;
    public string introText;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Text.text = introText;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Text.text = "";
    }
}
