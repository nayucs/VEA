using System.Collections;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private PositionUpdater[] positionUpdatersWithRotation;
    private PositionUpdaterWORotation[] positionUpdatersWithoutRotation;
    private PlayerPositionUpdater[] playerPositionUpdaters;

    private AudioSource audioSource;
    public AudioClip sound1;

    private HeadRotationRecorder recorder;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(DelayedInit());
    }

    IEnumerator DelayedInit()
    {
        yield return null; // 他オブジェクトのStart実行を待つ

        positionUpdatersWithRotation = FindObjectsOfType<PositionUpdater>();
        positionUpdatersWithoutRotation = FindObjectsOfType<PositionUpdaterWORotation>();
        playerPositionUpdaters = FindObjectsOfType<PlayerPositionUpdater>();
        recorder = FindObjectOfType<HeadRotationRecorder>();

        if (recorder == null)
        {
            Debug.LogWarning("HeadRotationRecorder が見つかりません。回転の記録は行われません。");
        }

        // どれか一体だけに終了イベントを登録
        if (positionUpdatersWithRotation.Length > 0)
        {
            positionUpdatersWithRotation[0].OnPlaybackFinished += OnAllFinished;
        }
        else if (positionUpdatersWithoutRotation.Length > 0)
        {
            positionUpdatersWithoutRotation[0].OnPlaybackFinished += OnAllFinished;
        }
        else if (playerPositionUpdaters.Length > 0)
        {
            playerPositionUpdaters[0].OnPlaybackFinished += OnAllFinished;
        }

        StartCoroutine(DelayedPlayback());
    }

    IEnumerator DelayedPlayback()
    {
        yield return new WaitForSeconds(7f);

        if (audioSource != null)
        {
            audioSource.PlayOneShot(sound1);
        }

        yield return new WaitForSeconds(3f);

        Debug.Log("再生開始");

        // 記録開始（recorder が null のときは何もしない）
        recorder?.StartRecording();

        // それぞれの再生を開始
        foreach (var updater in positionUpdatersWithRotation)
        {
            updater.StartPlayback();
        }

        foreach (var updater in positionUpdatersWithoutRotation)
        {
            updater.StartPlayback();
        }

        foreach (var updater in playerPositionUpdaters)
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
