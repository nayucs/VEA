using UnityEngine;

public class ModelFollower : MonoBehaviour
{
    public Transform cameraRig;       // OVRCameraRig本体
    public Transform playermodel;     // アバター見た目

    void LateUpdate()
    {
        // 高さを固定したい場合（例：Yだけ一定にする）
        Vector3 camPos = cameraRig.position;
        camPos.y = playermodel.position.y;

        playermodel.position = camPos;

        // 視線と同じ方向に回転させる（必要なら）
        playermodel.forward = Vector3.ProjectOnPlane(cameraRig.forward, Vector3.up).normalized;
    }
}
