using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public GameObject winUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            Time.timeScale = 0;
            winUI.SetActive(true);
        }
    }
}
