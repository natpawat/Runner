using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ChickenController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float strafeSpeed = 5f;
    [SerializeField] private float laneLimit = 3f;

    [Header("Spawn Point")]
    [SerializeField] private Transform spawnPoint;

    [Header("Game Over Settings")]
    [SerializeField] private string obstacleTag = "Obstacle";
    [SerializeField] private GameObject gameOverPanel;

    [Header("Score")]
    [SerializeField] private int coinScore = 0;

    private Rigidbody rb;
    private bool isGameOver = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (isGameOver)
        {
            return;
        }

        float horizontalInput = GetHorizontalInput();

        Vector3 forwardMove = Vector3.forward * forwardSpeed;
        Vector3 sidewaysMove = Vector3.right * horizontalInput * strafeSpeed;

        Vector3 velocity = forwardMove + sidewaysMove;
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;

        ClampPosition();
    }

    private float GetHorizontalInput()
    {
        float input = 0f;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
            {
                input = -1f;
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                input = 1f;
            }
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (input == 0f)
        {
            if (Input.GetKey(KeyCode.A))
            {
                input = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                input = 1f;
            }
        }
#endif

        return input;
    }

    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -laneLimit, laneLimit);
        transform.position = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(obstacleTag))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        Debug.Log("Game Over! Coins collected: " + coinScore);
    }

    public void AddCoins(int amount)
    {
        coinScore += amount;
        Debug.Log("Coins: " + coinScore);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}