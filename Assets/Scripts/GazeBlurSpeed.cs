using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GazeBlurSpeed : MonoBehaviour
{
    [Header("速度提供元")]
    public PositionSpeedUpdater speedSource;

    [Header("GazeBlur対象RawImage")]
    public RawImage gazeBlurUI;

    [Header("ブラー調整パラメータ")]
    public float minSpeed = 0f;
    public float maxSpeed = 5f;
    public float minBlurSize = 10f;
    public float maxBlurSize = 80f;
    public float minClearRadius = 0.2f;
    public float maxClearRadius = 0.05f;

    void Update()
    {
        if (speedSource == null || gazeBlurUI == null || gazeBlurUI.material == null) return;

        float speed = speedSource.currentSpeed;

        float blur = Mathf.Lerp(minBlurSize, maxBlurSize, Mathf.InverseLerp(minSpeed, maxSpeed, speed));
        float radius = Mathf.Lerp(minClearRadius, maxClearRadius, Mathf.InverseLerp(minSpeed, maxSpeed, speed));

        gazeBlurUI.material.SetFloat("_BlurSize", blur);
        gazeBlurUI.material.SetFloat("_ClearRadius", radius);
    }
}
