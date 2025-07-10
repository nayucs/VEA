using UnityEngine;

public class ScaledPlayerMovement : MonoBehaviour
{
    public Transform centerEyeAnchor; // OVRCameraRig/TrackingSpace/CenterEyeAnchor
    public float movementScale = 2.0f;

    private Vector3 lastCenterEyeWorldPos;
    private bool isInitialized = false;

    void Update()
    {
        if (centerEyeAnchor == null) return;

        if (!isInitialized)
        {
            lastCenterEyeWorldPos = centerEyeAnchor.position;
            isInitialized = true;
            return;
        }

        // 現実空間でのHMD移動量をワールド空間で取得
        Vector3 deltaWorld = centerEyeAnchor.position - lastCenterEyeWorldPos;

        deltaWorld.y = 0;

        // 仮想空間のプレイヤー全体を、スケーリングされた差分で移動
        transform.position += deltaWorld * (movementScale - 1.0f);

        lastCenterEyeWorldPos = centerEyeAnchor.position;
    }
}
