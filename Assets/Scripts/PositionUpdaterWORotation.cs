using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUpdaterWORotation : MonoBehaviour
{
    [Header("CSVファイル（TextAsset）")]
    public TextAsset csvFile;

    private List<Vector3> positions = new List<Vector3>();
    private int currentFrame = 0;
    private bool isPlaying = false;

    // 再生終了時に呼ばれるイベント（EventManagerが購読する）
    public event Action OnPlaybackFinished;

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
            StartCoroutine(UpdatePosition());
        }
    }

    IEnumerator UpdatePosition()
    {
        while (currentFrame < positions.Count)
        {
            transform.position = positions[currentFrame];
            currentFrame++;
            yield return new WaitForSeconds(0.033f);
        }

        // 最後の位置を保持
        if (positions.Count > 0)
        {
            transform.position = positions[positions.Count - 1];
        }

        isPlaying = false;

        // 再生終了イベント発火
        OnPlaybackFinished?.Invoke();
    }
}
