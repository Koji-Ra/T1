using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TouchingDirection))]
public class PlayerController2D : MonoBehaviour
{
    public float damage = 20;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 10f;

    private Vector2 moveInput;
    private TouchingDirection touchingDirection;
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] private bool _isMoving = false;
    [SerializeField] private bool _isRunning = false;

    [SerializeField] Button[] itemSlots;

    public DetectionZone attackZone; // โซนตรวจจับการโจมตี

    public bool canMove => animator.GetBool(AnimationStrings.canMove);

    public bool IsMoving
    {
        get => _isMoving;
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.IsMoving, value);
        }
    }

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.IsRunning, value);
        }
    }

    public bool IsFacingRight { get; private set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
    }
    private void Start()
    {
        if (itemSlots.Length > 0)
            foreach (var item in itemSlots)
            {
                item.interactable = false;
            }
    }
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float speed = CalculateSpeed();
        Vector2 targetVelocity = new Vector2(moveInput.x * speed, rb.velocity.y);

        // Stop horizontal movement if on wall and grounded
        if (touchingDirection.IsOnWall && touchingDirection.IsGrounded)
        {
            targetVelocity.x = 0;
        }

        // Smooth movement
        Vector2 smoothVelocity = rb.velocity;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref smoothVelocity, 0.1f);

        // Update animator parameters
        bool isGrounded = touchingDirection.IsGrounded;
        animator.SetBool(AnimationStrings.IsGrounded, isGrounded);
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

        IsMoving = moveInput.x != 0;
    }

    private float CalculateSpeed()
    {
        return touchingDirection.IsGrounded ? (IsRunning ? runSpeed : walkSpeed) : walkSpeed;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        SetFacingDirection(moveInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && touchingDirection.IsGrounded || context.performed && touchingDirection.IsOnWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetTrigger(AnimationStrings.Jump);
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if ((moveInput.x > 0 && !IsFacingRight) || (moveInput.x < 0 && IsFacingRight))
        {
            Flip();
        }
    }

    private void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        IsRunning = context.performed;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            foreach (var item in attackZone.detectedHeath)
            {
                Debug.Log($"Attack to {item.gameObject.name}");
            }
            animator.SetTrigger(AnimationStrings.Attack);

            attackZone.detectedHeath.ForEach(i => i.Hit(damage));
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DropItem"))
        {
            Debug.Log($"Hit {collision.gameObject.name} {collision.gameObject.name == "Note 1"} || {collision.gameObject.name == "Note 1(Clone)"}");
            if (collision.gameObject.name == "Note 1" || collision.gameObject.name == "Note 1(Clone)")
            {
                itemSlots[0].interactable = true;
                Destroy(collision.gameObject);
            }
        }
    }
}