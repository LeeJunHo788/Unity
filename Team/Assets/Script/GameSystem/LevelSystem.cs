using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    EnemyController Enemy;    
    
    public enum LevelState { Normal,Hard,VeryHard}
    public LevelState L_State;

    public TextMeshProUGUI levelText;


    // Start is called before the first frame update
    void Start()
    {
        Enemy = GetComponent<EnemyController>();
        levelText = GameObject.Find("StageLevel").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (L_State)
        {
            case LevelState.Normal:
                levelText.text = "Normal";
                break;
            case LevelState.Hard:
                levelText.text = "Hard";
                break;
            case LevelState.VeryHard:
                levelText.text = "Very Hard";
                break;
        }
    }

    // 난이도 선택 효과에 따른 몹 강화요소
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            switch (L_State)
            {
                case LevelState.Normal:
                    collision.transform.gameObject.GetComponent<EnemyController>().maxHp *= 1.0f;
                    break;

                case LevelState.Hard:
                    collision.transform.gameObject.GetComponent<EnemyController>().maxHp *= 2.0f;
                    collision.transform.gameObject.GetComponent<EnemyController>().attackDem += 2;        
                    break;

                case LevelState.VeryHard:
                    collision.transform.gameObject.GetComponent<EnemyController>().maxHp *= 4.0f;
                    collision.transform.gameObject.GetComponent<EnemyController>().attackDem *= 2;
                    collision.transform.gameObject.GetComponent<EnemyController>().moveSpeed += 0.5f;
                    break;                
            }
        }

    }
}
