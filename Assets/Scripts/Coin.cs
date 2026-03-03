using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.coins += 1;
            Destroy(gameObject);
        }
    }
}
