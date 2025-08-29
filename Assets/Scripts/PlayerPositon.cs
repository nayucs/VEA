using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    [Header("CSVファイル（TextAsset）")]
    public TextAsset csvFile;

    [Header("再生設定")]
    [Tooltip("1フレームあたりの秒数 (例: 30fps=0.033, 60fps=0.0167)")]
    [SerializeField] private float frameInterval = 0.033f;

    private readonly List<Vector3> positions = new List<Vector3>();
    private int currentFrame = 0;
    private bool isPlaying = false;

    public event Action OnPlaybackFinished;

    void Start()
    {
        if (csvFile != null)
        {
            LoadPositionsFromCSV(csvFile.text);
            if (positions.Count > 0)
            {
                transform.position = positions[0];
                // 自動で再生を始めたいならここで呼ぶ
                StartPlayback();
            }
            else
            {
                Debug.LogWarning("[PositionUpdater] CSVに有効なデータがありません");
            }
        }
        else
        {
            Debug.LogError("[PositionUpdater] CSVファイルがInspectorで設定されていません");
        }
    }

    private void LoadPositionsFromCSV(string csvText)
    {
        positions.Clear();
        string[] lines = csvText.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 1行目はヘッダ想定
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] values = lines[i].Split(',');
            if (values.Length < 4) continue;  // frame, x, y, z の4列必要

            if (float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                float.TryParse(values[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
            {
                positions.Add(new Vector3(x, y, z));
            }
        }
    }

    public void StartPlayback()
    {
        if (!isPlaying && positions.Count > 1)
        {
            isPlaying = true;
            currentFrame = 1;
            StartCoroutine(UpdatePosition());
        }
    }

    private IEnumerator UpdatePosition()
    {
        while (currentFrame < positions.Count)
        {
            transform.position = positions[currentFrame];
            currentFrame++;
            yield return new WaitForSeconds(frameInterval);
        }

        // 最後の位置で停止
        if (positions.Count > 0)
            transform.position = positions[positions.Count - 1];

        isPlaying = false;
        OnPlaybackFinished?.Invoke();
    }
}
