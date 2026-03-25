using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int coins;
    public int health = 100;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Image healthImage;

    public AudioClip jumpClip;
    public AudioClip hurtClip;

    private Rigidbody2D rb;
    private bool isGrounded;

    private Animator animator;

    private SpriteRenderer spriteRenderer;

    private AudioSource audioSource;

    public int extraJumpValue = 1;
    private int extraJumps;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        extraJumps = extraJumpValue;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (isGrounded) 
        {
            extraJumps = extraJumpValue;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded) 
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlaySFX(jumpClip);
            }
            else if (extraJumps > 0) 
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extraJumps--;
                PlaySFX(jumpClip);
            }
        }
        setAnimation(moveInput);

        healthImage.fillAmount = health / 100f;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void setAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if (moveInput == 0)
            {
                animator.Play("Player_Idle");
            }
            else
            {
                animator.Play("Player_Run");
            }
        }
        else 
        {
            if (rb.linearVelocityY > 0) 
            {
                animator.Play("Player_Jump");
            }
            else 
            {
                animator.Play("Player_Fall");
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage") 
        {
            PlaySFX(hurtClip);
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            if (health <=  0) 
            {
                Die();
            }
        }
        else if (collision.gameObject.tag == "Bounce Pad")
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 3);
        }
    }

    private IEnumerator BlinkRed() 
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die() 
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void PlaySFX(AudioClip audioClip, float volume = 1f) 
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
    }
}
