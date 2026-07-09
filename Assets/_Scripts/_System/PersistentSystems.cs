using UnityEngine;

public class PersistentSystems : MonoBehaviour
{
  private void Awake()
  {
    DontDestroyOnLoad(gameObject);
  }
}
