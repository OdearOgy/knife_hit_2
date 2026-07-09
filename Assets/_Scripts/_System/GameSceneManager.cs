using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
  public static GameSceneManager Instance { get; private set; }

  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
  }

  public void LoadMainMenu()
  {
    SceneManager.LoadScene("MainMenu");
  }

  public void LoadGameplay()
  {
    SceneManager.LoadScene("Gameplay");
  }

  public void LoadGameOver()
  {
    SceneManager.LoadScene("GameOver");
  }

  public void RestartGameplay()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }
}
