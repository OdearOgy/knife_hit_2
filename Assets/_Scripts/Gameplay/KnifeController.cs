using UnityEngine;

public enum KnifeState {
  Unset,
  Queued,
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
          case KnifeState.Queued:
            transform.position = Vector3.MoveTowards(transform.position, slideTarget, slideSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, slideTarget) < 0.01f) {
              GetComponent<Collider2D>().enabled = true;

              if (State == KnifeState.Queued) {
                  Debug.Log($"[Knife] {name}: Queued → Thrown (auto-throw at spawn)");
                  State = KnifeState.Thrown;
              } else {
                  Debug.Log($"[Knife] {name}: Unset → Prepared (at spawn)");
                  State = KnifeState.Prepared;
              }

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
      Debug.Log($"[Knife] {name}: OnTriggerEnter2D with {other.name} (tag={other.tag}) while state={State}");
      if (State == KnifeState.Thrown) {
        if (other.CompareTag("Knife")) {
          Debug.Log($"[Knife] {name}: HIT ANOTHER KNIFE → Falling");
          State = KnifeState.Falling;
          GameManager.Instance.SetState(GameState.Lost);
          controller?.OnKnifeMissed();
        } else if (other.CompareTag("Target")) {
          Debug.Log($"[Knife] {name}: HIT TARGET → Stick()");
          Stick(other.transform);
        }
      }

    }

    public void Prepare(Vector3 target, float speed) {
      slideTarget = target;
      slideSpeed = speed;
      GetComponent<Collider2D>().enabled = false;
    }

    public void Throw() {
      if (State == KnifeState.Stuck || State == KnifeState.Falling) {
        Debug.Log($"[Knife] {name}: Throw() ignored (state={State})");
        return;
      }

      if (State == KnifeState.Unset) {
        Debug.Log($"[Knife] {name}: Throw() while Unset → Queued (buffered)");
        State = KnifeState.Queued;
      } else if (State == KnifeState.Prepared) {
        Debug.Log($"[Knife] {name}: Throw() while Prepared → Thrown");
        State = KnifeState.Thrown;
      } else {
        Debug.Log($"[Knife] {name}: Throw() ignored (state={State})");
      }
    }

    private void Stick(Transform target) {
      Debug.Log($"[Knife] {name}: Stick() called on Target={target.name}");
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
