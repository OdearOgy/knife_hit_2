using UnityEngine;

public class KnifeThrower : MonoBehaviour
{
    [SerializeField] private Knife knife;
    [SerializeField] private Transform spawnPoint;

    private void OnEnable()
    {
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

      Knife knifeObject = Instantiate(knife, spawnPoint.position, Quaternion.identity);
      knifeObject.Launch();
    }
}
