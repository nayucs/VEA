using UnityEngine;

public class GazeBlurControllerExternalSpeed : MonoBehaviour
{
    public Material gazeBlurMaterial;
    public float maxSpeed = 7.08f;
    public Vector2 clearRadiusRange = new Vector2(0.2f, 0.172f);
    public Vector2 blurSizeRange = new Vector2(5f, 30f);

    private float externalSpeed = 0f; // ŠO•”‚©‚ç“n‚³‚ê‚½‘¬“x

    // PlayerPositionUpdater‚©‚ç‚±‚ÌŠÖ”‚Å‘¬“x‚ğ“n‚µ‚Ä‚à‚ç‚¤
    public void SetSpeed(float speed)
    {
        externalSpeed = speed;
    }

    void Update()
    {
        float t = Mathf.InverseLerp(0f, maxSpeed, externalSpeed);

        float clearRadius = Mathf.Lerp(clearRadiusRange.x, clearRadiusRange.y, t);
        float blurSize = Mathf.Lerp(blurSizeRange.x, blurSizeRange.y, t);

        gazeBlurMaterial.SetFloat("_ClearRadius", clearRadius);
        gazeBlurMaterial.SetFloat("_BlurSize", blurSize);
    }
}
