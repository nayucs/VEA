using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUpdater : MonoBehaviour
{
    [Header("CSVファイル（TextAsset）")]
    public TextAsset csvFile;  // Inspector上でセット

    private List<FrameData> frameDataList = new List<FrameData>();
    private int currentFrame = 0;

    // フレームごとのデータクラス
    private class FrameData
    {
        public Vector3 position;
        public float rotationY;

        public FrameData(Vector3 pos, float rotY)
        {
            position = pos;
            rotationY = rotY;
        }
    }

    void Start()
    {
        if (csvFile != null)
        {
            LoadDataFromCSV(csvFile.text);

            if (frameDataList.Count > 0)
            {
                // 最初のフレーム（1フレーム目）を適用
                SetTransform(frameDataList[0]);

                // 10秒待ってから2フレーム目以降の再生を開始
                StartCoroutine(DelayedStart());
            }
        }
        else
        {
            Debug.LogError("CSVファイルがInspector上で設定されていません。");
        }
    }

    void LoadDataFromCSV(string csvText)
    {
        string[] lines = csvText.Split('\n');
        for (int i = 1; i < lines.Length; i++)  // 1行目はヘッダーなのでスキップ
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length < 5) continue;  // frame, x, y, z, rotyが必要

            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);
            float roty = float.Parse(values[4]);

            frameDataList.Add(new FrameData(new Vector3(x, y, z), roty));
        }
    }

    void SetTransform(FrameData data)
    {
        transform.position = data.position;
        transform.rotation = Quaternion.Euler(0, data.rotationY, 0);
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(10f);  // 10秒待機

        // 2フレーム目から再生開始
        currentFrame = 1;  // 2フレーム目（index=1）から
        StartCoroutine(UpdateTransform());
    }

    IEnumerator UpdateTransform()
    {
        while (currentFrame < frameDataList.Count)
        {
            SetTransform(frameDataList[currentFrame]);

            currentFrame++;
            yield return new WaitForSeconds(0.033f);  // 30fps想定（必要に応じて変更）
        }
    }
}
