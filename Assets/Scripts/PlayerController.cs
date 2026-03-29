using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public Transform focalPoint;
    public GameObject powerupRing;
    private Rigidbody rb;

    private InputAction moveAction;
    private InputAction smashAction;
    private InputAction breakAction;

    public bool hasPowerUp = false;
    public bool hasStunPowerUp = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        smashAction = InputSystem.actions.FindAction("Smash");
        breakAction = InputSystem.actions.FindAction("Break");
        powerupRing.SetActive(false);
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
        PowerupCheck();
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (hasPowerUp)
            {
                var eRb = collision.gameObject.GetComponent<Rigidbody>();
                var dir = collision.transform.position - transform.position;

                eRb.AddForce(100 * dir.normalized, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerUp = true;
            Destroy(other.gameObject);
            StartCoroutine(PowerUpCountDown());
            if (countDownRoutine != null)
            {
                StopCoroutine(countDownRoutine);
            }
            countDownRoutine = StartCoroutine(PowerUpCountDown());
        }
        if (other.CompareTag("PowerUp"))
        {
            hasStunPowerUp = true;
            Destroy(other.gameObject);
             if (countDownRoutine != null)
            {
                StopCoroutine(countDownRoutine);
            }
            countDownRoutine = StartCoroutine(PowerUpCountDown());
        }
    }
    
    void PowerupCheck()
    {
        if (hasPowerUp || hasStunPowerUp)
        {
            powerupRing.SetActive(true);
        }
        powerupRing.transform.position = transform.position;
    }

    private Coroutine countDownRoutine;
    IEnumerator PowerUpCountDown()
    {
        yield return new WaitForSeconds(10f);
        hasPowerUp = false;
        hasStunPowerUp = false;
        powerupRing.SetActive(false);
    }
}