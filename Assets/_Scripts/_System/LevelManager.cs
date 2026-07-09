using UnityEngine;

public class LevelManager : MonoBehaviour
{
  public static LevelManager Instance { get; private set; }

  [SerializeField] private LevelConfig[] allLevels;

  private static int currentLevelIndex = 0;

  public LevelConfig CurrentLevel
  {
    get
    {
      var config = allLevels != null && currentLevelIndex < allLevels.Length ? allLevels[currentLevelIndex] : null;
      Debug.Log($"[LevelManager] CurrentLevel getter: index={currentLevelIndex}, config={(config != null ? config.name : "NULL")}");
      return config;
    }
  }

  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }
    Instance = this;
  }

  public void LoadNextLevel()
  {
    if (allLevels == null || allLevels.Length == 0) return;
    currentLevelIndex = (currentLevelIndex + 1) % allLevels.Length;
    Debug.Log($"[LevelManager] Advanced to level index: {currentLevelIndex}");
  }

  public void RestartFromLevelOne()
  {
    currentLevelIndex = 0;
  }
}
