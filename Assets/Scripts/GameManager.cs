using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string winSceneName = "WinScreen";
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    public bool HasKeycard { get; private set; }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied += OnPlayerDied;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied -= OnPlayerDied;
        }
    }

    public void CollectKeycard()
    {
        HasKeycard = true;
    }

    public void TriggerWin()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            Time.timeScale = 0f;
            return;
        }

        if (!string.IsNullOrWhiteSpace(winSceneName))
        {
            SceneManager.LoadScene(winSceneName);
        }
    }

    private void OnPlayerDied()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
