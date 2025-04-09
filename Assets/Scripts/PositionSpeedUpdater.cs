using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSpeedUpdater : MonoBehaviour
{
    [Header("CSVファイル（TextAsset）")]
    public TextAsset csvFile;

    public float currentSpeed { get; private set; } // ← Header を外して修正済！

    [Header("フレーム間隔 (秒)")]
    public float frameInterval = 0.033f; // 30fps相当

    private List<Vector3> positions = new List<Vector3>();
    private int currentFrame = 0;

    void Start()
    {
        if (csvFile != null)
        {
            LoadPositionsFromCSV(csvFile.text);
            if (positions.Count > 0)
            {
                transform.position = positions[0];
                StartCoroutine(DelayedStart());
            }
        }
        else
        {
            Debug.LogError("CSVファイルが設定されていません。");
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

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(10f);
        currentFrame = 1;
        StartCoroutine(UpdatePosition());
    }

    IEnumerator UpdatePosition()
    {
        while (currentFrame < positions.Count)
        {
            Vector3 prev = transform.position;
            Vector3 next = positions[currentFrame];

            transform.position = next;

            float distance = Vector3.Distance(prev, next);
            currentSpeed = distance / frameInterval;

            currentFrame++;
            yield return new WaitForSeconds(frameInterval);
        }
    }
}
