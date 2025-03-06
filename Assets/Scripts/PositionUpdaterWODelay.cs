using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUpdaterWODelay : MonoBehaviour
{
    [Header("CSVファイル（TextAsset）")]
    public TextAsset csvFile;  // Inspectorで設定する

    private List<FrameData> frameDataList = new List<FrameData>();
    private int currentFrame = 0;

    // フレームごとのデータをまとめたクラス
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
            StartCoroutine(UpdateTransform());
        }
        else
        {
            Debug.LogError("CSVファイルがInspector上で設定されていません。");
        }
    }

    void LoadDataFromCSV(string csvText)
    {
        string[] lines = csvText.Split('\n');
        for (int i = 1; i < lines.Length; i++) // 1行目はヘッダーなのでスキップ
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length < 5) continue;  // x, y, z, rotyが必要

            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);
            float roty = float.Parse(values[4]);

            frameDataList.Add(new FrameData(new Vector3(x, y, z), roty));
        }
    }

    IEnumerator UpdateTransform()
    {
        while (currentFrame < frameDataList.Count)
        {
            FrameData data = frameDataList[currentFrame];

            // 座標と回転を更新
            transform.position = data.position;
            transform.rotation = Quaternion.Euler(0, data.rotationY, 0);

            currentFrame++;
            yield return new WaitForSeconds(0.033f);  // 30fps想定（必要に応じて変更）
        }
    }
}
