using UnityEngine;
using System.Collections;

public class BlockSpawner : MonoBehaviour
{
  public GameObject blockObject;
  Transform[] spawnPoints;

  private void Start()
  {
    spawnPoints = new Transform[transform.childCount];

    for (int i = 0; i < transform.childCount; i++)
    {
      spawnPoints[i] = transform.GetChild(i);
    }
  }

  void SpawnBlock()
  {

  }

}
