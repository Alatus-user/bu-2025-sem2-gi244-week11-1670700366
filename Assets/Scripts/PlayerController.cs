using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform focalPoint;

    public float speed = 5f;

    private Rigidbody rb;

    private InputAction moveAction;
    private InputAction smashAction;
    private InputAction breakAction;

    public bool isPowerUp= false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        smashAction = InputSystem.actions.FindAction("Smash");
        breakAction = InputSystem.actions.FindAction("Break");
    }

    // Update is called once per frame
    void Update()
    {
        var move = moveAction.ReadValue<Vector2>();
        rb.AddForce(move.y * speed * focalPoint.forward);
        rb.AddForce(move.x * speed * focalPoint.right);

        if (breakAction.IsPressed())
        {
            rb.linearVelocity = Vector3.zero;
        }
       

    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            if (isPowerUp)
            {
                var rb = other.gameObject.GetComponent<Rigidbody>();
                var dir = (other.gameObject.transform.position - transform.position).normalized;
                rb.AddForce(dir * 10f, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            isPowerUp = true;
            Destroy(other.gameObject);
            if (countDownRoutine != null)
            {
                StopCoroutine(countDownRoutine);
            }
            countDownRoutine = StartCoroutine(PowerUpCountDown());
        }
    }

    private Coroutine countDownRoutine;

    IEnumerator PowerUpCountDown()
    {
        yield return new WaitForSeconds(5f);
        isPowerUp = false;
    }
}
