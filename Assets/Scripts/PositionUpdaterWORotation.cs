using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUpdaterWORotation : MonoBehaviour
{
    [Header("CSVファイル（TextAsset）")]
    public TextAsset csvFile;  // Inspector上でセット

    private List<Vector3> positions = new List<Vector3>();
    private int currentFrame = 0;

    void Start()
    {
        if (csvFile != null)
        {
            LoadPositionsFromCSV(csvFile.text);

            if (positions.Count > 0)
            {
                // 最初のフレームの位置を適用（回転は考慮しない）
                transform.position = positions[0];

                // 10秒待機後に2フレーム目以降を再生
                StartCoroutine(DelayedStart());
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
        for (int i = 1; i < lines.Length; i++)  // 1行目はヘッダーなのでスキップ
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length < 4) continue;  // frame, x, y, z まで

            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);

            positions.Add(new Vector3(x, y, z));
        }
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(10f);  // 10秒待機

        // 2フレーム目から再生開始
        currentFrame = 1;
        StartCoroutine(UpdatePosition());
    }

    IEnumerator UpdatePosition()
    {
        while (currentFrame < positions.Count)
        {
            transform.position = positions[currentFrame];
            currentFrame++;
            yield return new WaitForSeconds(0.033f);  // 30fps（適宜変更）
        }
    }
}
