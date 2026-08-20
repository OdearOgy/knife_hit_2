using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
  public static LevelManager Instance { get; private set; }

  [SerializeField] private LevelConfig[] levels;
  [SerializeField] private PlayerConfig playerConfig;

  private static int currentLevelIndex = 0;

  public LevelConfig CurrentLevel
  {
    get
    {
      if (playerConfig != null && playerConfig.forcedLevel != null)
        return playerConfig.forcedLevel;

      var config = GetLevelConfig(currentLevelIndex);
      Debug.Log($"[LevelManager] CurrentLevel getter: level={currentLevelIndex + 1}, config={(config != null ? config.name : "NULL")}");
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
    DontDestroyOnLoad(gameObject);
  }

  private LevelConfig GetLevelConfig(int levelIndex)
  {
    int levelNumber = levelIndex + 1;

    if (levelNumber % 5 == 0)
    {
      var bossPool = levels.Where(l => l.levelNumber == levelNumber).ToArray();
      if (bossPool.Length > 0)
        return bossPool[Random.Range(0, bossPool.Length)];
      return null;
    }

    return levels.FirstOrDefault(l => l.levelNumber == levelNumber);
  }

  public void LoadNextLevel()
  {
    if (playerConfig != null && playerConfig.forcedLevel != null) return;

    int maxLevel = levels.Length > 0 ? levels.Max(l => l.levelNumber) : 0;
    if (maxLevel == 0) return;

    currentLevelIndex = (currentLevelIndex + 1) % maxLevel;
    Debug.Log($"[LevelManager] Advanced to level index: {currentLevelIndex} (Level {currentLevelIndex + 1})");
  }

  public void RestartFromLevelOne()
  {
    currentLevelIndex = 0;
  }
}
