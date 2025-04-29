using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeTracking : MonoBehaviour
{
    OVREyeGaze eyeGaze;
    public GameObject GazePoint;
    private Camera HMDCamera;

    private Vector2 currentGazeUV = Vector2.zero; // 視線位置の保存
    public Vector2 CurrentGazeUV => currentGazeUV; // 外部参照用プロパティ

    void Start()
    {
        eyeGaze = GetComponent<OVREyeGaze>();
        HMDCamera = Camera.main;
    }

    void Update()
    {
        if (eyeGaze == null) return;

        Vector3 HMDPosition = HMDCamera.transform.position;
        Quaternion HMDRotation = HMDCamera.transform.rotation;

        if (eyeGaze.EyeTrackingEnabled)
        {
            Vector3 GazeDirection = eyeGaze.transform.forward;
            Vector3 NewDirection = HMDCamera.transform.forward + GazeDirection;
            Ray ray = new Ray(HMDPosition, GazeDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("UI")))
            {
                GazePoint.transform.position = hit.point;

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
                        currentGazeUV = uv; // ここで更新

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
