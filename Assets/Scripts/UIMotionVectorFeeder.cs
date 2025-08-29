// Assets/Scripts/UIMotionVectorFeeder.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UIMotionVectorFeeder : MonoBehaviour
{
    [Header("ブラーの元になるワールド物体（手/武器/敵など）")]
    public Transform targetWorld;

    [Header("画面変換に使うカメラ（HMDのメインカメラ等）")]
    public Camera referenceCamera;

    [Header("速度の安定化")]
    [Tooltip("大きいほどなめらか（遅延は少し増える）")]
    public float smoothTime = 0.12f;   // 0.10-0.18 推奨
    [Tooltip("この速度未満は0扱い（微振動ブラー防止）")]
    public float speedClamp = 0.02f;

    [Header("方向が逆に見える場合はON")]
    public bool invertDirection = false;

    [Header("視線の適用（任意）")]
    public bool useGazeUV = false;
    public Vector2 fallbackGazeUV = new Vector2(0.5f, 0.5f);

    RawImage _ri;
    Material _matInstance;     // RawImageごとの独立マテリアル
    Vector2 _prevVP;           // 前フレームのViewport座標
    Vector2 _vel;              // 平滑化済みの速度ベクトル

    void Awake()
    {
        _ri = GetComponent<RawImage>();
        if (!_ri.material)
            Debug.LogWarning("[UIMotionVectorFeeder] RawImage にマテリアルが設定されていません。");

        // 共有マテリアルと干渉しないよう必ずインスタンス化
        _matInstance = Instantiate(_ri.material);
        _ri.material = _matInstance;

        if (!referenceCamera) referenceCamera = Camera.main;
    }

    void OnEnable()
    {
        if (targetWorld && referenceCamera)
            _prevVP = WorldToViewport(targetWorld.position);
        _vel = Vector2.zero;
    }

    void Update()
    {
        if (!targetWorld || !referenceCamera || _matInstance == null) return;

        // 1) 物体の画面座標（Viewport: 0..1）を取得
        Vector2 nowVP = WorldToViewport(targetWorld.position);

        // 2) 差分＝速度（ΔUV/フレーム）
        Vector2 raw = nowVP - _prevVP;

        // 3) 指数平滑化（なめらかに）
        float k = 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(1e-4f, smoothTime));
        _vel = Vector2.Lerp(_vel, raw, k);

        // 4) 微小ノイズは0扱い
        if (_vel.magnitude < speedClamp) _vel = Vector2.zero;

        // 5) 方向反転が必要ならここで反転
        Vector2 outDir = invertDirection ? -_vel : _vel;

        // 6) マテリアルへ設定（シェーダ側で _MotionDir を参照）
        _matInstance.SetVector("_MotionDir", new Vector4(outDir.x, outDir.y, 0, 0));

        // 7) 視線UV（任意）
        if (useGazeUV)
        {
            _matInstance.SetVector("_GazeUV", new Vector4(fallbackGazeUV.x, fallbackGazeUV.y, 0, 0));
        }

        _prevVP = nowVP;
    }

    Vector2 WorldToViewport(Vector3 worldPos)
    {
        var vp = referenceCamera.WorldToViewportPoint(worldPos);
        return new Vector2(vp.x, vp.y);
    }

    /// <summary>
    /// EyeTracking から注視UVを注入するための口（任意で毎フレーム呼ぶ）
    /// </summary>
    public void SetGazeUV(Vector2 uv)
    {
        if (_matInstance)
            _matInstance.SetVector("_GazeUV", new Vector4(uv.x, uv.y, 0, 0));
    }
}
