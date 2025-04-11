using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 7.08f;          // 25.5km/h を m/s に換算
    public float acceleration = 10f;        // 加速力
    public float deceleration = 15f;        // 減速力
    public float rotationSpeed = 90f;       // 1秒あたりの回転角度
    public float inputThreshold = 0.1f;     // スティックの無効ゾーン

    private Rigidbody rb;
    private Vector3 currentVelocity = Vector3.zero;

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
        Vector3 targetVelocity = inputDir * maxSpeed;

        if (inputDir.magnitude > 0)
        {
            // 加速
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            // 減速
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        // y成分は維持（ジャンプや重力なしなのでyは変化しないが念のため）
        rb.velocity = new Vector3(currentVelocity.x, rb.velocity.y, currentVelocity.z);
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
            dir += transform.forward;
        if (Input.GetKey(KeyCode.S) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown, OVRInput.Controller.LTouch))
            dir -= transform.forward;
        if (Input.GetKey(KeyCode.A) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.LTouch))
            dir -= transform.right;
        if (Input.GetKey(KeyCode.D) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.LTouch))
            dir += transform.right;

        return dir.normalized;
    }
}

