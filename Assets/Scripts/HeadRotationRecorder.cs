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

    [Header("被験者ID（例：P001）")]
    public string participantID = "P001";

    [Header("視野内判定する対象オブジェクト")]
    [SerializeField] private GameObject targetObject;

    private Camera mainCamera;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        headTransform = GameObject.Find("CenterEyeAnchor")?.transform;
        eyeTracking = GameObject.FindObjectOfType<EyeTracking>();
        mainCamera = Camera.main;

        if (headTransform == null)
            Debug.LogError("CenterEyeAnchor が見つかりません。");

        if (eyeTracking == null)
            Debug.LogError("EyeTracking スクリプトが見つかりません。");

        if (mainCamera == null)
            Debug.LogError("Main Camera が見つかりません。");
    }

    void Update()
    {
        if (!isRecording || headTransform == null) return;

        float elapsedTime = Time.time - startTime;
        Vector3 eulerAngles = headTransform.rotation.eulerAngles;
        Vector2 gazeUV = eyeTracking != null ? eyeTracking.CurrentGazeUV : Vector2.zero;

        int inView = IsTargetInView() ? 1 : 0;

        logLines.Add($"{elapsedTime:F3},{eulerAngles.x:F2},{eulerAngles.y:F2},{eulerAngles.z:F2},{gazeUV.x:F3},{gazeUV.y:F3},{inView}");
    }

    bool IsTargetInView()
    {
        if (targetObject == null || mainCamera == null) return false;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(targetObject.transform.position);
        return viewportPos.z > 0 && viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1;
    }

    public void StartRecording()
    {
        if (headTransform == null) return;

        logLines.Clear();
        startTime = Time.time;
        logLines.Add("time,x,y,z,gaze_u,gaze_v,in_view"); // ← ヘッダーに in_view を追加
        isRecording = true;
        Debug.Log($"被験者 {participantID} の記録を開始しました。");
    }

    public void StopRecording()
    {
        isRecording = false;
        Debug.Log($"被験者 {participantID} の記録を停止しました。");
    }

    public void SaveRecording()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"HeadRotationLog_{participantID}_{timestamp}.csv";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            File.WriteAllLines(filePath, logLines);
            Debug.Log($"記録を保存しました: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError("記録保存に失敗: " + e.Message);
        }
    }
}

