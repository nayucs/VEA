using UnityEngine;

public class MouseMaskWithBlurController : MonoBehaviour
{
    public Material blurMaterial; // シェーダーが適用されたマテリアル
    public float maskRadius = 0.2f; // マスクの半径
    public float blurAmount = 10f; // ぼかしの強さ

    private Camera _camera;

    void Start()
    {
        if (blurMaterial == null)
        {
            Debug.LogError("Blur material is not assigned.");
            enabled = false;
            return;
        }

        // メインカメラを取得
        _camera = Camera.main;
        if (_camera == null)
        {
            Debug.LogError("Main Camera not found.");
            enabled = false;
        }
    }

    void Update()
    {
        if (_camera == null) return;

        // マウスのスクリーン座標を取得
        Vector3 mousePosition = Input.mousePosition;
        Vector2 normalizedMousePos = new Vector2(
            mousePosition.x / Screen.width,
            mousePosition.y / Screen.height
        );

        // シェーダーにマウス位置を渡す
        blurMaterial.SetVector("_MousePos", new Vector4(normalizedMousePos.x, normalizedMousePos.y, 0, 0));

        // マスク半径とぼかし量をシェーダーに渡す
        blurMaterial.SetFloat("_MaskRadius", maskRadius);
        blurMaterial.SetFloat("_Blur", blurAmount);
    }
}
