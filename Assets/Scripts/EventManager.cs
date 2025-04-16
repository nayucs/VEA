using System.Collections;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private PositionUpdaterWORotation[] positionUpdaters;
    public AudioSource audioSource;

    private HeadRotationRecorder recorder;

    void Start()
    {
        StartCoroutine(DelayedInit());
    }

    IEnumerator DelayedInit()
    {
        yield return null; // 他オブジェクトのStart実行を待つ

        positionUpdaters = FindObjectsOfType<PositionUpdaterWORotation>();
        recorder = FindObjectOfType<HeadRotationRecorder>();

        if (recorder == null)
        {
            Debug.LogWarning("HeadRotationRecorder が見つかりません。回転の記録は行われません。");
        }

        if (positionUpdaters.Length > 0)
        {
            positionUpdaters[0].OnPlaybackFinished += OnAllFinished;
        }

        StartCoroutine(DelayedPlayback());
    }

    IEnumerator DelayedPlayback()
    {
        yield return new WaitForSeconds(10f);

        if (audioSource != null)
        {
            audioSource.Play();
        }

        Debug.Log("再生開始");

        // 記録開始（recorder が null のときは何もしない）
        recorder?.StartRecording();

        foreach (var updater in positionUpdaters)
        {
            updater.StartPlayback();
        }
    }

    void OnAllFinished()
    {
        Debug.Log("全体の再生が完了しました（1体の終了をトリガーとする）");

        // 記録停止・保存
        recorder?.StopAndSave();
    }
}

