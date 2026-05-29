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

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float fireRate;

    [Header("Invincibility")]
    public float iFrameDuration = 1f;
    private bool isInvincible;

    [Header("Wall Sliding")]
    public float wallCheckDistance = 0.46f;
    public float wallSlideSpeed = 2f;

    private bool isTouchingWall;
    private bool isWallSliding;

    private float fireTimer;

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

    public bool speedBoost;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        healthImage = GameObject.FindWithTag("Health").GetComponent<Image>();

        extraJumps = extraJumpValue;

        if (Checkpoint.savedPosition != Vector2.zero) 
        {
            transform.position = Checkpoint.savedPosition;
        }
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

        HandleShooting();
        HandleWallSlide(moveInput);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        isTouchingWall = Physics2D.Raycast(transform.position, spriteRenderer.flipX ? Vector2.left : Vector2.right, wallCheckDistance, groundLayer);

        float moveInput = Input.GetAxis("Horizontal");
        rb.AddForce(new Vector2(moveInput * moveSpeed * 50, 0f), ForceMode2D.Force);

        if (!speedBoost)
            rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -moveSpeed, moveSpeed), rb.linearVelocity.y);

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
            if (isWallSliding)
            {
                animator.Play("Player_WallSlideLeft");
            }
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (isInvincible) return;
            
            PlaySFX(hurtClip);
            health -= 25;
            StartCoroutine(BlinkRed());

            StartCoroutine(InvincibilityFrames());

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
        spriteRenderer.color = new Color(Color.red.r, Color.red.g, Color.red.b, spriteRenderer.color.a);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = new Color(Color.white.r, Color.white.g, Color.white.b, spriteRenderer.color.a);
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

    private void HandleShooting() 
    {
        fireTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireTimer <= 0) 
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    private void Shoot() 
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (spriteRenderer.flipX) 
        {
            bulletScript.setDirection(Vector2.left);
        }
        else 
        {
            bulletScript.setDirection(Vector2.right);
        }

    }

    private IEnumerator InvincibilityFrames() 
    {
        isInvincible = true;

        float elapsed = 0f;

        while (elapsed < iFrameDuration) 
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.2f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            yield return new WaitForSeconds(0.1f);

            elapsed += 0.2f;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        isInvincible = false;
    }

    private void HandleWallSlide(float moveInput) 
    {
        if (isTouchingWall && !isGrounded && moveInput != 0 && rb.linearVelocityY > 0) 
        {
            isWallSliding = true;
            rb.linearVelocityY = -wallSlideSpeed;
        }
        else 
        {
            isWallSliding = false;
        }
    }
}
