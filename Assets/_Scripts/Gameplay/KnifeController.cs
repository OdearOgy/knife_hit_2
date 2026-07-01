using UnityEngine;

public enum KnifeState {
  Unset,
  Prepared,
  Thrown,
  Stuck,
  Falling
}


public class Knife : MonoBehaviour
{

    public KnifeState State {get; private set;} = KnifeState.Unset;

    [SerializeField] private float speed = 12f;

    private Vector3 slideTarget;
    private float slideSpeed;

    private GameController controller;
    public void SetController(GameController c) => controller = c;


    void Update() {
        switch (State) {
          case KnifeState.Unset:
            transform.position = Vector3.MoveTowards(transform.position, slideTarget, slideSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, slideTarget) < 0.01f) {
              State = KnifeState.Prepared;
              GetComponent<Collider2D>().enabled = true;
            }
          break;

          case KnifeState.Thrown:
            transform.position += Vector3.up * speed * Time.deltaTime;
          break;

          case KnifeState.Falling:
            FallDown();
          break;

        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if (State == KnifeState.Stuck) {
        return;
      }

      if (other.CompareTag("Knife") && State == KnifeState.Thrown) {
        State = KnifeState.Falling;
        GameManager.Instance.SetState(GameState.Lost);
        controller?.OnKnifeMissed();
      }

      if (other.CompareTag("Target")) {
        Stick(other.transform);
      }

    }

    public void Prepare(Vector3 target, float speed) {
      slideTarget = target;
      slideSpeed = speed;
      GetComponent<Collider2D>().enabled = false;
    }

    public void Throw() {
      if (State == KnifeState.Stuck) {
        return;
      }

      State = KnifeState.Thrown;
    }

    private void Stick(Transform target) {

      State = KnifeState.Stuck;

      transform.SetParent(target.Find("KnifeHolder"));
      transform.position = new Vector3(transform.position.x, transform.position.y, 0);

      controller?.OnSpawnKnife();
    }

    private void FallDown() {
      transform.position += Vector3.down * (slideSpeed / 10) * Time.deltaTime;
      transform.Rotate(0, 0, 360 * Time.deltaTime);
    }
}
