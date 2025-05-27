using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
  public int gameClear = 0;    // ���� Ŭ���� Ƚ��
  public List<string> unlockedCharacters = new List<string>();
}


public class SaveManager : MonoBehaviour
{


  private string savePath;

  void Awake()
  {
    // �ߺ� ����
    if (FindObjectsOfType<SaveManager>().Length > 1)
    {
      Destroy(gameObject);
      return;
    }

    DontDestroyOnLoad(gameObject); // �� ��ȯ���� ����
    savePath = Application.persistentDataPath + "/saveData.json";

    
    
  }

  void Start()
  {
    Debug.Log("���� ���: " + Application.persistentDataPath);

    // �׽�Ʈ�� ���� ����!
    // SaveGame();  // �� �� �� �߰� (�ӽ÷�!)
  }


  // ���� Ŭ���� �� ������ �Լ�
  public void GameClear()
  {
    SaveData data = LoadGame();

    // ó�� ������ Ŭ���� �ϸ� �˻� ĳ���� �ر�
    if (data.gameClear == 0)
    {
      data.unlockedCharacters.Add("SwordMan"); 

    }
    data.gameClear++;
      

    string json = JsonUtility.ToJson(data, true); // JSON ���ڿ��� ��ȯ
    File.WriteAllText(savePath, json); // ���Ϸ� ����

  }

  // ����� ������ �ҷ�����
  public SaveData LoadGame()
  {
    if (File.Exists(savePath))
    {
      string json = File.ReadAllText(savePath);
      SaveData data = JsonUtility.FromJson<SaveData>(json);
      return data;
    }
    else
    {
      // ����� ������ ������ ���ο� SaveData ��ȯ
      return new SaveData();
    }
  }

  // Ư�� ĳ���Ͱ� �رݵǾ����� Ȯ���ϴ� �Լ�
  public bool IsCharacterUnlocked(string characterName)
  {
    SaveData data = LoadGame();
    return data.unlockedCharacters.Contains(characterName);
  }




}
