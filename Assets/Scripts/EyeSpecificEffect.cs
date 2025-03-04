using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeSpecificEffect : MonoBehaviour
{
    public Camera leftEyeCamera;
    public Camera rightEyeCamera;

    public Material eyeShaderMaterial; // 目ごとのシェーダーマテリアル

    void Start()
    {
        if (leftEyeCamera == null || rightEyeCamera == null)
        {
            Debug.LogError("LeftEyeCamera または RightEyeCamera が設定されていません！");
            return;
        }

        // 左目カメラにマテリアル適用
        leftEyeCamera.GetComponent<Camera>().targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        leftEyeCamera.GetComponent<Camera>().targetTexture.Create();
        eyeShaderMaterial.SetInt("_StereoEyeIndex", 0); // 左目

        // 右目カメラにマテリアル適用
        rightEyeCamera.GetComponent<Camera>().targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        rightEyeCamera.GetComponent<Camera>().targetTexture.Create();
        eyeShaderMaterial.SetInt("_StereoEyeIndex", 1); // 右目
    }
}
