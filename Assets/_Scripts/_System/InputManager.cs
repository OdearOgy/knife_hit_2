using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private GameInputActions input;
    public System.Action OnTap;

   private void Awake() {
     if (Instance != null) {
       Destroy(gameObject);
       return;
     }

     Instance = this;
     DontDestroyOnLoad(gameObject);

     input = new GameInputActions();
    }

    private void OnEnable() {
      input.GameplayActions.Tap.performed += HandleTap;
      input.Enable();
    }

    private void OnDisable() {
      input.GameplayActions.Tap.performed -= HandleTap;
      input.Disable();
    }

    private void HandleTap(InputAction.CallbackContext ctx) {
      OnTap?.Invoke();
    }
}
