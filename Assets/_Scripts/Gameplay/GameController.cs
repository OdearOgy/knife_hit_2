
using UnityEngine;



public class GameController : MonoBehaviour {
  [SerializeField] private float slideInSpeed = 25f;
  [SerializeField] private float spawnOffset = 2.5f;


  [SerializeField] private Knife knifePrefab;
  [SerializeField] private Transform spawnPoint;
  private Knife currentKnife;

  private void OnEnable() {
    if (InputManager.Instance != null) {
      InputManager.Instance.OnTap += ThrowKnife;

    }
  }

  private void OnDisable()
  {
      if (InputManager.Instance != null) {
        InputManager.Instance.OnTap -= ThrowKnife;
      }
  }

  public void OnSpawnKnife() {
    Debug.Log("[GameController] OnSpawnKnife() → spawning next knife");
    currentKnife = null;
    SpawnKnife();
  }

  public void OnKnifeMissed() {
    Debug.Log("[GameController] OnKnifeMissed() → game over, no new spawn");
    currentKnife = null;
  }

  void Start() {
    Debug.Log("[GameController] Start()");
    SpawnKnife();
  }

  void SpawnKnife() {
    Vector3 initialSpawnPoint = spawnPoint.position - Vector3.up * spawnOffset;
    Debug.Log($"[GameController] SpawnKnife() at {initialSpawnPoint} targeting {spawnPoint.position}");

    currentKnife = Instantiate(knifePrefab, initialSpawnPoint, Quaternion.identity);
    currentKnife.SetController(this);
    currentKnife.Prepare(spawnPoint.position, slideInSpeed);
  }

  void ThrowKnife() {
    if (GameManager.Instance.State != GameState.Playing) {
      Debug.Log("[GameController] ThrowKnife() ignored (GameState != Playing)");
      return;
    }

    if (currentKnife == null) {
      Debug.Log("[GameController] ThrowKnife() ignored (currentKnife is null)");
      return;
    }

    Debug.Log($"[GameController] ThrowKnife() → currentKnife.state={currentKnife.State}");
    currentKnife.Throw();
  }
}
