
using UnityEngine;



public class GameController : MonoBehaviour {

  [SerializeField] private Knife knifePrefab;
  [SerializeField] private Transform spawnPoint;


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

  void ThrowKnife() {
    if (GameManager.Instance.State != GameState.Playing) {
      return;
    }

    Knife knifeObject = Instantiate(knifePrefab, spawnPoint.position, Quaternion.identity);
    knifeObject.Launch();
  }
}
