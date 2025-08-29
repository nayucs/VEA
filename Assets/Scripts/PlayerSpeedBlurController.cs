using UnityEngine;

public class PlayerSpeedBlurController : MonoBehaviour
{
    [Header("速度[m/s] → ブラー半径[px]")]
    public AnimationCurve speedToRadius = AnimationCurve.Linear(0, 0, 10, 16);
    [Range(0, 64)] public float maxRadiusPx = 24f;

    [Header("安定化")]
    [Tooltip("外部から既に平滑化済みでも、軽く慣性を入れると見栄えが安定")]
    public float additionalSmoothing = 0.08f; // 秒
    [Tooltip("この速度未満はブラーなし")]
    public float minSpeedThreshold = 0.05f;

    [Header("有効/無効")]
    public bool blurEnabled = true;

    float _currentSpeed;
    float _currentRadius;

    /// <summary>
    /// PlayerPositionUpdater などから速度をセットする
    /// </summary>
    public void SetSpeed(float metersPerSecond)
    {
        _currentSpeed = Mathf.Max(0f, metersPerSecond);
    }

    void Update()
    {
        if (!blurEnabled)
        {
            Shader.SetGlobalFloat("_GSB_Enabled", 0f);
            return;
        }

        // 速度から目標半径を計算
        float spd = _currentSpeed;
        float targetRadius = (spd < minSpeedThreshold) ? 0f
            : Mathf.Min(maxRadiusPx, speedToRadius.Evaluate(spd));

        // スムージング（指数平滑）
        float k = 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(1e-4f, additionalSmoothing));
        _currentRadius = Mathf.Lerp(_currentRadius, targetRadius, k);

        // グローバルに渡す
        Shader.SetGlobalFloat("_GSB_Enabled", (_currentRadius > 0.01f) ? 1f : 0f);
        Shader.SetGlobalFloat("_GSB_RadiusPx", _currentRadius);
    }
}
