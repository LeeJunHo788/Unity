using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpManager : MonoBehaviour
{
  public static ExpManager instance;   // �̱��� ����

  public float maxExp;        // �ִ� ����ġ
  public float currentExp;    // ���� ����ġ

  GameObject player;    // �÷��̾� ��ü
  PlayerController pc;  // �÷��̾� ��Ʈ�ѷ�

  public static event Action<float, float, int> ExpChange; // ����ġ ���� �̺�Ʈ
  public static event Action<int> LevelUp; // ������ �̺�Ʈ

  void Awake()
  {
    if(instance == null)
    {
      instance = this;
    }
  }

  void Start()
  {
    StartCoroutine(Starting());
  }

  IEnumerator Starting()
  {
    while (GameObject.FindWithTag("Player") == null)
    {
      yield return null; // ���� �����ӱ��� ���
    }
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();

    // �ʱ� ����ġ ����
    maxExp = Mathf.RoundToInt(40 * Mathf.Pow(1.1f, pc.level - 1));     // 1�� maxExp = 100;
    currentExp = 0;

    ExpChange?.Invoke(maxExp, currentExp, pc.level);  // ����ġ �� ������Ʈ
  }

  public void AddExp(int exp)
  {
    currentExp += exp + (exp * pc.expAcq);

    if(currentExp >= maxExp)
    {
      // ������ ����Ʈ �߻�
      // �����߰�

      pc.level++;   // �÷��̾� ���� ����
      currentExp -= maxExp; // �ʰ� ����ġ ����
      maxExp = Mathf.RoundToInt(100 * Mathf.Pow(1.1f, pc.level - 1));   // �ִ� ����ġ ����

      LevelUp?.Invoke(pc.level);

    }

    ExpChange?.Invoke(maxExp, currentExp, pc.level);

  }


}
