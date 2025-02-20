using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MotionBlurEffect : MonoBehaviour
{
    public Shader motionBlurShader;
    private Material motionBlurMaterial;
    private RenderTexture previousFrameRT;
    [Range(0.0f, 1.0f)]
    public float blurAmount = 0.5f;

    void Start()
    {
        motionBlurMaterial = new Material(motionBlurShader);
        previousFrameRT = new RenderTexture(Screen.width, Screen.height, 0);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // 前フレームのテクスチャをシェーダーに渡す
        motionBlurMaterial.SetTexture("_PrevFrameTex", previousFrameRT);
        motionBlurMaterial.SetFloat("_BlurAmount", blurAmount);

        // 現在の映像をブラー処理
        Graphics.Blit(source, destination, motionBlurMaterial);

        // 現在のフレームを保存
        Graphics.Blit(destination, previousFrameRT);
    }
}
