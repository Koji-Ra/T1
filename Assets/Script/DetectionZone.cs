using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public List<Heath> detectedHeath = new List<Heath>(); // รายการของ Collider ที่ถูกตรวจจับ


    private void OnTriggerEnter2D(Collider2D other)
    {
        Heath heath;

        if (!other.gameObject.GetComponent<PlayerController2D>() && !transform.parent.GetComponent<PlayerController2D>()) return;

        if (heath = other.gameObject.GetComponent<Heath>())
        {
            if (!detectedHeath.Contains(heath))
            {
                detectedHeath.Add(heath); // เพิ่ม Collider ที่เข้าสู่โซน
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Heath heath;
        if (heath = other.gameObject.GetComponent<Heath>())
        {

            if (detectedHeath.Contains(heath))
            {
                detectedHeath.Remove(heath); // ลบ Collider ที่ออกจากโซน
            }
        }
    }
}