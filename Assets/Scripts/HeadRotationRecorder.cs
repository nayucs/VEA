using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class HeadRotationRecorder : MonoBehaviour
{
    private List<string> logLines = new List<string>();
    private bool isRecording = false;
    private Transform headTransform;
    private float startTime;
    private EyeTracking eyeTracking;

    private Camera HMDCamera;
    public GameObject targetObject;

    [Header("被験者ID（例：P001）")]
    public string participantID = "P001";

    void Start()
    {
        headTransform = GameObject.Find("CenterEyeAnchor")?.transform;
        eyeTracking = GameObject.FindObjectOfType<EyeTracking>();
        HMDCamera = Camera.main;

        if (headTransform == null)
        {
            Debug.LogError("CenterEyeAnchor が見つかりません。OVRCameraRig を確認してください。");
        }

        if (eyeTracking == null)
        {
            Debug.LogError("EyeTracking スクリプトが見つかりません。");
        }

        if (HMDCamera == null)
        {
            Debug.LogError("Main Camera が見つかりません。");
        }
    }

    void Update()
    {
        if (!isRecording || headTransform == null) return;

        float elapsedTime = Time.time - startTime;
        Vector3 eulerAngles = headTransform.rotation.eulerAngles;
        Vector2 gazeUV = eyeTracking != null ? eyeTracking.CurrentGazeUV : Vector2.zero;

        // オブジェクトの視野内判定（列名を ball_in_view に）
        string ballInView = (targetObject != null && IsObjectVisible(HMDCamera, targetObject)) ? "1" : "0";

        logLines.Add($"{elapsedTime:F3},{eulerAngles.x:F2},{eulerAngles.y:F2},{eulerAngles.z:F2},{gazeUV.x:F3},{gazeUV.y:F3},{ballInView}");
    }

    public void StartRecording()
    {
        if (headTransform == null) return;

        logLines.Clear();
        startTime = Time.time;

        logLines.Add("time,x,y,z,gaze_u,gaze_v,ball_in_view");

        isRecording = true;
        Debug.Log($"被験者 {participantID} の記録を開始しました。");
    }

    public void StopAndSave()
    {
        isRecording = false;

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"HeadRotationLog_{participantID}_{timestamp}.csv";

        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            File.WriteAllLines(filePath, logLines);
            Debug.Log($"記録を保存しました: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("記録保存に失敗: " + e.Message);
        }
    }

    bool IsObjectVisible(Camera cam, GameObject obj)
    {
        if (obj == null || cam == null) return false;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, obj.GetComponent<Renderer>().bounds);
    }
}
