using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public float speed = 1f;
    public int lives = 5;
    public float jumpForce = 8f;

    public bool isGrounded = false;
    public float checkGroundOffsetY = -1.8f;
    public float checkGroundRadius = 0.3f;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;
    private float HorizontalMove = 0f;
    private bool FacingRight = true;

    public bool isAttacking = false;
    public bool isRecharged = true;

    public Transform attackPos;
    public float attackRange;
    public LayerMask enemy;

    public static Hero Instance { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        isRecharged = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (HorizontalMove == 0 && !isAttacking && isGrounded) State = States.idle;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }

        HorizontalMove = Input.GetAxisRaw("Horizontal") * speed;

        if (HorizontalMove < 0 && FacingRight && !isAttacking && isGrounded)
        {
            Flip();
        }
        else if (HorizontalMove > 0 && !FacingRight && !isAttacking && isGrounded)
        {
            Flip();
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    private States State
    {
        get { return (States)animator.GetInteger("state"); }
        set { animator.SetInteger("state", (int)value); }
    }

    private void FixedUpdate()
    {
        Vector2 TargetVelocity = new Vector2(HorizontalMove * 10f, rb.velocity.y);
        rb.velocity = TargetVelocity;

        CheckGround();
    }

    private void Flip()
    {
        FacingRight = !FacingRight;

        if (isGrounded) State = States.run;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector3(transform.position.x, transform.position.y + checkGroundOffsetY), checkGroundRadius);
        
        if (colliders.Length > 1)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (!isGrounded)
        {
            State = States.jump;
        }
    }

    private void Attack()
    {
        if (isGrounded && isRecharged && HorizontalMove == 0) 
        {
            State = States.attack1;
            isAttacking = true;
            isRecharged = false;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }

    private void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemy);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(1f);
        isAttacking = false;
        if (isGrounded) State = States.run;
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        isRecharged = true;
    }

    public void GetDamage()
    {
        lives -= 1;
        Debug.Log(lives);
    }
}

public enum States
{
    idle,
    run,
    jump,
    attack1,
    attack2
}