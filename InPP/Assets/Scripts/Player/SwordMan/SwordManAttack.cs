using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordManAttack : MonoBehaviour
{
  PlayerController pc;    // �÷��̾� ��Ʈ�ѷ�
  GameObject player;    // �÷��̾��� ĳ����

  public GameObject[] AttackPrefabs;    // ���� ������Ʈ ������
  GameObject newAttack;   // ���ݽ� ����� ������Ʈ

  Vector3 pos;    // ���� ������Ʈ�� ��ȯ �� ��ġ

  void Start()
  {
    player = GameObject.FindWithTag("Player");
    pc = player.GetComponent<PlayerController>();

    // �� Ÿ���� ���ݷ� ���� ����
    PlayerPrefs.SetFloat("SwordAttack1", 1.0f);
    PlayerPrefs.SetFloat("SwordAttack2", 1.2f);
    PlayerPrefs.SetFloat("SwordAttack3", 1.5f);

  }

  void Update()
  {
    if (pc.isAttack == true && newAttack == null && !IsInvoking("SpawnAttack"))
    {
      Invoke("SpawnAttack", pc.attackTime * 0.25f);
    }
  }

  void SpawnAttack()
  {
    if (AttackPrefabs == null)
    {
      return;
    }
    
    // ���� ������ ���� ������Ʈ�� �����Ǳ� ���� �ǰݽ� ���� ���
    if(pc.isKnockBack)
    {
      pc.isAttack = false;
      return;
    }

    pos = player.transform.position;

    // �÷��̾ �ٶ󺸴� ���⿡ ���� ���� ���� ��ġ ����
    if (player.transform.rotation.y == 0)
    {
      pos.x += 1;
    }

    else
    {
      pos.x -= 0.5f;
    }

    int index = pc.comboStep - 1;

    if (index < 0 || index >= AttackPrefabs.Length)
    {
      return; // �迭 ������ ����� �������� �ʰ� ����
    }

    newAttack = Instantiate(AttackPrefabs[index], pos, Quaternion.identity);
    newAttack.name = AttackPrefabs[index].name;    // �̸� �ڿ� Clone����

    StartCoroutine(AttackActiveFalse(newAttack, pc.attackTime / 1.5f));
    Destroy(newAttack, pc.attackTime);
  }

  // ���� ��Ȱ��ȭ �޼���
  IEnumerator AttackActiveFalse(GameObject Attack, float time)
  {
    yield return new WaitForSeconds(time);
    Attack.SetActive(false);
  }  
}
