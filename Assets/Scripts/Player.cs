using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int coins;
    public int health = 100;
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float jumpContinuousForce = 0.6f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private Image healthImage;

    public AudioClip jumpClip;
    public AudioClip hurtClip;

    private Rigidbody2D rb;
    private bool isGrounded;

    private Animator animator;

    private SpriteRenderer spriteRenderer;

    private AudioSource audioSource;

    public int extraJumpValue = 1;
    private int extraJumps;

    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        healthImage = GameObject.FindWithTag("Health").GetComponent<Image>();

        extraJumps = extraJumpValue;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (rb.linearVelocityX != 0) 
        {
            if (rb.linearVelocityX > 0) 
            {
                spriteRenderer.flipX = false;
            }
            else 
            {
                spriteRenderer.flipX = true;
            }
        }

        if (isGrounded) 
        {
            coyoteTimeCounter = coyoteTime;
            extraJumps = extraJumpValue;
        }
        else 
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else 
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f)
        {
            if (coyoteTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlaySFX(jumpClip);
                coyoteTimeCounter = 0f;
                jumpBufferCounter = 0f;
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extraJumps--;
                PlaySFX(jumpClip);
            }
        }

        if (Input.GetKey(KeyCode.Space) && rb.linearVelocityY > 0) 
        {
            rb.AddForceY(jumpContinuousForce);
        }
            
        setAnimation(moveInput);

        healthImage.fillAmount = health / 100f;

        if (rb.linearVelocityY < 0) 
        {
            rb.gravityScale = 2f;
        }
        else 
        {
            rb.gravityScale = 1f;
        }

        if (transform.position.y < -10) 
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float moveInput = Input.GetAxis("Horizontal");
        rb.AddForce(new Vector2(moveInput * moveSpeed * 50, 0f), ForceMode2D.Force);

        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -moveSpeed, -moveSpeed), rb.linearVelocity.y);

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
            if (rb.linearVelocityY > 0f) 
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 2);
        }

        else if (collision.gameObject.tag == "Green Apple") 
        {
            health += 15;

            if (health > 100) 
            {
                health = 100;
            }
            Destroy(collision.gameObject);
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
