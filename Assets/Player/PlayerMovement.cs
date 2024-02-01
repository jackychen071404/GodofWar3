using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    private Animator anim;

    private bool isJumping;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int dmg;

    public GameObject attackPoint;
    public float radius;
    public LayerMask enemies;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        //Debug.Log("This is a debug message. The value of myNumber is: " + horizontal);

        if (IsGrounded()){
            coyoteTimeCounter = coyoteTime;
            if (horizontal != 0)
                {
                    anim.SetBool("running",true);
                    anim.SetBool("jumping",false);
                }
            else
                {
                    anim.SetBool("running",false);
                    anim.SetBool("jumping",false);
                }
        } else {
            coyoteTimeCounter -= Time.deltaTime;
            anim.SetBool("jumping",true);
            anim.SetBool("running",false);
        }

        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferTime;
        } else {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            jumpBufferCounter = 0f;
            StartCoroutine(JumpCooldown());
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f) {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown("z")) {
            Attack();
        }

        Flip();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private void Attack() {
        anim.SetTrigger("attacking");
    }

    private void DetectAttack() {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position,radius,enemies);
        foreach (Collider2D enemyGameobject in enemy) {
            Debug.Log(enemyGameobject.GetComponent<cuboneHealth>().health);
            enemyGameobject.GetComponent<cuboneHealth>().TakeDamage(dmg);
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontal > 0f || !isFacingRight && horizontal < 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.4f);
        isJumping = false;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
    }
}
