using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;

  public int maxPlayerCount;
  public int currentPlayerCount;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); // �� ��ȯ �� ����
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
