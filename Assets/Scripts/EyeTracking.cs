using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeTracking : MonoBehaviour
{
    OVREyeGaze eyeGaze;
    public GameObject GazePoint;
    private Camera HMDCamera;

    // スタート時に呼ばれる
    void Start()
    {
        eyeGaze = GetComponent<OVREyeGaze>();
        HMDCamera = Camera.main;
    }

    // フレーム更新毎に呼ばれる
    void Update()
    {
        if (eyeGaze == null) return;

        //HMDの方向と位置を取得
        Vector3 HMDPosition = HMDCamera.transform.position;
        Quaternion HMDRotation = HMDCamera.transform.rotation;

        if (eyeGaze.EyeTrackingEnabled)
        {
            //視線の方向ベクトルを取得
            Vector3 GazeDirection = eyeGaze.transform.forward;

            //HMDの方向ベクトルと視線の方向ベクトルの和をとる
            Vector3 NewDirection = HMDCamera.transform.forward + GazeDirection;

            //視線の先にRayを飛ばす
            Ray ray = new Ray(HMDPosition, GazeDirection);

            //RayがHitしたオブジェクトの情報を格納用
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("UI")))
            {
                GazePoint.transform.position = hit.point;

                // UIのRawImageかチェック
                RawImage rawImage = hit.collider.GetComponent<RawImage>();
                RectTransform rectTransform = hit.collider.GetComponent<RectTransform>();

                if (rawImage != null && rectTransform != null)
                {
                    Vector2 localPoint;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        rectTransform,
                        HMDCamera.WorldToScreenPoint(hit.point),
                        HMDCamera,
                        out localPoint))
                    {
                        Vector2 uv = LocalToUV(rectTransform, localPoint);
                        //Debug.Log($"Hit UV座標: {uv}");
                        //Debug.Log("WorldToScreenPoint Y: " + HMDCamera.WorldToScreenPoint(hit.point).y);

                        Material mat = rawImage.material;
                        if (mat != null)
                        {
                            mat.SetVector("_GazeUV", new Vector4(uv.x, uv.y, 0, 0));
                        }
                    }
                }
            }
        }
    }

    Vector2 LocalToUV(RectTransform rectTransform, Vector2 localPoint)
    {
        Vector2 pivot = rectTransform.pivot;
        Vector2 size = rectTransform.rect.size;

        float u = (localPoint.x / size.x) + pivot.x;
        float v = (localPoint.y / size.y) + pivot.y;

        return new Vector2(u, v);
    }
}