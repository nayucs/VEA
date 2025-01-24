using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class BlurEdgesEffect : MonoBehaviour
{
    public Material material;
    public float effectRadius = 0.5f;
    public float blurStrength = 5.0f;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            // 中心をスクリーンスペースで設定
            Vector2 center = new Vector2(0.5f, 0.5f);
            material.SetVector("_Center", center);

            // 半径と強度を設定
            material.SetFloat("_Radius", effectRadius);
            material.SetFloat("_BlurStrength", blurStrength);

            // マテリアルを使用してエフェクトを適用
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
