using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUpdater : MonoBehaviour
{
    [Header("CSVファイル（TextAsset）")]
    public TextAsset csvFile;

    private List<Vector3> positions = new List<Vector3>();
    private List<float> rotationsY = new List<float>();
    private int currentFrame = 0;
    private bool isPlaying = false;

    public event Action OnPlaybackFinished;

    void Start()
    {
        if (csvFile != null)
        {
            LoadPositionsAndRotationsFromCSV(csvFile.text);
            if (positions.Count > 0)
            {
                transform.position = positions[0];
                transform.rotation = Quaternion.Euler(0f, rotationsY[0], 0f);
            }
        }
        else
        {
            Debug.LogError("CSVファイルがInspector上で設定されていません。");
        }
    }

    void LoadPositionsAndRotationsFromCSV(string csvText)
    {
        string[] lines = csvText.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] values = lines[i].Split(',');
            if (values.Length < 5) continue;

            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);
            float roty = float.Parse(values[4]);

            positions.Add(new Vector3(x, y, z));
            rotationsY.Add(roty);
        }
    }

    public void StartPlayback()
    {
        if (!isPlaying && positions.Count > 1)
        {
            isPlaying = true;
            currentFrame = 1;
            StartCoroutine(UpdatePositionAndRotation());
        }
    }

    IEnumerator UpdatePositionAndRotation()
    {
        while (currentFrame < positions.Count)
        {
            transform.position = positions[currentFrame];
            transform.rotation = Quaternion.Euler(0f, rotationsY[currentFrame], 0f);
            currentFrame++;
            yield return new WaitForSeconds(0.033f);
        }

        if (positions.Count > 0)
        {
            transform.position = positions[positions.Count - 1];
            transform.rotation = Quaternion.Euler(0f, rotationsY[rotationsY.Count - 1], 0f);
        }

        isPlaying = false;
        OnPlaybackFinished?.Invoke();
    }
}
