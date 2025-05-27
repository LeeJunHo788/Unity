using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // ���� ����
    private int currentScore;
    public Text currentScoreUI;

    // �̱��� ��ü
    public static ScoreManager Instance = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // �ְ� ����
    private int bestScore;
    public Text bestScoreUI;

    private void Start()
    {
        // �����ߴ� �ְ� ���� �ҷ�����
        bestScore = PlayerPrefs.GetInt("BestScore");
        bestScoreUI.text = "�ְ� ���� : " +bestScore;
    }

    // ������Ƽ
    public int Score
    {
        get
        {
            return currentScore;
        }

        set
        {
            // ���� ���� ���� ����
            currentScore = value;

            // ���� ������ �����Ѵ�
            currentScoreUI.text = "���� ���� : " + currentScore;

            // ���� ������ �ְ� ������ �Ѿ�� �� ���� �����Ѵ�
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                bestScoreUI.text = "�ְ� ���� : " + bestScore;

                // �ְ� ���� ����
                PlayerPrefs.SetInt("Best Score", bestScore);
            }
            
        }
    }
}
