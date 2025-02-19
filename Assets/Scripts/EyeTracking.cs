using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            if (Physics.Raycast(ray, out hit, 1000f))
            {
                // LogにHitしたオブジェクト名を出力
                Debug.Log("HitObject : " + hit.collider.gameObject.name);

                //Hitした位置にマーカーを移動
                GazePoint.transform.position = hit.point;

                // シェーダーに視線ヒット位置を渡す
                Shader.SetGlobalVector("_GazeHitPosition", hit.point);

            }
        }
    }
}