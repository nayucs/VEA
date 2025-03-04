using UnityEngine;
using UnityEngine.Rendering;

public class BlurEffectVR : MonoBehaviour
{
    public Material blurMaterial;  // ぼかし用マテリアル
    private Camera cam;
    private RenderTexture renderTexture;

    void Start()
    {
        cam = Camera.main;
        renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        cam.targetTexture = renderTexture;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, renderTexture, blurMaterial);  // ブラーエフェクト適用
        Graphics.Blit(renderTexture, dest);
    }
}