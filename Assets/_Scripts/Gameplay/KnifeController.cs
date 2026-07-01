using UnityEngine;

public class Knife : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    private bool isMoving = false;
    private bool isStuck = false;

    [ContextMenu("Launch knife")]
    public void Launch() {
      isMoving = true;
    }

    void Update()
    {
        if (!isMoving || isStuck) {
          return;
        }

        transform.position += Vector3.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if (isStuck) {
        return;
      }

      if (other.CompareTag("Target")) {
        Stick(other.transform);
      }
    }

    private void Stick(Transform target) {
      isMoving = false;
      isStuck = true;

      transform.SetParent(target.Find("KnifeHolder"));
      transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
