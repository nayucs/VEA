using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionUpdater : MonoBehaviour
{
    [Header("CSVファイル（TextAsset）")]
    public TextAsset csvFile;

    private List<Vector3> positions = new List<Vector3>();
    private int currentFrame = 0;
    private bool isPlaying = false;

    private float calculatedSpeed = 0f; // 生の速度
    private float smoothedSpeed = 0f;    // 滑らかにした速度

    private Queue<float> speedBuffer = new Queue<float>(); // 平均化用バッファ
    public int smoothingFrameCount = 5; // 何フレーム分を平均するか（例：5フレーム）

    public event Action OnPlaybackFinished;

    [Header("連携先")]
    public GazeBlurControllerExternalSpeed gazeBlurController;

    private float frameInterval = 0.033f; // 約30fps

    void Start()
    {
        if (csvFile != null)
        {
            LoadPositionsFromCSV(csvFile.text);
            if (positions.Count > 0)
            {
                transform.position = positions[0];
            }
        }
        else
        {
            Debug.LogError("CSVファイルがInspector上で設定されていません。");
        }
    }

    void LoadPositionsFromCSV(string csvText)
    {
        string[] lines = csvText.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] values = lines[i].Split(',');
            if (values.Length < 4) continue;

            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);

            positions.Add(new Vector3(x, y, z));
        }
    }

    public void StartPlayback()
    {
        if (!isPlaying && positions.Count > 1)
        {
            isPlaying = true;
            currentFrame = 1;
            StartCoroutine(UpdatePositionAndSpeed());
        }
    }

    IEnumerator UpdatePositionAndSpeed()
    {
        while (currentFrame < positions.Count)
        {
            Vector3 prevPos = transform.position;
            Vector3 nextPos = positions[currentFrame];

            transform.position = nextPos;

            float distance = Vector3.Distance(prevPos, nextPos);
            calculatedSpeed = distance / frameInterval;

            // バッファに追加して平均化
            speedBuffer.Enqueue(calculatedSpeed);
            if (speedBuffer.Count > smoothingFrameCount)
            {
                speedBuffer.Dequeue();
            }
            smoothedSpeed = 0f;
            foreach (float s in speedBuffer)
            {
                smoothedSpeed += s;
            }
            smoothedSpeed /= speedBuffer.Count;

            // DebugLog（スムーズな速度）
            //Debug.Log($"[PlayerPositionUpdater] 現在のスムーズ速度: {smoothedSpeed:F2} m/s");

            // GazeBlurControllerに渡すのはスムーズ速度
            if (gazeBlurController != null)
            {
                gazeBlurController.SetSpeed(smoothedSpeed);
            }

            currentFrame++;
            yield return new WaitForSeconds(frameInterval);
        }

        if (positions.Count > 0)
        {
            transform.position = positions[positions.Count - 1];
            calculatedSpeed = 0f;
            smoothedSpeed = 0f;
            if (gazeBlurController != null)
            {
                gazeBlurController.SetSpeed(0f);
            }
        }

        isPlaying = false;
        OnPlaybackFinished?.Invoke();
    }
}
