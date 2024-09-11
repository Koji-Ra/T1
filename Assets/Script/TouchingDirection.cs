using UnityEngine;

public class TouchingDirection : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;
    public float wallDistance = 0.02f;
    public float ceilingDistance = 0.05f;

    private Collider2D touchingCol;
    private Animator animator;

    [SerializeField]
    private bool _isGrounded;
    [SerializeField]
    private bool _isOnWall;
    [SerializeField]
    private bool _isOnCeiling;

    public bool IsGrounded
    {
        get => _isGrounded;
        private set
        {
            _isGrounded = value;
            animator?.SetBool(AnimationStrings.IsGrounded, value);
        }
    }

    public bool IsOnWall
    {
        get => _isOnWall;
        private set
        {
            _isOnWall = value;
            animator?.SetBool(AnimationStrings.IsOnWall, value);
        }
    }

    public bool IsOnCeiling
    {
        get => _isOnCeiling;
        private set
        {
            _isOnCeiling = value;
            animator?.SetBool(AnimationStrings.IsOnCeiling, value);
        }
    }

    private Vector2 wallCheckDirection => transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    private void Awake()
    {
        touchingCol = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        CheckGroundContact();
        CheckWallContact();
        CheckCeilingContact();
    }

    private void CheckGroundContact()
    {
        IsGrounded = touchingCol.Cast(Vector2.down, castFilter, new RaycastHit2D[5], groundDistance) > 0;
    }

    private void CheckWallContact()
    {
        IsOnWall = touchingCol.Cast(wallCheckDirection, castFilter, new RaycastHit2D[5], wallDistance) > 0;
    }

    private void CheckCeilingContact()
    {
        IsOnCeiling = touchingCol.Cast(Vector2.up, castFilter, new RaycastHit2D[5], ceilingDistance) > 0;
    }
}
