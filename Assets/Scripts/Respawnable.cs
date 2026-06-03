using System.Collections;
using UnityEngine;

public class Respawnable : MonoBehaviour
{
    public float timeBeforeRespawn;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Hide() 
    {
        col.enabled = false;
        spriteRenderer.enabled = false;
        StartCoroutine(EnableAgain());
    }

    public IEnumerator EnableAgain() 
    {
        yield return new WaitForSeconds(timeBeforeRespawn);
        col.enabled = true;
        spriteRenderer.enabled = true;
    }
}
