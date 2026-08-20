using UnityEngine;

public class LevelManager : MonoBehaviour
{
  public static LevelManager Instance { get; private set; }

  [SerializeField] private LevelConfig[] normalLevels;
  [SerializeField] private LevelConfig[] bossLevel5;
  [SerializeField] private LevelConfig[] bossLevel10;
  [SerializeField] private LevelConfig[] bossLevel15;

  private static int currentLevelIndex = 0;

  public LevelConfig CurrentLevel
  {
    get
    {
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
      var bossPool = GetBossPool(levelNumber);
      if (bossPool != null && bossPool.Length > 0)
        return bossPool[Random.Range(0, bossPool.Length)];
      return null;
    }

    if (normalLevels != null && levelIndex < normalLevels.Length)
      return normalLevels[levelIndex];

    return null;
  }

  private LevelConfig[] GetBossPool(int levelNumber)
  {
    return levelNumber switch
    {
      5 => bossLevel5,
      10 => bossLevel10,
      15 => bossLevel15,
      _ => null
    };
  }

  public void LoadNextLevel()
  {
    int totalNormalLevels = normalLevels != null ? normalLevels.Length : 0;
    int totalBossLevels = 0;
    if (bossLevel5 != null && bossLevel5.Length > 0) totalBossLevels++;
    if (bossLevel10 != null && bossLevel10.Length > 0) totalBossLevels++;
    if (bossLevel15 != null && bossLevel15.Length > 0) totalBossLevels++;
    
    int maxLevelIndex = totalNormalLevels + totalBossLevels;
    if (maxLevelIndex == 0) return;
    
    currentLevelIndex = (currentLevelIndex + 1) % maxLevelIndex;
    Debug.Log($"[LevelManager] Advanced to level index: {currentLevelIndex} (Level {currentLevelIndex + 1})");
  }

  public void RestartFromLevelOne()
  {
    currentLevelIndex = 0;
  }
}
