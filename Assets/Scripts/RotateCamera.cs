using UnityEngine;
using UnityEngine.InputSystem;

// 1.1 START HERE
public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed;

    private InputAction moveAction;

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        transform.Rotate(Vector3.up, moveInput.x * rotationSpeed * Time.deltaTime);
    }
}