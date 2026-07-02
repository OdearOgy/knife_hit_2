using UnityEngine;

public class TargetController : MonoBehaviour
{

  [SerializeField] private float rotationSpeed = 50f;
  [SerializeField] private bool isClockwise = true; // refactor to direction enum

  private float directionMultiplier => isClockwise ? -1f : 1f;

  void Update()
  {
    float zIndex = directionMultiplier * rotationSpeed * Time.deltaTime;
    transform.Rotate(0f, 0f, zIndex);
  }
}
