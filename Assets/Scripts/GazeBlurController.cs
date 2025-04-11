using UnityEngine;

public class GazeBlurController : MonoBehaviour
{
    public Material gazeBlurMaterial;
    public Rigidbody player; // à⁄ìÆÇ∑ÇÈÉvÉåÉCÉÑÅ[ÇÃRigidbody
    public float maxSpeed = 7.08f;
    public Vector2 clearRadiusRange = new Vector2(0.07f, 0.0602f);
    public Vector2 blurSizeRange = new Vector2(5f, 30f);

    void Update()
    {
        float speed = player.velocity.magnitude;
        float t = Mathf.InverseLerp(0f, maxSpeed, speed);

        float clearRadius = Mathf.Lerp(clearRadiusRange.x, clearRadiusRange.y, t);
        float blurSize = Mathf.Lerp(blurSizeRange.x, blurSizeRange.y, t);

        gazeBlurMaterial.SetFloat("_ClearRadius", clearRadius);
        gazeBlurMaterial.SetFloat("_BlurSize", blurSize);
    }
}
