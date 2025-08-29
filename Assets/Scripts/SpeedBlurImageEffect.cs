using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SpeedBlurImageEffect : MonoBehaviour
{
    [Tooltip("Hidden/GazeSpeedBlur (Built-in用) を使うマテリアル")]
    public Material blurMaterial;

    [Range(0.25f, 1f)]
    public float downsample = 1f;   // 0.5で半解像度（高速化）
    [Range(1, 32)]
    public int sampleCount = 8;     // 4-12目安

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (blurMaterial == null || Shader.GetGlobalFloat("_GSB_Enabled") < 0.5f)
        {
            Graphics.Blit(src, dst); // 無効時は素通し
            return;
        }

        blurMaterial.SetInt("_GSB_Samples", sampleCount);

        int w = src.width, h = src.height;
        if (downsample < 0.999f) { w = Mathf.Max(1, (int)(w * downsample)); h = Mathf.Max(1, (int)(h * downsample)); }

        var tmpA = RenderTexture.GetTemporary(w, h, 0, src.format);
        var tmpB = RenderTexture.GetTemporary(w, h, 0, src.format);

        // 横ブラー（pass 0）
        Graphics.Blit(src, tmpA, blurMaterial, 0);
        // 縦ブラー（pass 1）
        Graphics.Blit(tmpA, tmpB, blurMaterial, 1);
        // 出力
        Graphics.Blit(tmpB, dst);

        RenderTexture.ReleaseTemporary(tmpA);
        RenderTexture.ReleaseTemporary(tmpB);
    }
}
