using UnityEngine;

public enum KnifeState {
  Unset,
  Prepared,
  Thrown,
  Stuck,
}


public class Knife : MonoBehaviour
{
    public KnifeState State {get; private set;} = KnifeState.Unset;

    [SerializeField] private float speed = 12f;

    private Vector3 slideTarget;
    private float slideSpeed;



    void Update() {
        if (State == KnifeState.Unset) {
          transform.position = Vector3.MoveTowards(transform.position, slideTarget, slideSpeed * Time.deltaTime);


          if (Vector3.Distance(transform.position, slideTarget) < 0.01f) {
            State = KnifeState.Prepared;
            GetComponent<Collider2D>().enabled = true;
          }
        }


        if (State == KnifeState.Thrown || State == KnifeState.Stuck) {
          return;
        }

        transform.position += Vector3.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if (State == KnifeState.Stuck) {
        return;
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
      if (State == KnifeState.Unset) {
        return;
      }

      State = KnifeState.Thrown;
    }

    private void Stick(Transform target) {
      State = KnifeState.Stuck;

      transform.SetParent(target.Find("KnifeHolder"));
      transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
