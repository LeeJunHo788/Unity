using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
  public int gameClear = 0;    // 게임 클리어 횟수
  public List<string> unlockedCharacters = new List<string>();
}


public class SaveManager : MonoBehaviour
{


  private string savePath;

  void Awake()
  {
    // 중복 방지
    if (FindObjectsOfType<SaveManager>().Length > 1)
    {
      Destroy(gameObject);
      return;
    }

    DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
    savePath = Application.persistentDataPath + "/saveData.json";

    
    
  }

  void Start()
  {
    Debug.Log("저장 경로: " + Application.persistentDataPath);

    // 테스트용 저장 실행!
    // SaveGame();  // ← 이 줄 추가 (임시로!)
  }


  // 게임 클리어 시 실행할 함수
  public void GameClear()
  {
    SaveData data = LoadGame();

    // 처음 게임을 클리어 하면 검사 캐릭터 해금
    if (data.gameClear == 0)
    {
      data.unlockedCharacters.Add("SwordMan"); 

    }
    data.gameClear++;
      

    string json = JsonUtility.ToJson(data, true); // JSON 문자열로 변환
    File.WriteAllText(savePath, json); // 파일로 저장

  }

  // 저장된 데이터 불러오기
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
      // 저장된 파일이 없으면 새로운 SaveData 반환
      return new SaveData();
    }
  }

  // 특정 캐릭터가 해금되었는지 확인하는 함수
  public bool IsCharacterUnlocked(string characterName)
  {
    SaveData data = LoadGame();
    return data.unlockedCharacters.Contains(characterName);
  }




}
