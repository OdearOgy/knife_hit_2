using UnityEngine;

public class KnifeThrower : MonoBehaviour
{
    [SerializeField] private Knife knife;
    [SerializeField] private Transform spawnPoint;

    private GameInputActions input;

    private void Awake()
    {
        input = new GameInputActions();
    }

    private void OnEnable()
    {
      InputManager.Instance.OnTap += ThrowKnife;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null) {
          InputManager.Instance.OnTap -= ThrowKnife;
        }
    }

    void ThrowKnife() {
      Knife knifeObject = Instantiate(knife, spawnPoint.position, Quaternion.identity);
      knifeObject.Launch();
    }
}
