using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 10;

    [Header("Movement Settings")]
    public float walkSpeed = 3f; // ความเร็วในการเดินของตัวละคร
    public float raycastDistance = 0.5f; // ระยะห่างของ Raycast เพื่อตรวจสอบการชน

    [Header("Detection Settings")]
    public DetectionZone attackZone; // โซนตรวจจับการโจมตี
    public float attackDelay = 1f; // การหน่วงเวลาการโจมตี


    [Header("Drop Setting")]
    public bool randomDrop = false;
    public GameObject[] itemDrops;
    public LayerMask wallMask;
    public Vector2 rayOffset;

    private Rigidbody2D rb; // Rigidbody2D ของตัวละคร
    private Vector2 walkDirectionVector; // ทิศทางการเดิน
    private TouchingDirection touchingDirection; // Component สำหรับตรวจสอบการสัมผัสพื้นผิว
    private Animator animator; // Animator ของตัวละคร
    private bool isChangingDirection = false; // ตรวจสอบการเปลี่ยนทิศทาง

    public enum WalkableDirection { Right, Left } // Enum สำหรับทิศทางการเดิน

    public WalkableDirection _walkDirection; // ทิศทางการเดินปัจจุบัน

    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value); // ใช้ SetBool เพื่อควบคุม Animator
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    public WalkableDirection WalkDirection
    {
        get => _walkDirection;
        set
        {
            if (_walkDirection != value)
            {
                // พลิกทิศทางการเดินของตัวละคร
                Vector3 scale = transform.localScale;
                scale.x *= -1; // พลิกแนวแกน X
                transform.localScale = scale;

                walkDirectionVector = value == WalkableDirection.Right ? Vector2.right : Vector2.left;
                _walkDirection = value;
            }
        }
    }

    public bool IsOnWall => touchingDirection.IsOnWall;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirection>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // เริ่มต้นด้วยทิศทางการเดินเริ่มต้น
        WalkDirection = WalkableDirection.Right;
    }

    private void FixedUpdate()
    {
        // ตรวจสอบการชน
        CheckForObstacles();

        // ตรวจสอบว่าตัวละครกำลังสัมผัสผนัง
        if (IsOnWall)
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // หยุดการเคลื่อนที่ถ้าตัวละครสัมผัสผนัง
        }
        else if (CanMove) // เคลื่อนที่เมื่อสามารถเคลื่อนที่ได้
        {
            rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y); // ตั้งค่าเวกเตอร์ความเร็วให้ตัวละครเดินไปในทิศทางที่กำหนด
        }
    }

    private void Update()
    {
        // ตรวจสอบว่ามีเป้าหมายในโซนโจมตีหรือไม่
        HasTarget = attackZone.detectedHeath.Count > 0;
        rb.velocity = new(0, rb.velocity.y);

        ChasinghAttack();
    }

    // ฟังก์ชันตรวจสอบการชน
    private void CheckForObstacles()
    {
        // ใช้ Raycast เพื่อตรวจสอบว่ามีสิ่งกีดขวางอยู่ข้างหน้า
        Vector2 raycastOrigin = (Vector2)transform.position + (walkDirectionVector * 0.5f) + rayOffset;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, walkDirectionVector, raycastDistance, wallMask);
        Debug.DrawRay(raycastOrigin, walkDirectionVector * raycastDistance, Color.red); // ใช้ Debug.DrawRay เพื่อตรวจสอบ Raycast

        if (hit.collider != null && !isChangingDirection)
        {
            //Debug.Log(hit.collider.name);

            // เปลี่ยนทิศทางเมื่อชนกับวัตถุ
            FlipDirection();
        }
    }

    // ฟังก์ชันสำหรับสลับทิศทางการเดิน
    private void FlipDirection()
    {
        WalkDirection = WalkDirection == WalkableDirection.Right ? WalkableDirection.Left : WalkableDirection.Right;
        isChangingDirection = true;

        // หยุดการเคลื่อนที่ชั่วคราวเมื่อเปลี่ยนทิศทาง
        if (CanMove)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        StartCoroutine(ResetDirectionChangeFlag()); // เริ่ม Coroutine เพื่อรีเซ็ตการเปลี่ยนทิศทาง
    }

    // Coroutine สำหรับรีเซ็ตสถานะการเปลี่ยนทิศทาง
    private IEnumerator ResetDirectionChangeFlag()
    {
        yield return new WaitForSeconds(0.1f); // รอเวลาเล็กน้อยก่อนรีเซ็ต
        isChangingDirection = false;
    }

    // ฟังก์ชันโจมตี Player
    public void ChasinghAttack()
    {
        if (!CanMove && attackZone.detectedHeath.Count > 0) // ตรวจสอบว่าตัวละครไม่สามารถเคลื่อนที่ได้และมีเป้าหมาย
        {
            Debug.Log("Player attacked!");

            attackZone.detectedHeath.ForEach(i => i.Hit(damage));
        }
    }
}
