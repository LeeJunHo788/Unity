using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // 현재 점수
    private int currentScore;
    public Text currentScoreUI;

    // 싱글톤 객체
    public static ScoreManager Instance = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // 최고 점수
    private int bestScore;
    public Text bestScoreUI;

    private void Start()
    {
        // 저장했던 최고 점수 불러오기
        bestScore = PlayerPrefs.GetInt("BestScore");
        bestScoreUI.text = "최고 점수 : " +bestScore;
    }

    // 프로퍼티
    public int Score
    {
        get
        {
            return currentScore;
        }

        set
        {
            // 전달 받은 값을 대입
            currentScore = value;

            // 현재 점수를 갱신한다
            currentScoreUI.text = "현재 점수 : " + currentScore;

            // 현재 점수가 최고 점수를 넘어섰을 때 동시 갱신한다
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                bestScoreUI.text = "최고 점수 : " + bestScore;

                // 최고 점수 저장
                PlayerPrefs.SetInt("Best Score", bestScore);
            }
            
        }
    }
}
