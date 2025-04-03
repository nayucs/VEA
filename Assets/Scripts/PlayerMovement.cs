using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float accelerationTime = 5f; // 何秒かけてmaxSpeedに到達するか
    public float inputThreshold = 0.1f;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private float elapsedTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 inputDir = GetInputDirection();

        if (inputDir != Vector3.zero)
        {
            elapsedTime += Time.deltaTime;
            float speedFactor = Mathf.Clamp01(elapsedTime / accelerationTime);
            currentSpeed = Mathf.Lerp(0f, maxSpeed, speedFactor);

            // 移動方向に速度を与える
            Vector3 move = inputDir * currentSpeed;
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        }
        else
        {
            // 入力が止まったら速度もリセット（オプション）
            elapsedTime = 0f;
            currentSpeed = 0f;
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    Vector3 GetInputDirection()
    {
        Vector3 dir = Vector3.zero;

        // キーボード入力
        if (Input.GetKey(KeyCode.W))
        {
            dir += transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            dir -= transform.forward;
        }

        return dir.normalized;
    }
}

