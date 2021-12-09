using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject wall;
    InfoCarry info;

    private void Awake()
    {
        info = FindObjectOfType<InfoCarry>().GetComponent<InfoCarry>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.Z))
        {
            wall.SetActive(false);
            gameObject.SetActive(false);
            info.delete.Add(wall.name);
            info.delete.Add(gameObject.name);
        }
    }
}
