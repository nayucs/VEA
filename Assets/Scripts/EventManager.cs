using System.Collections;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private PositionUpdaterWORotation[] positionUpdaters;
    public AudioSource audioSource; // Inspector から設定

    void Start()
    {
        positionUpdaters = FindObjectsOfType<PositionUpdaterWORotation>();
        StartCoroutine(DelayedPlayback());
    }

    IEnumerator DelayedPlayback()
    {
        yield return new WaitForSeconds(10f); // 10秒待機

        // 音を鳴らす
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // 各PositionUpdaterに再生開始命令
        foreach (var updater in positionUpdaters)
        {
            if (updater != null)
            {
                updater.StartPlayback();
            }
        }
    }
}