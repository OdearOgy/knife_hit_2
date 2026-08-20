using UnityEngine;

public enum GameState
{
  Playing,
  Won,
  Lost
}

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }

  public GameState State { get; private set; } = GameState.Playing;

  public int CurrentScore { get; private set; }
  public int HighScore { get; private set; }
  public int MaxStage { get; private set; }

  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);

    HighScore = PlayerPrefs.GetInt("HighScore", 0);
    MaxStage = PlayerPrefs.GetInt("MaxStage", 0);
  }

  public void SetState(GameState state)
  {
    State = state;
  }

  public void AddScore()
  {
    CurrentScore++;
  }

  public void SaveHighScore()
  {
    int currentStage = LevelManager.Instance?.CurrentLevel?.levelNumber ?? 1;

    if (CurrentScore > HighScore)
    {
      HighScore = CurrentScore;
      PlayerPrefs.SetInt("HighScore", HighScore);
    }

    if (currentStage > MaxStage)
    {
      MaxStage = currentStage;
      PlayerPrefs.SetInt("MaxStage", MaxStage);
    }

    PlayerPrefs.SetInt("LastScore", CurrentScore);
    PlayerPrefs.SetInt("LastStage", currentStage);
    PlayerPrefs.Save();
  }

  public void ResetScore()
  {
    CurrentScore = 0;
    PlayerPrefs.DeleteKey("LastScore");
    PlayerPrefs.DeleteKey("LastStage");
    PlayerPrefs.Save();
  }
}
