using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private LevelConfig[] allLevels;

    private int currentLevelIndex = 0;

    public LevelConfig CurrentLevel => allLevels != null && currentLevelIndex < allLevels.Length ? allLevels[currentLevelIndex] : null;

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
        currentLevelIndex = (currentLevelIndex + 1) % allLevels.Length;
    }

    public void RestartLevel()
    {
        // Stay on current level
    }
}
