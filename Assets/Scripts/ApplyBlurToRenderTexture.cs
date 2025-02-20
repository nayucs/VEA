using UnityEngine;
using UnityEngine.UI;

public class ApplyBlurToRenderTexture : MonoBehaviour
{
    public Camera sourceCamera; // 別カメラ
    public RenderTexture sourceTexture; // 別カメラのRenderTexture
    public Material blurMaterial; // ブラー適用マテリアル
    public RawImage targetUI; // UIに適用

    private RenderTexture blurredTexture;

    void Start()
    {
        // ブラー用のRenderTextureを作成
        blurredTexture = new RenderTexture(sourceTexture.width, sourceTexture.height, 0);
        targetUI.texture = blurredTexture;
    }

    void Update()
    {
        if (sourceTexture != null && blurMaterial != null)
        {
            // ブラーを適用
            Graphics.Blit(sourceTexture, blurredTexture, blurMaterial);
        }
    }
}
