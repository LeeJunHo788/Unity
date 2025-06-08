using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BlockSpawner : MonoBehaviour
{
  public GameObject blockObject;
  Transform[] spawnPoints;

  private void Start()
  {
    // ��ȯ ��ġ ��������
    spawnPoints = new Transform[transform.childCount];
    for (int i = 0; i < transform.childCount; i++)
    {
      spawnPoints[i] = transform.GetChild(i);
    }


    PlayerController.Instance.OnPlayerReady += SpawnBlock;  // �̺�Ʈ ����
  }

  // ������ ��ġ�� �� ��ȯ
  void SpawnBlock()
  {
    int count = Random.Range(1, spawnPoints.Length);

    List<int> rnd = new List<int>();

    while (rnd.Count < count)
    {
      int num = Random.Range(0, spawnPoints.Length);
      if(!rnd.Contains(num))
      {
        rnd.Add(num);
      }
    }

    foreach (int n in rnd)
    {
      GameObject block = Instantiate(blockObject, spawnPoints[n].transform.position, Quaternion.identity);
      block.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

      block.transform.DOScale(new Vector3(1.2f, 0.6f, 1), 0.3f).SetEase(Ease.InOutSine);
    }

  }

  void OnDestroy()
  {
    PlayerController.Instance.OnPlayerReady -= SpawnBlock;  // �̺�Ʈ ���� ����
  }

}
