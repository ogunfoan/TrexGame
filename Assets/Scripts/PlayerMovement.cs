using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]

    public Button jumpButton;
    private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 15f;
    private bool isGrounded;
    private bool gameOver;
    private bool isJumpButtonPressed = false;

    [Header("Audio")]
    public AudioClip jumpClip;
    private AudioSource audioSource;

    [Header("Shield Power-Up")]
    public bool hasShield = false;
    public Image shieldIcon;

    [Header("Particle Effects")]
    public ParticleSystem shieldEffect;
    public ParticleSystem InstanceParticleEffect;
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();

        // Kalkan ikonunu başlangıçta gizle
        if (hasShield == false && shieldIcon != null)
            shieldIcon.gameObject.SetActive(false);
    }

    void Update()
    {
        Jump();
    }

    public void ShieldImageController()
    {
        if (hasShield == true)
        {
            shieldIcon.gameObject.SetActive(hasShield);
            print("Shield activated");
        }
    }


    public void JumpButtonPressed()
    {
        isJumpButtonPressed = true;
        print("Jump button pressed");
        Invoke("ResetJumpButton", 0.1f); // Kısa bir gecikme ile buton durumunu sıfırla
    }

    private void ResetJumpButton()
    {
        isJumpButtonPressed = false;
    }

    public void Jump()
    {
        if (gameOver) return;

        if ( (isJumpButtonPressed && isGrounded) || (Input.GetKeyDown(KeyCode.Space) && isGrounded))
        {
            // Mevcut dikey hızı sıfırla, ardından anlık kuvvet ekle
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            audioSource.PlayOneShot(jumpClip);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Obstacles"))
        {
            if (hasShield)
            {
                // Kalkanı harca, engeli yok et, oyuna devam et
                hasShield = false;
                if (shieldIcon != null)
                    shieldIcon.gameObject.SetActive(false);

                Destroy(collision.gameObject);
            }
            else
            {
                // Kalkan yoksa game over
                gameOver = true;
                gameManager.GameOver();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerUp"))
        {
            // Kalkan power-up toplandı
            hasShield = true;
            ShieldEffectController();
            ShieldImageController();
            Destroy(other.gameObject);
        }
    }

    private void ShieldEffectController()
    {
        if (hasShield == true)
        {
            if (!shieldEffect.isPlaying)
            {
                shieldEffect.Play();
            }
        }
        else
        {
            if (shieldEffect.isPlaying)
            {
                shieldEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

}
    
