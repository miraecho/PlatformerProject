using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;

    private Vector2 direction;

    public void setDirection(Vector2 dir) 
    {
        direction = dir;
        Destroy(gameObject, lifeTime);
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") return;
        
        Destroy(gameObject);
    }
}
