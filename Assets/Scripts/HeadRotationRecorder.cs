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
    private EyeTracking eyeTracking; // EyeTracking の参照

    [Header("被験者ID（例：P001）")]
    public string participantID = "P001";  // Inspector で指定可能に

    void Start()
    {
        headTransform = GameObject.Find("CenterEyeAnchor")?.transform;
        eyeTracking = GameObject.FindObjectOfType<EyeTracking>();

        if (headTransform == null)
        {
            Debug.LogError("CenterEyeAnchor が見つかりません。OVRCameraRig を確認してください。");
        }

        if (eyeTracking == null)
        {
            Debug.LogError("EyeTracking スクリプトが見つかりません。");
        }
    }

    void Update()
    {
        if (!isRecording || headTransform == null) return;

        float elapsedTime = Time.time - startTime;
        Vector3 eulerAngles = headTransform.rotation.eulerAngles;
        Vector2 gazeUV = eyeTracking != null ? eyeTracking.CurrentGazeUV : Vector2.zero;

        logLines.Add($"{elapsedTime:F3},{eulerAngles.x:F2},{eulerAngles.y:F2},{eulerAngles.z:F2},{gazeUV.x:F3},{gazeUV.y:F3}");
    }

    public void StartRecording()
    {
        if (headTransform == null) return;

        logLines.Clear();
        startTime = Time.time;

        // ヘッダーを追加（任意）
        logLines.Add("time,x,y,z,gaze_u,gaze_v");

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
}
