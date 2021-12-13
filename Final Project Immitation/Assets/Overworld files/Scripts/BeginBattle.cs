using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginBattle : MonoBehaviour
{
    public List<BattleCharacter> foes;
    InfoCarry info;
    float timer = 0.0f;

    private void Awake()
    {
        info = FindObjectOfType<InfoCarry>().GetComponent<InfoCarry>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && timer >= 1.0f)
        {
            info.enemies = foes;
            info.playerPosition = gameObject.transform.position;
            info.sceneName = SceneManager.GetActiveScene().name;
            info.delete.Add(gameObject.name);
            SceneManager.LoadScene("Omori Battle");
        }
    }
}