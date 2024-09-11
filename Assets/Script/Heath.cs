using UnityEngine;
using UnityEngine.UI;

public class Heath : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private float _maxhealth = 100f; // ค่าพลังชีวิตเริ่มต้น
    public float health = 100f; // ค่าพลังชีวิตปัจจุบัน

    [SerializeField]
    private float invincibilityTimer = 2f; // เวลาที่ตัวละครจะอยู่ในสถานะ invincible หลังจากโดนโจมตี
    private float timeSinceHit = 0f; // เวลาที่ผ่านไปตั้งแต่ตัวละครโดนโจมตีครั้งล่าสุด

    [SerializeField]
    private Image hpBar; // บาร์พลังชีวิต

    [SerializeField]
    private bool _isAlive = true; // ตรวจสอบสถานะความมีชีวิต

    [SerializeField]
    private bool isInvincible = false; // ตรวจสอบสถานะการไม่สามารถโดนโจมตี

    public GameObject gameover; // UI เกมจบ
    public GameObject levelcomplete; // UI ระดับเสร็จสิ้น
    [SerializeField]
    private bool boss;
    Knight monster;

    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetTrigger(AnimationStrings.IsAlive);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        monster = GetComponent<Knight>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTimer)
            {
                isInvincible = false;
                timeSinceHit = 0f;
            }

            timeSinceHit += Time.deltaTime;
        }

        hpBar.fillAmount = health / _maxhealth;

        if (health <= 0)
        {
            if (_isAlive)
            {
                // การจัดการการตายของตัวละคร
                _isAlive = false;
                animator.Play("Death"); // เปลี่ยนเป็นชื่อ Animation ที่ถูกต้องของการตาย
                HandleDeath();
            }
        }
    }

    public void Hit(float damage)
    {
        if (_isAlive && !isInvincible)
        {
            health -= damage;
            isInvincible = true;
            timeSinceHit = 0f;
        }
    }

    private void HandleDeath()
    {
        if (name == "Player")
        {
            gameover.SetActive(true); // แสดง UI เกมจบ
        }
        else
        {
            // ถ้าไม่ใช่ Player เช่น BOSS
            if (monster.itemDrops.Length > 0)
            {
                //ดรอปแบบสุ่ม สุ่มดรอของในตัวมอน
                if (monster.randomDrop)
                {
                    Instantiate(monster.itemDrops[Random.Range(0, monster.itemDrops.Length)], transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(monster.itemDrops[0], transform.position, Quaternion.identity);
                }
                //ดรอแบบกำหนดเอง
            }
            if (boss)
                levelcomplete.SetActive(true); // แสดง UI ระดับเสร็จสิ้น
            Destroy(gameObject, 2f); // ทำลาย GameObject หลังจาก 2 วินาที
        }
    }
}
