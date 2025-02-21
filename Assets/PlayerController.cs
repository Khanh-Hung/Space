using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public GameObject missilePrefab;
    public Transform missilePosition;
    public GameObject effectDestroy;
    public GameObject gameOver;
    public GameObject UIObject;
    public GameObject nextScene;
    public GameObject gameManage;
    public GameObject fireEffectPrefab; // Fire Effect Prefab
    private GameObject fireEffectInstance; // Fire Effect Instance

    public AudioSource audioSource;
    public AudioClip stoneHitSound;
    public AudioClip starCollectSound;
    public AudioClip shotSound;

    private float minX = -10f; // Giới hạn bên trái
    private float maxX = 10f;// Giới hạn bên phải
    private float minY = -5f;  // Giới hạn dưới
    private float maxY = 5f;   // Giới hạn trên

    void Start()
    {
        speed = 15f;

        if (audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Khởi tạo hiệu ứng lửa nhưng tắt ban đầu
        if (fireEffectPrefab != null)
        {
            fireEffectInstance = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity);
            fireEffectInstance.transform.SetParent(transform); // Gắn vào ship
            fireEffectInstance.transform.localPosition = new Vector3(0, -0.5f, 0); // Điều chỉnh vị trí
            fireEffectInstance.SetActive(false); // Tắt ban đầu
        }
    }

    void Update()
    {
        PlayerMove();
        PlayerShoot();
    }

    void PlayerMove()
    {
        float xPosition = Input.GetAxis("Horizontal");
        float yPosition = Input.GetAxis("Vertical");
        Vector3 v = new Vector3(xPosition, yPosition, 0) * speed * Time.deltaTime;
        transform.Translate(v);

        // Giới hạn vị trí của Player trên cả trục X và trục Y
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);

        // Kiểm tra nếu nhấn phím W thì bật hiệu ứng lửa, nếu không thì tắt
        if (Input.GetKey(KeyCode.W))
        {
            if (fireEffectInstance != null)
                fireEffectInstance.SetActive(true);
        }
        else
        {
            if (fireEffectInstance != null)
                fireEffectInstance.SetActive(false);
        }
    }

    void PlayerShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.PlayOneShot(shotSound);
            GameObject gm = Instantiate(missilePrefab, missilePosition);
            gm.transform.SetParent(null);
            Destroy(gm, 5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stone"))
        {
            audioSource.PlayOneShot(stoneHitSound);
            GameObject gm = Instantiate(effectDestroy, transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
            Destroy(gm, 1f);
            StartCoroutine(WaitAndShowGameOver());
        }

        if (collision.CompareTag("Star"))
        {
            audioSource.PlayOneShot(starCollectSound);
            ShowNextScene();
            Destroy(collision.gameObject);
            UIObject.GetComponent<UIController>().InscreaseScore();
        }

        if (collision.CompareTag("NextScene"))
        {
            NextScene();
        }
    }

    public void ShowNextScene()
    {
        int score = UIObject.GetComponent<UIController>().GetScore();
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int scoreToPass = 5;
        if (currentIndex >= 3)
        {
            scoreToPass = 10;
        }

        if (score >= scoreToPass)
        {
            Vector3 v = new Vector3(0f, 3f, 0f);
            Instantiate(nextScene, v, Quaternion.identity);
            gameManage.GetComponent<GameManage>().CancelInvoke();
        }
    }

    public void NextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }

    IEnumerator WaitAndShowGameOver()
    {
        yield return new WaitForSeconds(0.5f);
        gameOver.SetActive(true);
        Destroy(this.gameObject);
    }
}
