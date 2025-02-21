using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    public float speed = 20f;
    public GameObject effectDestroy;
    private Vector3 moveDirection;

    void Start()
    {
        // Chọn góc ngẫu nhiên từ trên cao
        float angle = Random.Range(-45f, 45f); // Góc rơi ngẫu nhiên
        moveDirection = Quaternion.Euler(0, 0, angle) * Vector3.down; // Hướng rơi

        // Điều chỉnh tốc độ nếu cần
        speed = Random.Range(15f, 25f);
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stone"))
        {
            GameObject gm = Instantiate(effectDestroy, transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
            Destroy(gm, 2f);
        }
    }
}
