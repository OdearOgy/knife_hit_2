using UnityEngine;

public class FPSLimiter : MonoBehaviour
{

  public static FPSLimiter Instance { get; private set; }

  void Awake()
  {

    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);

    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = 60;
  }
}
