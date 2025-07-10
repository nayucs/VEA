using UnityEngine;

public class SpeedTracker : MonoBehaviour
{
    public GazeBlurControllerExternalSpeed blurController;
    private Vector3 lastPosition;
    private float smoothedSpeed = 0f;

    [Range(0f, 1f)]
    public float smoothingFactor = 0.1f; // 0に近いほど滑らか、1に近いほど敏感

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 delta = transform.position - lastPosition;
        float rawSpeed = delta.magnitude / Time.deltaTime;

        // スムージング（指数移動平均）
        smoothedSpeed = Mathf.Lerp(smoothedSpeed, rawSpeed, smoothingFactor);

        if (blurController != null)
        {
            blurController.SetSpeed(smoothedSpeed);
        }

        // 速度をコンソールに出力
        Debug.Log($"[SpeedTracker] Current Speed: {smoothedSpeed:F2}");

        lastPosition = transform.position;
    }
}
