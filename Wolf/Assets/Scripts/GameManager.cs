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
      DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
