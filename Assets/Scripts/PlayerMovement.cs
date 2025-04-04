using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float accelerationTime = 5f;
    public float inputThreshold = 0.1f;
    public float rotationSpeed = 90f; // 1•b‚ ‚½‚è‰½“x‰ñ“]‚·‚é‚©

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private float elapsedTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        Vector3 inputDir = GetInputDirection();

        if (inputDir != Vector3.zero)
        {
            elapsedTime += Time.deltaTime;
            float speedFactor = Mathf.Clamp01(elapsedTime / accelerationTime);
            currentSpeed = Mathf.Lerp(0f, maxSpeed, speedFactor);

            Vector3 move = inputDir * currentSpeed;
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        }
        else
        {
            elapsedTime = 0f;
            currentSpeed = 0f;
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void HandleRotation()
    {
        float rightStickX = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).x;

        if (Mathf.Abs(rightStickX) > inputThreshold)
        {
            float rotationAmount = rightStickX * rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, rotationAmount, 0f);
        }
    }

    Vector3 GetInputDirection()
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp, OVRInput.Controller.LTouch))
        {
            dir += transform.forward;
        }

        if (Input.GetKey(KeyCode.S) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown, OVRInput.Controller.LTouch))
        {
            dir -= transform.forward;
        }

        if (Input.GetKey(KeyCode.A) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.LTouch))
        {
            dir -= transform.right;
        }
        if (Input.GetKey(KeyCode.D) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.LTouch))
        {
            dir += transform.right;
        }

        return dir.normalized;
    }
}

