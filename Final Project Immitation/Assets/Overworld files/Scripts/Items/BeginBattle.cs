using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginBattle : MonoBehaviour
{
    public List<BattleCharacter> foes;
    InfoCarry info;

    private void Awake()
    {
        info = FindObjectOfType<InfoCarry>().GetComponent<InfoCarry>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.Z))
        {
            info.enemies = foes;
            info.playerPosition = gameObject.transform.position;
            info.sceneName = SceneManager.GetActiveScene().name;
            info.delete.Add(gameObject.name);
            SceneManager.LoadScene("Omori Battle");
        }
    }

}
