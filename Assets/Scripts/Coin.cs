using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    public AudioClip coinClip;
    public int coinsToGive = 1;
    private TextMeshProUGUI coinText;

    private Respawnable respawnable;

    private void Start()
    {
        coinText = GameObject.FindWithTag("Coin Text").GetComponent<TextMeshProUGUI>();
        respawnable = GetComponent<Respawnable>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.coins += coinsToGive;
            player.PlaySFX(coinClip, 0.4f);
            coinText.text = player.coins.ToString();
            respawnable.Hide();
        }
    }
}
