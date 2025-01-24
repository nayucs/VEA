using UnityEngine;

[ExecuteInEditMode]
public class BlurController : MonoBehaviour
{
    [SerializeField]
    private Material blurMaterial; // ActiveBlurマテリアル

    [SerializeField]
    private Camera mainCamera; // 使用するカメラ

    [SerializeField, Range(0.01f, 1f)]
    private float radius = 0.1f; // ぼかし解除の半径

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (blurMaterial == null || mainCamera == null || MousePosition.Instance == null)
        {
            return;
        }

        // MousePositionスクリプトからスクリーン座標を取得
        Vector3 Position = MousePosition.Instance.GetMousePosition();

        // スクリーン座標を正規化
        Vector2 normalizedPos = new Vector2(
            Position.x / Screen.width,
            Position.y / Screen.height
        );

        // シェーダーに値を渡す
        blurMaterial.SetVector("_Pos", normalizedPos);
        blurMaterial.SetFloat("_Radius", radius);
    }
}
